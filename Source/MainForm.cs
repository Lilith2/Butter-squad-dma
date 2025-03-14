using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Numerics;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using DarkModeForms;
using MaterialSkin.Controls;
using squad_dma.Properties;

namespace squad_dma
{
    public partial class MainForm : Form
    {
        private readonly Config _config;
        private readonly SKGLControl _mapCanvas;
        private readonly Stopwatch _fpsWatch = new();
        private readonly object _renderLock = new();
        private readonly object _loadMapBitmapsLock = new();
        private readonly System.Timers.Timer _mapChangeTimer = new(100);
        private readonly List<Map> _maps = new(); // Contains all maps from \\Maps folder
        private readonly DarkModeCS _darkmode;
        private bool _isFreeMapToggled = false;
        private float _uiScale = 1.0f;
        private UActor _closestPlayerToMouse = null;
        private bool _isDragging = false;
        private Point _lastMousePosition = Point.Empty;
        private int _fps = 0;
        private int _mapSelectionIndex = 0;
        private Map _selectedMap;
        private SKBitmap[] _loadedBitmaps;
        private MapPosition _mapPanPosition = new();
        private readonly List<PointOfInterest> _pointsOfInterest = new();
        private PointOfInterest _hoveredPoi;
        private const int POI_HOVER_THRESHOLD = 15;

        private const int ZOOM_INTERVAL = 2;
        private int targetZoomValue = 0;
        private System.Windows.Forms.Timer zoomTimer;

        private const float DRAG_SENSITIVITY = 3.5f;

        private const double PAN_SMOOTHNESS = 0.1;
        private const int PAN_INTERVAL = 10;
        private SKPoint targetPanPosition;
        private System.Windows.Forms.Timer panTimer;

        #region Getters
        /// <summary>
        /// Radar has found process and is ready.
        /// </summary>
        private bool Ready
        {
            get => Memory.Ready;
        }

        /// <summary>
        /// Radar has found Local Game World.
        /// </summary>
        private bool InGame
        {
            get => Memory.InGame;
        }
        private string MapName
        {
            get => Memory.MapName;
        }

        /// <summary>
        /// LocalPlayer (who is running Radar) 'Player' object.
        /// </summary>
        private UActor LocalPlayer
        {
            get => Memory.LocalPlayer;
        }

        /// <summary>
        /// All Actors in Game World (including tech)
        /// </summary>
        private ReadOnlyDictionary<ulong, UActor> AllActors
        {
            get => Memory.Actors;
        }

        private Vector3 AbsoluteLocation
        {
            get => Memory.AbsoluteLocation;
        }

        public void AddPointOfInterest(Vector3 position, string name)
        {
            _pointsOfInterest.Add(new PointOfInterest(position, name));
        }

        public void RemovePointOfInterest(string name)
        {
            var poi = _pointsOfInterest.FirstOrDefault(p => p.Name == name);
            if (poi != null)
            {
                _pointsOfInterest.Remove(poi);
            }
        }

        public void ClearPointsOfInterest()
        {
            _pointsOfInterest.Clear();
        }

        #endregion

        #region Constructor
        /// <summary>
        /// GUI Constructor.
        /// </summary>
        public MainForm()
        {
            _config = Program.Config;

            InitializeComponent();
            SetDarkMode(ref _darkmode);

            _mapCanvas = new SKGLControl()
            {
                Size = new Size(50, 50),
                Dock = DockStyle.Fill,
                VSync = false
            };
            tabRadar.Controls.Add(_mapCanvas);
            chkMapFree.Parent = _mapCanvas;

            LoadConfig();
            LoadMaps();

            _mapChangeTimer.AutoReset = false;
            _mapChangeTimer.Elapsed += MapChangeTimer_Elapsed;

            this.DoubleBuffered = true;
            this.Shown += frmMain_Shown;

            _mapCanvas.PaintSurface += skMapCanvas_PaintSurface;
            _mapCanvas.MouseMove += skMapCanvas_MouseMove;;
            _mapCanvas.MouseDown += skMapCanvas_MouseDown;
            _mapCanvas.MouseDoubleClick += skMapCanvas_MouseDoubleClick;
            _mapCanvas.MouseUp += skMapCanvas_MouseUp;

            tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
            //_fpsWatch.Start();

            zoomTimer = new System.Windows.Forms.Timer();
            zoomTimer.Interval = ZOOM_INTERVAL;
            zoomTimer.Tick += ZoomTimer_Tick;

            panTimer = new System.Windows.Forms.Timer();
            panTimer.Interval = PAN_INTERVAL;
            panTimer.Tick += PanTimer_Tick;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Set Dark Mode on startup.
        /// </summary>
        /// <param name="darkmode"></param>
        private void SetDarkMode(ref DarkModeCS darkmode)
        {
            darkmode = new DarkModeCS(this);
           // if (darkmode.IsDarkMode)
           // {
           //     SharedPaints.PaintBitmap.ColorFilter = SharedPaints.GetDarkModeColorFilter(0.7f);
            //    SharedPaints.PaintBitmapAlpha.ColorFilter = SharedPaints.GetDarkModeColorFilter(0.7f);
           // }
        }

        /// <summary>
        /// Form closing event.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            e.Cancel = true; // Cancel shutdown
            this.Enabled = false; // Lock window

            CleanupLoadedBitmaps();
            Config.SaveConfig(_config); // Save Config to Config.json
            Memory.Shutdown(); // Wait for Memory Thread to gracefully exit
            e.Cancel = false; // Ready to close
            base.OnFormClosing(e); // Proceed with closing
        }

        /// <summary>
        /// Process hotkey presses.sc
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) => keyData switch
        {
        Keys.F1 => ZoomIn(5),
        Keys.F2 => ZoomOut(5),
        Keys.F5 => ToggleMap(),
        Keys.F11 => ToggleFullscreen(FormBorderStyle is FormBorderStyle.Sizable),
            _ => base.ProcessCmdKey(ref msg, keyData),
        };

        /// <summary>
        /// Process mousewheel events.
        /// </summary>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (tabControl.SelectedIndex == 0) // Main Radar Tab should be open
            {
                var zoomSens = (double)_config.ZoomSensitivity / 175;
                int zoomDelta = -(int)(e.Delta * zoomSens);

                if (zoomDelta < 0)
                    ZoomIn(-zoomDelta);
                else if (zoomDelta > 0)
                    ZoomOut(zoomDelta);

                if (this._isFreeMapToggled && zoomDelta < 0) // Only move the zoom position when scrolling in
                {
                    var mousePos = this._mapCanvas.PointToClient(Cursor.Position);
                    var mapParams = GetMapLocation();
                    var mapMousePos = new SKPoint(
                        mapParams.Bounds.Left + mousePos.X / mapParams.XScale,
                        mapParams.Bounds.Top + mousePos.Y / mapParams.YScale
                    );

                    this.targetPanPosition = mapMousePos;

                    if (!this.panTimer.Enabled)
                        this.panTimer.Start();
                }

                return;
            }

            base.OnMouseWheel(e);
        }
        #endregion

        #region GUI Events / Functions
        #region General Helper Functions
        private bool ToggleMap()
        {
            if (!btnToggleMap.Enabled)
                return false;

            if (_mapSelectionIndex == _maps.Count - 1)
                _mapSelectionIndex = 0; // Start over when end of maps reached
            else
                _mapSelectionIndex++; // Move onto next map

            tabRadar.Text = $"Radar ({_maps[_mapSelectionIndex].Name})";
            _mapChangeTimer.Restart(); // Start delay
            Program.Log("Toggled Map");

            return true;
        }

        private void InitiateUIScaling()
        {
            _uiScale = (.01f * _config.UIScale);

            #region Update Paints/Text
            SKPaints.TextBaseOutline.StrokeWidth = 2 * _uiScale;
            SKPaints.TextRadarStatus.TextSize = 48 * _uiScale;
            SKPaints.PaintBase.StrokeWidth = 3 * _uiScale;
            SKPaints.PaintTransparentBacker.StrokeWidth = 1 * _uiScale;
            #endregion

            InitiateFontSize();
        }

        private void InitiateFont()
        {
            var fontToUse = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
            SKPaints.TextBase.Typeface = fontToUse;
            SKPaints.TextBaseOutline.Typeface = fontToUse;
            SKPaints.TextRadarStatus.Typeface = fontToUse;
        }

        private void InitiateFontSize()
        {
            SKPaints.TextBase.TextSize = _config.FontSize * _uiScale;
            SKPaints.TextBaseOutline.TextSize = _config.FontSize * _uiScale;
        }

        private DialogResult ShowErrorDialog(string message)
        {
            return new MaterialDialog(this, "Error", message, "OK", false, "", true).ShowDialog(this);
        }

        private DialogResult ShowConfirmationDialog(string message, string title)
        {
            return new MaterialDialog(this, title, message, "Yes", true, "No", true).ShowDialog(this);
        }

        private void LoadMaps()
        {
            var dir = new DirectoryInfo($"{Environment.CurrentDirectory}\\Maps");
            if (!dir.Exists)
                dir.Create();

            var configs = dir.GetFiles("*.json");
            //Debug.WriteLine($"Found {configs.Length} .json map configs.");
            if (configs.Length == 0)
                throw new IOException("No .json map configs found!");

            foreach (var config in configs)
            {
                var name = Path.GetFileNameWithoutExtension(config.Name);
                //Debug.WriteLine($"Loading Map: {name}");
                var mapConfig = MapConfig.LoadFromFile(config.FullName); // Assuming LoadFromFile is updated to handle new JSON format
                //Add map ID to map config
                var mapID = mapConfig.MapID[0];
                var map = new Map(name.ToUpper(), mapConfig, config.FullName, mapID);
                // Assuming map.ConfigFile now has a 'mapLayers' property that is a List of a new type matching the JSON structure
                map.ConfigFile.MapLayers = map.ConfigFile
                    .MapLayers
                    .OrderBy(x => x.MinHeight)
                    .ToList();

                _maps.Add(map);
            }
        }

        private void LoadConfig()
        {
            #region Settings
            #region General
            // User Interface
            chkShowAimview.Checked = _config.AimviewEnabled;
            chkHideNames.Checked = _config.ShowNames;
            trkAimLength.Value = _config.PlayerAimLineLength;
            trkZoomSensivity.Value = _config.ZoomSensitivity;

            trkUIScale.Value = _config.UIScale;
            #endregion

            #endregion
            InitiateFont();
            InitiateUIScaling();
        }
        #endregion

        private bool ToggleFullscreen(bool toFullscreen)
        {
            var screen = Screen.FromControl(this);

            if (toFullscreen)
            {
                WindowState = FormWindowState.Normal;
                FormBorderStyle = FormBorderStyle.None;
                Location = new Point(screen.Bounds.Left, screen.Bounds.Top);
                Width = screen.Bounds.Width;
                Height = screen.Bounds.Height;
            }
            else
            {
                FormBorderStyle = FormBorderStyle.Sizable;
                WindowState = FormWindowState.Normal;
                Width = 1280;
                Height = 720;
                CenterToScreen();
            }

            return true;
        }

        #region General Event Handlers
        private async void frmMain_Shown(object sender, EventArgs e)
        {
            while (_mapCanvas.GRContext is null)
                await Task.Delay(1);

            _mapCanvas.GRContext.SetResourceCacheLimit(1610612736); // Fixes low FPS on big maps

            while (true)
            {
                await Task.Run(() => Thread.SpinWait(25000)); // High performance async delay
                _mapCanvas.Refresh(); // draw next frame
            }
        }

        private void MapChangeTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.BeginInvoke(
                new MethodInvoker(
                    delegate
                    {
                        btnToggleMap.Enabled = false;
                        btnToggleMap.Text = "Loading...";
                    }
                )
            );

            lock (_renderLock)
            {
                try
                {
                    _selectedMap = _maps[_mapSelectionIndex]; // Swap map

                    if (_loadedBitmaps is not null)
                    {
                        foreach (var bitmap in _loadedBitmaps)
                            bitmap?.Dispose(); // Cleanup resources
                    }

                    _loadedBitmaps = new SKBitmap[_selectedMap.ConfigFile.MapLayers.Count];

                    for (int i = 0; i < _loadedBitmaps.Length; i++)
                    {
                        using (
                            var stream = File.Open(
                                _selectedMap.ConfigFile.MapLayers[i].Filename,
                                FileMode.Open,
                                FileAccess.Read))
                        {
                            _loadedBitmaps[i] = SKBitmap.Decode(stream); // Load new bitmap(s)
                            _loadedBitmaps[i].SetImmutable();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        $"ERROR loading {_selectedMap.ConfigFile.MapLayers[0].Filename}: {ex}"
                    );
                }
                finally
                {
                    this.BeginInvoke(
                        new MethodInvoker(
                            delegate
                            {
                                btnToggleMap.Enabled = true;
                                btnToggleMap.Text = "Toggle Map (F5)";
                            }
                        )
                    );
                }
            }
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        { }
        #endregion

        #region Radar Tab
        #region Helper Functions
        private void UpdateWindowTitle()
        {
            bool inGame = this.InGame;
            var localPlayer = this.LocalPlayer;

            if (inGame && localPlayer is not null)
            {
                UpdateSelectedMap();

                if (_fpsWatch.ElapsedMilliseconds >= 1000)
                {
                    // RE-ENABLE & EXPLORE WHAT THIS DOES
                    _mapCanvas.GRContext.PurgeResources(); // Seems to fix mem leak issue on increasing resource cache

                    #region Radar Stats
                    var fps = _fps;
                    var memTicks = Memory.Ticks;

                   // if (lblFPS.Text != fps.ToString())
                   //    lblFPS.Text = $"{fps}";

                  //  if (lblMems.Text != memTicks.ToString())
                   //     lblMems.Text = $"{memTicks}";
                    #endregion

                    _fpsWatch.Restart();
                    _fps = 0;
                }
                else
                {
                    _fps++;
                }
            }
        }

        private void UpdateSelectedMap()
        {
            string currentMap = this.MapName;
            string currentMapPrefix = currentMap.ToLower().Substring(0, Math.Min(6, currentMap.Length));

            if (_selectedMap is null || !_selectedMap.MapID.ToLower().StartsWith(currentMapPrefix))
            {
                var selectedMapName = _maps.FirstOrDefault(x => x.MapID.ToLower().StartsWith(currentMapPrefix) || x.MapID.ToLower() == currentMap.ToLower());

                if (selectedMapName is not null)
                {
                    _selectedMap = selectedMapName;

                    // Init map
                    CleanupLoadedBitmaps();
                    LoadMapBitmaps();
                }
            }
        }

        private void CleanupLoadedBitmaps()
        {
            if (_loadedBitmaps is not null)
            {
                Parallel.ForEach(_loadedBitmaps, bitmap =>
                {
                    bitmap?.Dispose();
                });

                _loadedBitmaps = null;
            }
        }

        private void LoadMapBitmaps()
        {
            var mapLayers = _selectedMap.ConfigFile.MapLayers;
            _loadedBitmaps = new SKBitmap[mapLayers.Count];

            Parallel.ForEach(mapLayers, (mapLayer, _, _) =>
            {
                lock (_loadMapBitmapsLock)
                {
                    using (var stream = File.Open(mapLayer.Filename, FileMode.Open, FileAccess.Read))
                    {
                        _loadedBitmaps[mapLayers.IndexOf(mapLayer)] = SKBitmap.Decode(stream);
                        _loadedBitmaps[mapLayers.IndexOf(mapLayer)].SetImmutable();
                    }
                }
            });
        }

        private bool IsReadyToRender()
        {
            bool isReady = this.Ready;
            bool inGame = this.InGame;
            bool localPlayerExists = this.LocalPlayer is not null;
            bool selectedMapLoaded = this._selectedMap is not null;

            if (!isReady)
                return false; // Game process not running

            if (!inGame)
                return false; // Waiting for game start

            if (!localPlayerExists)
                return false; // Cannot find local player

            if (!selectedMapLoaded)
                return false; // Map not loaded

            return true; // Ready to render
        }

        private int GetMapLayerIndex(float playerHeight)
        {
            for (int i = _loadedBitmaps.Length - 1; i >= 0; i--)
            {
                if (playerHeight > _selectedMap.ConfigFile.MapLayers[i].MinHeight)
                {
                    return i;
                }
            }

            return 0; // Default to the first layer if no match is found
        }

        private MapParameters GetMapParameters(MapPosition localPlayerPos)
        {
            int mapLayerIndex = GetMapLayerIndex(localPlayerPos.Height);

            var bitmap = _loadedBitmaps[mapLayerIndex];
            float zoomFactor = 0.01f * _config.DefaultZoom;
            float zoomWidth = bitmap.Width * zoomFactor;
            float zoomHeight = bitmap.Height * zoomFactor;

            var bounds = new SKRect(
                Math.Max(Math.Min(localPlayerPos.X, bitmap.Width - zoomWidth / 2) - zoomWidth / 2, 0),
                Math.Max(Math.Min(localPlayerPos.Y, bitmap.Height - zoomHeight / 2) - zoomHeight / 2, 0),
                Math.Min(Math.Max(localPlayerPos.X, zoomWidth / 2) + zoomWidth / 2, bitmap.Width),
                Math.Min(Math.Max(localPlayerPos.Y, zoomHeight / 2) + zoomHeight / 2, bitmap.Height)
            ).AspectFill(_mapCanvas.CanvasSize);

            return new MapParameters
            {
                UIScale = _uiScale,
                MapLayerIndex = mapLayerIndex,
                Bounds = bounds,
                XScale = (float)_mapCanvas.Width / bounds.Width, // Set scale for this frame
                YScale = (float)_mapCanvas.Height / bounds.Height // Set scale for this frame
            };
        }

        private MapParameters GetMapLocation()
        {
            var localPlayer = this.LocalPlayer;
            if (localPlayer is not null)
            {
                var localPlayerPos = localPlayer.Position + AbsoluteLocation;
                var localPlayerMapPos = localPlayerPos.ToMapPos(_selectedMap);

                if (_isFreeMapToggled)
                {
                    _mapPanPosition.Height = localPlayerMapPos.Height;
                    return GetMapParameters(_mapPanPosition);
                }
                else
                    return GetMapParameters(localPlayerMapPos);
            }
            else
            {
                return GetMapParameters(_mapPanPosition);
            }
        }

        private void DrawMap(SKCanvas canvas)
        {
            if (grpMapSetup.Visible) // Print coordinates (to make it easy to setup JSON configs)
            {
                var localPlayer = this.LocalPlayer;
                var localPlayerPos = localPlayer.Position + AbsoluteLocation;
                grpMapSetup.Text = $"Map Setup - X,Y,Z: {localPlayerPos.X}, {localPlayerPos.Y}, {localPlayerPos.Z}";
            }
            else if (grpMapSetup.Text != "Map Setup" && !grpMapSetup.Visible)
            {
                grpMapSetup.Text = "Map Setup";
            }

            // Prepare to draw Game Map
            var mapParams = GetMapLocation();

            var mapCanvasBounds = new SKRect() // Drawing Destination
            {
                Left = _mapCanvas.Left,
                Right = _mapCanvas.Right,
                Top = _mapCanvas.Top,
                Bottom = _mapCanvas.Bottom
            };

            // Draw Game Map
            canvas.DrawBitmap(
                _loadedBitmaps[mapParams.MapLayerIndex],
                mapParams.Bounds,
                mapCanvasBounds,
                SKPaints.PaintBitmap
            );
        }

        private void DrawActors(SKCanvas canvas)
        {
            var localPlayer = this.LocalPlayer;

            if (this.InGame && localPlayer is not null)
            {
                var allPlayers = this.AllActors?.Select(x => x.Value);

                if (allPlayers is not null)
                {
                    var localPlayerPos = localPlayer.Position + AbsoluteLocation;
                    var localPlayerMapPos = localPlayerPos.ToMapPos(_selectedMap);
                    var mapParams = GetMapLocation();

                    // Draw LocalPlayer
                    var localPlayerZoomedPos = localPlayerMapPos.ToZoomedPos(mapParams);
                    localPlayerZoomedPos.DrawPlayerMarker(canvas, localPlayer, trkAimLength.Value);

                    foreach (var actor in allPlayers) // Draw actors
                    {
                        var actorPos = actor.Position + AbsoluteLocation;
                        if (actor.ActorType == ActorType.Player && !actor.IsAlive || (Math.Abs(actorPos.X - AbsoluteLocation.X) + Math.Abs(actorPos.Y - AbsoluteLocation.Y) + Math.Abs(actorPos.Z - AbsoluteLocation.Z) < 1.0))
                            continue;

                        var actorMapPos = actorPos.ToMapPos(_selectedMap);
                        var actorZoomedPos = actorMapPos.ToZoomedPos(mapParams);

                        actor.ZoomedPosition = new Vector2() // Cache Position as Vec2 for MouseMove event
                        {
                            X = actorZoomedPos.X,
                            Y = actorZoomedPos.Y
                        };

                        int aimlineLength = 15;

                        // Draw
                        DrawActor(canvas, actor, actorZoomedPos, aimlineLength, localPlayerMapPos);
                    }
                }
            }
        }  

        private void DrawActor(SKCanvas canvas, UActor actor, MapPosition actorZoomedPos, int aimlineLength, MapPosition localPlayerMapPos)
        {
            if (this.InGame && this.LocalPlayer is not null)
            {
                string[] lines = null;
                var height = actorZoomedPos.Height - localPlayerMapPos.Height;

                if (actor.ActorType == ActorType.Player)
                {
                    actorZoomedPos.DrawPlayerMarker(
                        canvas,
                        actor,
                        aimlineLength
                    );
                }
                else if (actor.ActorType == ActorType.Projectile)
                {
                    actorZoomedPos.DrawProjectile(canvas, actor);
                }
                else
                {
                    // Define vehicle types that should show distance
                    var vehicleTypes = new HashSet<ActorType>
                    {
                        ActorType.TruckTransport,
                        ActorType.TruckLogistics,
                        ActorType.TruckAntiAir,
                        ActorType.TruckArtillery,
                        ActorType.TruckTransportArmed,
                        ActorType.JeepTransport,
                        ActorType.JeepLogistics,
                        ActorType.JeepTurret,
                        ActorType.JeepArtillery,
                        ActorType.JeepAntitank,
                        ActorType.JeepRWSTurret,
                        ActorType.APC,
                        ActorType.IFV,
                        ActorType.TrackedAPC,
                        ActorType.TrackedIFV,
                        ActorType.TrackedJeep,
                        ActorType.Tank,
                        ActorType.TankMGS,
                        ActorType.TransportHelicopter,
                        ActorType.AttackHelicopter,
                        ActorType.Boat,
                        ActorType.BoatLogistics,
                        ActorType.Motorcycle,
                        ActorType.AntiAir,
                        ActorType.TrackedLogistics,
                        ActorType.LoachCAS,
                        ActorType.LoachScout
                    };

                    // Only calculate/show distance for vehicles
                    if (vehicleTypes.Contains(actor.ActorType))
                    {
                        var dist = Vector3.Distance(this.LocalPlayer.Position, actor.Position);

                        // Only draw the distance if the vehicle is more than 50 meters away
                        if (dist > 50 * 100)
                        {
                            lines = new string[1] { $"{(int)Math.Round(dist / 100)}m" };

                            if (actor.ErrorCount > 10)
                                lines[0] = "ERROR";

                            actorZoomedPos.DrawActorText(
                                canvas,
                                actor,
                                lines
                            );
                        }
                    }
                    // Draw the vehicle/tech marker
                    actorZoomedPos.DrawTechMarker(canvas, actor);
                }
            }
        }


        private void DrawPOIs(SKCanvas canvas)
        {
            if (!IsReadyToRender()) return;

            var mapParams = GetMapLocation();
            var localPlayerPos = LocalPlayer.Position + AbsoluteLocation;

            using var crosshairPaint = new SKPaint
            {
                Color = SKColors.Red,
                StrokeWidth = 1.5f,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };

            foreach (var poi in _pointsOfInterest)
            {
                var poiMapPos = poi.Position.ToMapPos(_selectedMap);
                var poiZoomedPos = poiMapPos.ToZoomedPos(mapParams);

                var center = poiZoomedPos.GetPoint();
                float crossSize = 8 * _uiScale;

                canvas.DrawLine(
                    center.X - crossSize, center.Y - crossSize,
                    center.X + crossSize, center.Y + crossSize,
                    crosshairPaint);

                canvas.DrawLine(
                    center.X + crossSize, center.Y - crossSize,
                    center.X - crossSize, center.Y + crossSize,
                    crosshairPaint);

                var distance = Vector3.Distance(localPlayerPos, poi.Position);
                float bearing = CalculateBearing(localPlayerPos, poi.Position);
                DrawPOIText(canvas, poiZoomedPos, distance, bearing, crossSize);
            }
        }

        private void DrawPOIText(SKCanvas canvas, MapPosition position, float distance, float bearing, float crosshairSize)
        {
            int distanceMeters = (int)Math.Round(distance / 100);
            string[] lines =
            {
                $"{(int)Math.Round(bearing)}°",
                $"{distanceMeters}m"
            };

            SKPaint textPaint = SKPaints.TextBase;
            SKPaint outlinePaint = SKPaints.TextBaseOutline;

            var basePosition = position.GetPoint(crosshairSize + 6 * _uiScale, 0);
            float verticalSpacing = 12 * _uiScale;

            foreach (var line in lines)
            {
                canvas.DrawText(line, basePosition.X, basePosition.Y, outlinePaint);
                canvas.DrawText(line, basePosition.X, basePosition.Y, textPaint);
                basePosition.Y += verticalSpacing;
            }
        }


        private float CalculateBearing(Vector3 playerPos, Vector3 poiPos)
        {
            float deltaX = poiPos.X - playerPos.X;
            float deltaY = playerPos.Y - poiPos.Y;

            float radians = (float)Math.Atan2(deltaY, deltaX);
            float degrees = radians * (180f / (float)Math.PI);
            degrees = 90f - degrees;

            if (degrees < 0) degrees += 360f;

            return degrees;
        }

        private void DrawToolTips(SKCanvas canvas)
        {
            var localPlayer = this.LocalPlayer;
            var mapParams = GetMapLocation();

            if (localPlayer is not null)
            {
                if (_closestPlayerToMouse is not null)
                {
                    // Calculate the distance between the local player and the hovered player
                    var localPlayerPos = localPlayer.Position + AbsoluteLocation;
                    var hoveredPlayerPos = _closestPlayerToMouse.Position + AbsoluteLocation;
                    var distance = Vector3.Distance(localPlayerPos, hoveredPlayerPos);

                    // Format the distance as a string (e.g., "123m")
                    var distanceText = $"{(int)Math.Round(distance / 100)}m";

                    // Get the zoomed position of the hovered player
                    var playerZoomedPos = (_closestPlayerToMouse
                        .Position + AbsoluteLocation)
                        .ToMapPos(_selectedMap)
                        .ToZoomedPos(mapParams);

                    // Draw the tooltip with the distance included
                    playerZoomedPos.DrawToolTip(canvas, _closestPlayerToMouse, distanceText);
                }
            }
        }

        private void btnAddPOI_Click(object sender, EventArgs e)
        {
            var localPlayer = this.LocalPlayer;
            if (localPlayer is not null)
            {
                var position = localPlayer.Position + AbsoluteLocation;
                AddPointOfInterest(position, "POI 1");

                // Refresh the map canvas to render the new POI
                _mapCanvas.Invalidate();
            }
        }


        private void DrawStatusText(SKCanvas canvas)
        {
            bool isReady = this.Ready;
            bool inGame = this.InGame;
            var localPlayer = this.LocalPlayer;
            var selectedMap = this._selectedMap;

            string statusText;
            if (!isReady)
            {
                statusText = "Game Process Not Running";
            }
            else if (!inGame)
            {
                statusText = "Waiting for Game Start...";

                if (selectedMap is not null)
                {
                    this._selectedMap = null;
                }
            }
            else if (localPlayer is null)
            {
                statusText = "Cannot find LocalPlayer";
            }
            else if (selectedMap is null)
            {
                statusText = "Loading Map";
            }
            else
            {
                return; // No status text to draw
            }

            var centerX = _mapCanvas.Width / 2;
            var centerY = _mapCanvas.Height / 2;

            canvas.DrawText(statusText, centerX, centerY, SKPaints.TextRadarStatus);
        }

        private void ClearPlayerRefs()
        {
            _closestPlayerToMouse = null;
        }

        private T FindClosestObject<T>(IEnumerable<T> objects, Vector2 position, Func<T, Vector2> positionSelector, float threshold)
            where T : class
        {
            if (objects is null || !objects.Any())
                return null;

            var closestObject = objects.Aggregate(
                (x1, x2) =>
                    x2 == null || Vector2.Distance(positionSelector(x1), position)
                    < Vector2.Distance(positionSelector(x2), position)
                        ? x1
                        : x2
            );

            if (closestObject is not null && Vector2.Distance(positionSelector(closestObject), position) < threshold)
                return closestObject;

            return null;
        }

        private void PanTimer_Tick(object sender, EventArgs e)
        {
            var panDifference = new SKPoint(
                this.targetPanPosition.X - this._mapPanPosition.X,
                this.targetPanPosition.Y - this._mapPanPosition.Y
            );

            if (panDifference.Length > 0.1)
            {
                this._mapPanPosition.X += (float)(panDifference.X * PAN_SMOOTHNESS);
                this._mapPanPosition.Y += (float)(panDifference.Y * PAN_SMOOTHNESS);
            }
            else
            {
                this.panTimer.Stop();
            }
        }

        private void ZoomTimer_Tick(object sender, EventArgs e)
        {
            int zoomDifference = this.targetZoomValue - _config.DefaultZoom;

            if (zoomDifference != 0)
            {
                int zoomStep = Math.Sign(zoomDifference);
                _config.DefaultZoom += zoomStep;
            }
            else
            {
                this.zoomTimer.Stop();
            }
        }

        private bool ZoomIn(int amt)
        {
            this.targetZoomValue = Math.Max(10, _config.DefaultZoom - amt);
            this.zoomTimer.Start();

            return true;
        }

        private bool ZoomOut(int amt)
        {
            this.targetZoomValue = Math.Min(200, _config.DefaultZoom + amt);
            this.zoomTimer.Start();

            return false;
        }
        #endregion

        #region Event Handlers
        private void chkMapFree_CheckedChanged(object sender, EventArgs e)
        {
            if (_isFreeMapToggled)
            {
                chkMapFree.Text = "Map Follow";
                _isFreeMapToggled = false;

                lock (_renderLock)
                {
                    var localPlayer = this.LocalPlayer;
                    if (localPlayer is not null)
                    {
                        var localPlayerMapPos = (localPlayer.Position + AbsoluteLocation).ToMapPos(_selectedMap);
                        _mapPanPosition = new MapPosition()
                        {
                            X = localPlayerMapPos.X,
                            Y = localPlayerMapPos.Y,
                            Height = localPlayerMapPos.Height
                        };
                    }
                }
            }
            else
            {
                chkMapFree.Text = "Map Free";
                _isFreeMapToggled = true;
            }
        }

        private void btnApplyMapScale_Click(object sender, EventArgs e)
        {
            if (float.TryParse(txtMapSetupX.Text, out float x)
                && float.TryParse(txtMapSetupY.Text, out float y)
                && float.TryParse(txtMapSetupScale.Text, out float scale))
            {
                lock (_renderLock)
                {
                    if (_selectedMap is not null)
                    {
                        _selectedMap.ConfigFile.X = x;
                        _selectedMap.ConfigFile.Y = y;
                        _selectedMap.ConfigFile.Scale = scale;
                        _selectedMap.ConfigFile.Save(_selectedMap);
                    }
                }
            }
            else
            {
                ShowErrorDialog("Invalid value(s) provided in the map setup textboxes.");
            }
        }

        private void HandleMapClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && InGame)
            {
                var mapParams = GetMapLocation();
                var mouseX = (e.X / mapParams.XScale) + mapParams.Bounds.Left;
                var mouseY = (e.Y / mapParams.YScale) + mapParams.Bounds.Top;

                var worldX = (mouseX - _selectedMap.ConfigFile.X) / _selectedMap.ConfigFile.Scale;
                var worldY = (mouseY - _selectedMap.ConfigFile.Y) / _selectedMap.ConfigFile.Scale;
                var worldPos = new Vector3(worldX, worldY, LocalPlayer.Position.Z);

                _pointsOfInterest.Add(new PointOfInterest(worldPos, "POI"));
                _mapCanvas.Invalidate();
            }
            else if (e.Button == MouseButtons.Right && _hoveredPoi != null)
            {
                _pointsOfInterest.Remove(_hoveredPoi);
                _mapCanvas.Invalidate();
            }
        }

        private float Distance(SKPoint a, SKPoint b)
        {
            return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        public class PointOfInterest
        {
            public Vector3 Position { get; }
            public string Name { get; }

            public PointOfInterest(Vector3 position, string name)
            {
                Position = position;
                Name = name;
            }
        }

        private void skMapCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            // Handle player hover and dragging
            if (this.InGame && this.LocalPlayer is not null)
            {
                var mouse = new Vector2(e.X, e.Y);

                // Find the closest player to the mouse cursor
                var players = this.AllActors?.Select(x => x.Value);
                _closestPlayerToMouse = FindClosestObject(players, mouse, x => x.ZoomedPosition, 12 * _uiScale);
            }
            else if (this.InGame)
            {
                ClearPlayerRefs();
            }

            // Handle map dragging
            if (this._isDragging && this._isFreeMapToggled)
            {
                if (!this._lastMousePosition.IsEmpty)
                {
                    int dx = e.X - this._lastMousePosition.X;
                    int dy = e.Y - this._lastMousePosition.Y;

                    dx = (int)(dx * DRAG_SENSITIVITY);
                    dy = (int)(dy * DRAG_SENSITIVITY);

                    this.targetPanPosition.X -= dx;
                    this.targetPanPosition.Y -= dy;

                    if (!this.panTimer.Enabled)
                        this.panTimer.Start();
                }

                this._lastMousePosition = e.Location;
            }

            // Handle POI hover
            _hoveredPoi = null;
            if (InGame && _pointsOfInterest.Count > 0)
            {
                var mapParams = GetMapLocation();
                var mousePos = new SKPoint(e.X, e.Y);

                foreach (var poi in _pointsOfInterest)
                {
                    var poiPos = poi.Position.ToMapPos(_selectedMap).ToZoomedPos(mapParams).GetPoint();
                    if (SKPoint.Distance(mousePos, poiPos) < 20 * _uiScale) // 20px hover threshold
                    {
                        _hoveredPoi = poi;
                        break; 
                    }
                }
            }

            _mapCanvas.Invalidate();
        }

        private void skMapCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && this._isFreeMapToggled)
            {
                this._isDragging = true;
                this._lastMousePosition = e.Location;
            }

            // Remove POI on right-click (hover-based removal)
            if (e.Button == MouseButtons.Right && _hoveredPoi != null)
            {
                _pointsOfInterest.Remove(_hoveredPoi);
                _hoveredPoi = null; 
                _mapCanvas.Invalidate();
            }
        }

        private void skMapCanvas_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && this.InGame && this.LocalPlayer is not null)
            {
                var mapParams = GetMapLocation();
                var mouseX = e.X / mapParams.XScale + mapParams.Bounds.Left;
                var mouseY = e.Y / mapParams.YScale + mapParams.Bounds.Top;

                var worldX = (mouseX - _selectedMap.ConfigFile.X) / _selectedMap.ConfigFile.Scale;
                var worldY = (mouseY - _selectedMap.ConfigFile.Y) / _selectedMap.ConfigFile.Scale;
                var worldZ = this.LocalPlayer.Position.Z;

                var poiPosition = new Vector3(worldX, worldY, worldZ);

                AddPointOfInterest(poiPosition, "POI");

                _mapCanvas.Invalidate();
            }
        }

        private void skMapCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (this._isDragging)
            {
                this._isDragging = false;
                this._lastMousePosition = e.Location;
            }
        }

        private void skMapCanvas_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            try
            {
                SKCanvas canvas = e.Surface.Canvas;
                canvas.Clear();

                UpdateWindowTitle();

                if (IsReadyToRender())
                {
                    lock (_renderLock)
                    {
                        DrawMap(canvas);
                        DrawActors(canvas);
                        DrawPOIs(canvas);
                        //DrawToolTips(canvas); 
                    }
                }
                else
                {
                    DrawStatusText(canvas);
                }

                canvas.Flush();
            }
            catch { }
        }

        private void btnToggleMap_Click(object sender, EventArgs e)
        {
            ToggleMap();
        }
        #endregion
        #endregion

        #region Settings
        #region General
        #region Event Handlers
        private void chkShowMapSetup_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowMapSetup.Checked)
            {
                grpMapSetup.Visible = true;
                txtMapSetupX.Text = _selectedMap?.ConfigFile.X.ToString() ?? "0";
                txtMapSetupY.Text = _selectedMap?.ConfigFile.Y.ToString() ?? "0";
                txtMapSetupScale.Text = _selectedMap?.ConfigFile.Scale.ToString() ?? "0";
            }
            else
                grpMapSetup.Visible = false;
        }

        private void btnRestartRadar_Click(object sender, EventArgs e)
        {
            Memory.Restart();
        }

        private void trkZoomSensivity_Scroll(object sender, EventArgs e)
        {
            _config.ZoomSensitivity = trkZoomSensivity.Value;
        }

        private void trkUIScale_Scroll(object sender, EventArgs e)
        {
            _config.UIScale = trkUIScale.Value;
            _uiScale = (.01f * trkUIScale.Value);

            InitiateUIScaling();
        }
        #endregion
        #endregion

        #region Colors
        #region Helper Functions

        private Color PaintColorToColor(string name)
        {
            PaintColor.Colors color = _config.PaintColors[name];
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        private Color DefaultPaintColorToColor(string name)
        {
            PaintColor.Colors color = _config.DefaultPaintColors[name];
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        private void UpdatePaintColorByName(string name, PictureBox pictureBox)
        {
            if (colDialog.ShowDialog() == DialogResult.OK)
            {
                Color col = colDialog.Color;
                pictureBox.BackColor = col;

                var paintColorToUse = new PaintColor.Colors
                {
                    A = col.A,
                    R = col.R,
                    G = col.G,
                    B = col.B
                };

                if (_config.PaintColors.ContainsKey(name))
                {
                    _config.PaintColors[name] = paintColorToUse;
                }
                else
                {
                    _config.PaintColors.Add(name, paintColorToUse);
                }
            }
        }
        #endregion
        #endregion
        #endregion
        #endregion
    }
}