using squad_dma.Source.Squad.Features;
using squad_dma.Source.Squad.Debug;
using System.Collections.ObjectModel;
using System.Numerics;

namespace squad_dma
{
    /// <summary>
    /// Class containing Game instance.
    /// </summary>
    public class Game
    {
        #region Fields
        private readonly ulong _squadBase;
        private volatile bool _inGame = false;
        private RegistredActors _actors;
        private UActor _localUPlayer;
        private ulong _gameWorld;
        private ulong _gameInstance;
        private ulong _localPlayer;
        private ulong _playerController;
        private Vector3 _absoluteLocation;
        private string _currentLevel = string.Empty;
        private DateTime _lastTeamCheck = DateTime.MinValue;
        private const int TeamCheckInterval = 1000;

        private Source.Squad.Manager _soldierManager;

        private GameTickets _gameTickets;
        private PlayerStats _gameStats;
        private DebugVehicles _debugVehicles;
        private DebugTeam _debugTeam;
        private DebugSoldier _debugSoldier;
        #endregion

        #region Properties
        public bool InGame => _inGame;
        public string MapName => _currentLevel;
        public UActor LocalPlayer => _localUPlayer;
        public ReadOnlyDictionary<ulong, UActor> Actors => _actors?.Actors;
        public Vector3 AbsoluteLocation => _absoluteLocation;
        public Dictionary<int, int> TeamTickets => _gameTickets.GetTickets();
        public GameTickets GameTickets => _gameTickets;
        public PlayerStats GameStats => _gameStats;
        #endregion

        #region Constructor
        public Game(ulong squadBase)
        {
            _squadBase = squadBase;
            _gameTickets = null;
            _gameStats = null;
        }
        #endregion

        #region Public Methods
        public void SetInstantSeatSwitch() => _debugVehicles?.SetInstantSeatSwitch();
        public void LogVehicles(bool force = false) => _debugVehicles?.LogVehicles(force);
        public void VehicleTeam() => _debugVehicles?.VehicleTeam();
        public void LogTeamInfo() => _debugTeam?.LogTeamInfo();
        public void SetSuppression(bool enable) => _soldierManager?.SetSuppression(enable);
        public void SetInteractionDistances(bool enable) => _soldierManager?.SetInteractionDistances(enable);
        public void SetShootingInMainBase(bool enable) => _soldierManager?.SetShootingInMainBase(enable);
        public void SetSpeedHack(bool enable) => _soldierManager?.SetSpeedHack(enable);
        public void SetAirStuck(bool enable) => _soldierManager?.SetAirStuck(enable);
        public void SetHideActor(bool enable) => _soldierManager?.SetHideActor(enable);
        public void DisableCollision(bool disable) => _soldierManager?.DisableCollision(disable);
        public void SetQuickZoom(bool enable) => _soldierManager?.SetQuickZoom(enable);
        public void SetRapidFire(bool enable) => _soldierManager?.SetRapidFire(enable);
        public void SetInfiniteAmmo(bool enable) => _soldierManager?.SetInfiniteAmmo(enable);
        public void SetQuickSwap(bool enable) => _soldierManager?.SetQuickSwap(enable);
        public void ReadCurrentWeapons(bool includeOtherPlayers = false) => _debugSoldier?.ReadCurrentWeapons(includeOtherPlayers);
        public void LogCurrentValues() => _debugSoldier?.LogCurrentValues();
        public void WaitForGame()
        {
            while (true)
            {
                try
                {
                    if (!Memory.GetModuleBase())
                    {
                        throw new GameNotRunningException("Process terminated during wait");
                    }

                    if (GetGameWorld() && GetGameInstance() && GetCurrentLevel() && InitActors() && GetLocalPlayer())
                    {
                        if (!Memory.GetModuleBase())
                        {
                            throw new GameNotRunningException("Process terminated during initialization");
                        }

                        Thread.Sleep(1000);
                        Program.Log("Game has started!!");
                        this._inGame = true;
                        Memory.GameStatus = GameStatus.InGame;
                        
                        _gameTickets = new GameTickets(_gameWorld, _localUPlayer);
                        _gameStats = new PlayerStats(_playerController);
                        
                        InitializeManagers();
                        
                        return;
                    }
                }
                catch (GameNotRunningException)
                {
                    throw; // Propagate up to break out of wait loop
                }
                Thread.Sleep(500);
            }
        }

        public void GameLoop()
        {
            try
            {
                if (!this._inGame)
                {
                    throw new GameEnded("Game has ended!");
                }

                UpdateLocalPlayerInfo();
                this._actors.UpdateList();
                this._actors.UpdateAllPlayers();
            }
            catch (DMAShutdown)
            {
                HandleDMAShutdown();
            }
            catch (GameEnded e)
            {
                HandleGameEnded(e);
            }
            catch (Exception ex)
            {
                HandleUnexpectedException(ex);
            }
        }
        #endregion

        #region Private Methods
        private void InitializeManagers()
        {
            _soldierManager = new Source.Squad.Manager(_playerController, _inGame, _actors);

            _debugVehicles = new DebugVehicles(_playerController, _inGame, _actors);
            _debugTeam = new DebugTeam(_inGame, _localUPlayer, _actors?.Actors);
            _debugSoldier = new DebugSoldier(_playerController, _inGame);
        }

        private bool TryExecute(Action action)
        {
            try
            {
                action();
                return true;
            }
            catch { return false; }
        }

        private void HandleDMAShutdown()
        {
            Program.Log("DMA shutdown");
            this._inGame = false;
        }

        private void HandleGameEnded(GameEnded e)
        {
            Program.Log("Game has ended!");
            this._inGame = false;
            Memory.GameStatus = GameStatus.Menu;
            Memory.Restart();
        }

        private void HandleUnexpectedException(Exception ex)
        {
            Program.Log($"CRITICAL ERROR - Game ended due to unhandled exception: {ex}");
            this._inGame = false;
        }

        private bool GetGameWorld() => 
            TryExecute(() => _gameWorld = Memory.ReadPtr(_squadBase + Offsets.GameObjects.GWorld));
  
        private bool GetGameInstance() => 
            TryExecute(() => _gameInstance = Memory.ReadPtr(_gameWorld + Offsets.World.OwningGameInstance));

        private bool GetCurrentLevel() => 
            TryExecute(() => 
            {
                var currentLayer = Memory.ReadPtr(_gameInstance + Offsets.GameInstance.CurrentLayer);
                var currentLevelIdPtr = currentLayer + Offsets.SQLayer.LevelID;
                var currentLevelId = Memory.ReadValue<uint>(currentLevelIdPtr);
                _currentLevel = Memory.GetNamesById([currentLevelId])[currentLevelId];
                Program.Log($"Current level is {_currentLevel}");
            });

        private bool InitActors() => 
            TryExecute(() => 
            {
                var persistentLevel = Memory.ReadPtr(_gameWorld + Offsets.World.PersistentLevel);
                _actors = new RegistredActors(persistentLevel);
            });
  
        private bool GetLocalPlayer() => 
            TryExecute(() => 
            {
                var localPlayers = Memory.ReadPtr(_gameInstance + Offsets.GameInstance.LocalPlayers);
                _localPlayer = Memory.ReadPtr(localPlayers);
                _localUPlayer = new UActor(_localPlayer);
                _localUPlayer.Team = Team.Unknown;
                GetPlayerController();
            });

        private bool GetPlayerController() => 
            TryExecute(() => _playerController = Memory.ReadPtr(_localPlayer + Offsets.UPlayer.PlayerController));

        private bool UpdateLocalPlayerInfo()
        {
            try
            {
                if ((DateTime.Now - _lastTeamCheck).TotalMilliseconds > TeamCheckInterval)
                {
                    _lastTeamCheck = DateTime.Now;

                    try
                    {
                        ulong playerState = Memory.ReadPtr(_playerController + Offsets.Controller.PlayerState);
                        ulong squadState = Memory.ReadPtr(_playerController + Offsets.PlayerController.SquadState);

                        if (playerState == 0 || squadState == 0)
                            return false;

                        int teamId = Memory.ReadValue<int>(playerState + Offsets.ASQPlayerState.TeamID);
                        int squadId = Memory.ReadValue<int>(squadState + Offsets.ASQSquadState.SquadId);

                        if (_localUPlayer.TeamID != teamId || _localUPlayer.SquadID != squadId)
                        {
                            _localUPlayer.TeamID = teamId;
                            _localUPlayer.SquadID = squadId;
                        }
                    }
                    catch { return false; }
                }
                GetCameraCache();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool GetCameraCache()
        {
            try
            {
                var cameraInfoScatterMap = new ScatterReadMap(1);
                var cameraManagerRound = cameraInfoScatterMap.AddRound();
                var cameraInfoRound = cameraInfoScatterMap.AddRound();

                var cameraManagerPtr = cameraManagerRound.AddEntry<ulong>(0, 0, _playerController + Offsets.PlayerController.PlayerCameraManager);
                cameraManagerRound.AddEntry<int>(0, 11, _gameWorld + Offsets.World.WorldOrigin);
                cameraManagerRound.AddEntry<int>(0, 12, _gameWorld + Offsets.World.WorldOrigin + 0x4);
                cameraManagerRound.AddEntry<int>(0, 13, _gameWorld + Offsets.World.WorldOrigin + 0x8);
                cameraInfoRound.AddEntry<Vector3>(0, 1, cameraManagerPtr, null, Offsets.Camera.CameraLocation);
                cameraInfoRound.AddEntry<Vector3>(0, 2, cameraManagerPtr, null, Offsets.Camera.CameraRotation);

                cameraInfoScatterMap.Execute();

                if (!cameraInfoScatterMap.Results[0][1].TryGetResult<Vector3>(out var location))
                {
                    return false;
                }
                if (!cameraInfoScatterMap.Results[0][2].TryGetResult<Vector3>(out var rotation))
                {
                    return false;
                }
                if (cameraInfoScatterMap.Results[0][11].TryGetResult<int>(out var absoluteX)
                && cameraInfoScatterMap.Results[0][12].TryGetResult<int>(out var absoluteY)
                && cameraInfoScatterMap.Results[0][13].TryGetResult<int>(out var absoluteZ))
                {
                    _absoluteLocation = new Vector3(absoluteX, absoluteY, absoluteZ);
                }
                _localUPlayer.Position = location;
                _localUPlayer.Rotation = new Vector2(rotation.Y, rotation.X);
                _localUPlayer.Rotation3D = rotation;
                return true;
            }
            catch { return false; }
        }
        #endregion
    }

    #region Exceptions
    public class GameNotRunningException : Exception
    {
        public GameNotRunningException() { }
        public GameNotRunningException(string message) : base(message) { }
        public GameNotRunningException(string message, Exception inner) : base(message, inner) { }
    }

    public class GameEnded : Exception
    {
        public GameEnded() { }
        public GameEnded(string message) : base(message) { }
        public GameEnded(string message, Exception inner) : base(message, inner) { }
    }
    #endregion
}