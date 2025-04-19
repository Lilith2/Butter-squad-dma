using Offsets;
using squad_dma.Source.Squad.Features;

namespace squad_dma.Source.Squad
{
    public class Manager : IDisposable
    {
        public readonly ulong _playerController;
        public readonly bool _inGame;
        private readonly Config _config;
        private CancellationTokenSource _cancellationTokenSource;
        
        protected ulong _cachedPlayerState = 0;
        protected ulong _cachedSoldierActor = 0;
        protected ulong _cachedInventoryComponent = 0;
        protected ulong _cachedCurrentWeapon = 0;
        protected ulong _cachedCharacterMovement = 0;
        protected DateTime _lastPointerUpdate = DateTime.MinValue;
        
        // Modules
        private Suppression _suppression;
        private InteractionDistances _interactionDistances;
        private ShootingInMainBase _shootingInMainBase;
        private SpeedHack _speedHack;
        private Collision _collision;
        private AirStuck _airStuck;
        private QuickZoom _quickZoom;
        private RapidFire _rapidFire;
        private InfiniteAmmo _infiniteAmmo;
        private QuickSwap _quickSwap;
        private ForceFullAuto _FullAuto;
        private NoCameraShake _noCameraShake;
        private NoSpread _noSpread;
        private NoRecoil _noRecoil;
        private NoSway _noSway;

        /// <summary>
        /// Constructor for feature classes that inherit from Manager
        /// </summary>
        /// <param name="playerController">The player controller address</param>
        /// <param name="inGame">Whether the game is currently active</param>
        public Manager(ulong playerController, bool inGame)
        {
            _playerController = playerController;
            _inGame = inGame;
            _cancellationTokenSource = null; // Features don't need this
            
            UpdateCachedPointers();
        }
        
        public Manager(ulong playerController, bool inGame, RegistredActors actors)
        {
            _playerController = playerController;
            _inGame = inGame;
            _cancellationTokenSource = new CancellationTokenSource();
            
            // Initialize cached pointers
            UpdateCachedPointers();
            
            // Initialize all feature modules
            InitializeFeatures();
            
            // Start a timer to apply features
            StartFeatureTimer();
        }
        
        /// <summary>
        /// Updates cached pointers to avoid redundant memory reads
        /// </summary>
        protected void UpdateCachedPointers()
        {
            try
            {
                if ((DateTime.Now - _lastPointerUpdate).TotalMilliseconds < 500 && 
                    _cachedPlayerState != 0 && _cachedSoldierActor != 0)
                    return;
                    
                if (!_inGame || _playerController == 0) return;
                
                _cachedPlayerState = Memory.ReadPtr(_playerController + Controller.PlayerState);
                if (_cachedPlayerState == 0) return;
                
                _cachedSoldierActor = Memory.ReadPtr(_cachedPlayerState + ASQPlayerState.Soldier);
                if (_cachedSoldierActor == 0) return;
                
                _cachedInventoryComponent = Memory.ReadPtr(_cachedSoldierActor + ASQSoldier.InventoryComponent);
                if (_cachedInventoryComponent != 0)
                {
                    _cachedCurrentWeapon = Memory.ReadPtr(_cachedInventoryComponent + USQPawnInventoryComponent.CurrentWeapon);
                }
                
                _cachedCharacterMovement = Memory.ReadPtr(_cachedSoldierActor + Character.CharacterMovement);
                
                _lastPointerUpdate = DateTime.Now;
            }
            catch
            {
                // Reset pointers on error
                _cachedPlayerState = 0;
                _cachedSoldierActor = 0;
                _cachedInventoryComponent = 0;
                _cachedCurrentWeapon = 0;
                _cachedCharacterMovement = 0;
            }
        }
        
        private void InitializeFeatures()
        {
            _suppression = new Suppression(_playerController, _inGame);
            _interactionDistances = new InteractionDistances(_playerController, _inGame);
            _shootingInMainBase = new ShootingInMainBase(_playerController, _inGame);
            _speedHack = new SpeedHack(_playerController, _inGame);
            _collision = new Collision(_playerController, _inGame);
            _airStuck = new AirStuck(_playerController, _inGame, _collision);
            _quickZoom = new QuickZoom(_playerController, _inGame);
            _rapidFire = new RapidFire(_playerController, _inGame);
            _infiniteAmmo = new InfiniteAmmo(_playerController, _inGame);
            _quickSwap = new QuickSwap(_playerController, _inGame);
            _FullAuto = new ForceFullAuto(_playerController, _inGame);
            _noCameraShake = new NoCameraShake(_playerController, _inGame);
            _noSpread = new NoSpread(_playerController, _inGame);
            _noRecoil = new NoRecoil(_playerController, _inGame);
            _noSway = new NoSway(_playerController, _inGame);
        }
        
        /// <summary>
        /// Checks if the local player is valid (has a valid player state and soldier actor)
        /// </summary>
        /// <returns>True if local player is valid, false otherwise</returns>
        public bool IsLocalPlayerValid()
        {
            try
            {
                if (!_inGame || _playerController == 0) return false;
                
                ulong playerState = _cachedPlayerState != 0 ? _cachedPlayerState : Memory.ReadPtr(_playerController + Controller.PlayerState);
                if (playerState == 0) return false;
                
                ulong soldierActor = _cachedSoldierActor != 0 ? _cachedSoldierActor : Memory.ReadPtr(playerState + ASQPlayerState.Soldier);
                if (soldierActor == 0) return false;
                
                return true;
            }
            catch
            { return false; }
        }
        
        // Start a timer to apply features every second
        // Simple fix for when the Localplayer respawns
        // Need to change it to a better solution
        private void StartFeatureTimer()
        {
            Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        UpdateCachedPointers();

                        if (_config.DisableSuppression)
                            _suppression.Apply();
                        if (_config.SetInteractionDistances)
                            _interactionDistances.Apply();
                        if (_config.AllowShootingInMainBase)
                            _shootingInMainBase.Apply();
                        if (_config.SetSpeedHack)
                            _speedHack.Apply();
                        if (_config.SetAirStuck)
                            _airStuck.Apply();
                        
                        // Handle DisableCollision, ensuring it's disabled if AirStuck is disabled
                        if (!_config.SetAirStuck && _config.DisableCollision)
                        {
                            _config.DisableCollision = false;
                            _collision.SetEnabled(false);
                        }
                        
                        if (_config.DisableCollision)
                            _collision.Apply();
                        if (_config.RapidFire)
                            _rapidFire.Apply();
                        if (_config.InfiniteAmmo)
                            _infiniteAmmo.Apply();
                        if (_config.QuickSwap)
                            _quickSwap.Apply();
                        if (_config.ForceFullAuto)
                            _FullAuto.Apply();
                        if (_config.NoCameraShake)
                            _noCameraShake.Apply();
                        if (_config.NoSpread)
                            _noSpread.Apply();
                        if (_config.NoRecoil)
                            _noRecoil.Apply();
                        if (_config.NoSway)
                            _noSway.Apply();
                    }
                    catch { /* Silently fail */ }
                    await Task.Delay(1000, _cancellationTokenSource.Token);
                }
            }, _cancellationTokenSource.Token);
        }
        
        /// <summary>
        /// Apply method to be implemented by features
        /// </summary>
        public virtual void Apply() { }
        
        #region Feature Control Methods
        
        public void SetSuppression(bool enable)
        {
            _suppression.SetEnabled(enable);
        }
        
        public void SetInteractionDistances(bool enable)
        {
            _interactionDistances.SetEnabled(enable);
        }
        
        public void SetShootingInMainBase(bool enable)
        {
            _shootingInMainBase.SetEnabled(enable);
        }
        
        public void SetSpeedHack(bool enable)
        {
            _speedHack.SetEnabled(enable);
        }
        
        public void SetAirStuck(bool enable)
        {
            _airStuck.SetEnabled(enable);
        }
        
        public void SetQuickZoom(bool enable)
        {
            _quickZoom.SetEnabled(enable);
        }
        

        
        public void DisableCollision(bool disable)
        {
            // Only allow enabling if AirStuck is enabled
            if (disable && !Program.Config.SetAirStuck)
            {
                return;
            }
            
            _collision.SetEnabled(disable);
        }
        
        public void SetRapidFire(bool enable)
        {
            _rapidFire.SetEnabled(enable);
        }

        public void SetFullAuto(bool enable)
        {
            _FullAuto.SetEnabled(enable);
        }
        
        public void SetInfiniteAmmo(bool enable)
        {
            _infiniteAmmo.SetEnabled(enable);
        }
        
        public void SetQuickSwap(bool enable)
        {
            _quickSwap.SetEnabled(enable);
        }
        
        public void SetNoCameraShake(bool enable)
        {
            _noCameraShake.SetEnabled(enable);
        }
        
        public void SetNoSpread(bool enable)
        {
            _noSpread.SetEnabled(enable);
        }

        public void SetNoRecoil(bool enable)
        {
            _noRecoil.SetEnabled(enable);
        }

        public void SetNoSway(bool enable)
        {
            _noSway.SetEnabled(enable);
        }
        
        public void Dispose()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }
        }
        #endregion
    }
}