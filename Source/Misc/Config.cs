using System.Text.Json.Serialization;
using System.Text.Json;

namespace squad_dma
{
    /// <summary>
    /// Configuration class for managing application settings and feature states.
    /// Handles loading and saving settings to disk in JSON format.
    /// </summary>
    public class Config
    {
        #region UI Settings
        [JsonPropertyName("defaultZoom")]
        public int DefaultZoom { get; set; } = 100;

        [JsonPropertyName("enemyCount")]
        public bool EnemyCount { get; set; } = false;

        [JsonPropertyName("font")]
        public int Font { get; set; } = 0;

        [JsonPropertyName("fontSize")]
        public int FontSize { get; set; } = 13;

        [JsonPropertyName("playerAimLine")]
        public int PlayerAimLineLength { get; set; } = 1000;

        [JsonPropertyName("uiScale")]
        public int UIScale { get; set; } = 100;

        [JsonPropertyName("techMarkerScale")]
        public int TechMarkerScale { get; set; } = 100;

        [JsonPropertyName("vsync")]
        public bool VSync { get; set; } = false;

        [JsonPropertyName("showEnemyDistance")]
        public bool ShowEnemyDistance { get; set; } = true;
        #endregion

        #region Zoom Settings
        [JsonPropertyName("zoomInKey")]
        public Keys ZoomInKey { get; set; } = Keys.Up;

        [JsonPropertyName("zoomOutKey")]
        public Keys ZoomOutKey { get; set; } = Keys.Down;

        [JsonPropertyName("zoomStep")]
        public int ZoomStep { get; set; } = 1;
        #endregion

        #region Feature States
        [JsonPropertyName("disableSuppression")]
        public bool DisableSuppression { get; set; } = false;

        [JsonPropertyName("setInteractionDistances")]
        public bool SetInteractionDistances { get; set; } = false;

        [JsonPropertyName("allowShootingInMainBase")]
        public bool AllowShootingInMainBase { get; set; } = false;

        [JsonPropertyName("setSpeedHack")]
        public bool SetSpeedHack { get; set; } = false;

        [JsonPropertyName("setAirStuck")]
        public bool SetAirStuck { get; set; } = false;

        [JsonPropertyName("disableCollision")]
        public bool DisableCollision { get; set; } = false;

        [JsonPropertyName("quickZoom")]
        public bool QuickZoom { get; set; } = false;

        [JsonPropertyName("rapidFire")]
        public bool RapidFire { get; set; } = false;

        [JsonPropertyName("infiniteAmmo")]
        public bool InfiniteAmmo { get; set; } = false;

        [JsonPropertyName("quickSwap")]
        public bool QuickSwap { get; set; } = false;

        [JsonPropertyName("forceFullAuto")]
        public bool ForceFullAuto { get; set; } = false;
        #endregion

        #region Feature Cache
        [JsonPropertyName("originalFov")]
        public float OriginalFov { get; set; } = 0.0f;

        [JsonPropertyName("originalSuppressionPercentage")]
        public float OriginalSuppressionPercentage { get; set; } = 0.0f;

        [JsonPropertyName("originalMaxSuppression")]
        public float OriginalMaxSuppression { get; set; } = -1.0f;

        [JsonPropertyName("originalSuppressionMultiplier")]
        public float OriginalSuppressionMultiplier { get; set; } = 1.0f;

        [JsonPropertyName("originalCameraRecoil")]
        public bool OriginalCameraRecoil { get; set; } = true;

        [JsonPropertyName("originalTimeBetweenShots")]
        public float OriginalTimeBetweenShots { get; set; } = 0.0f;

        [JsonPropertyName("originalTimeBetweenSingleShots")]
        public float OriginalTimeBetweenSingleShots { get; set; } = 0.0f;

        [JsonPropertyName("originalVehicleTimeBetweenShots")]
        public float OriginalVehicleTimeBetweenShots { get; set; } = 0.0f;

        [JsonPropertyName("originalVehicleTimeBetweenSingleShots")]
        public float OriginalVehicleTimeBetweenSingleShots { get; set; } = 0.0f;

        [JsonPropertyName("originalMovementMode")]
        public byte OriginalMovementMode { get; set; } = 1; // MOVE_Walking

        [JsonPropertyName("originalReplicatedMovementMode")]
        public byte OriginalReplicatedMovementMode { get; set; } = 1; // MOVE_Walking

        [JsonPropertyName("originalReplicateMovement")]
        public byte OriginalReplicateMovement { get; set; } = 16;

        [JsonPropertyName("originalMaxFlySpeed")]
        public float OriginalMaxFlySpeed { get; set; } = 200.0f;

        [JsonPropertyName("originalMaxCustomMovementSpeed")]
        public float OriginalMaxCustomMovementSpeed { get; set; } = 600.0f;

        [JsonPropertyName("originalMaxAcceleration")]
        public float OriginalMaxAcceleration { get; set; } = 500.0f;

        [JsonPropertyName("originalCollisionEnabled")]
        public byte OriginalCollisionEnabled { get; set; } = 1; // QueryOnly

        [JsonPropertyName("originalFireModes")]
        public int[] OriginalFireModes { get; set; } = null;

        [JsonPropertyName("originalManualBolt")]
        public bool OriginalManualBolt { get; set; } = false;

        /// <summary>
        /// Clears all cached feature values to ensure a clean state on game/app restart.
        /// This should be called when the game closes or the application is terminated.
        /// </summary>
        public void ClearFeatureCaches()
        {
            OriginalFov = 0.0f;
            OriginalSuppressionPercentage = 0.0f;
            OriginalMaxSuppression = -1.0f;
            OriginalSuppressionMultiplier = 1.0f;
            OriginalCameraRecoil = true;
            OriginalTimeBetweenShots = 0.0f;
            OriginalTimeBetweenSingleShots = 0.0f;
            OriginalVehicleTimeBetweenShots = 0.0f;
            OriginalVehicleTimeBetweenSingleShots = 0.0f;
            OriginalMovementMode = 1;
            OriginalReplicatedMovementMode = 1;
            OriginalReplicateMovement = 16;
            OriginalMaxFlySpeed = 200.0f;
            OriginalMaxCustomMovementSpeed = 600.0f;
            OriginalMaxAcceleration = 500.0f;
            OriginalCollisionEnabled = 1;
            OriginalFireModes = null;
            OriginalManualBolt = false;
            // Add other cache clearing here as we implement more features
        }
        #endregion

        #region Keybinds
        [JsonPropertyName("keybindSpeedHack")]
        public Keys KeybindSpeedHack { get; set; } = Keys.None;

        [JsonPropertyName("keybindAirStuck")]
        public Keys KeybindAirStuck { get; set; } = Keys.None;

        [JsonPropertyName("keybindQuickZoom")]
        public Keys KeybindQuickZoom { get; set; } = Keys.None;

        [JsonPropertyName("keybindToggleEnemyDistance")]
        public Keys KeybindToggleEnemyDistance { get; set; } = Keys.None;

        [JsonPropertyName("keybindToggleMap")]
        public Keys KeybindToggleMap { get; set; } = Keys.None;

        [JsonPropertyName("keybindToggleFullscreen")]
        public Keys KeybindToggleFullscreen { get; set; } = Keys.None;

        [JsonPropertyName("keybindDumpNames")]
        public Keys KeybindDumpNames { get; set; } = Keys.None;

        [JsonPropertyName("keybindZoomIn")]
        public Keys KeybindZoomIn { get; set; } = Keys.Up;

        [JsonPropertyName("keybindZoomOut")]
        public Keys KeybindZoomOut { get; set; } = Keys.Down;
        #endregion

        #region Private Fields
        [JsonIgnore]
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            Converters = { new JsonKeyEnumConverter() }
        };

        [JsonIgnore]
        private static readonly object _lock = new();

        [JsonIgnore]
        private const string SettingsDirectory = "Configuration";
        #endregion

        #region Public Methods
        /// <summary>
        /// Attempts to load the configuration from disk.
        /// </summary>
        /// <param name="config">The loaded configuration if successful, or a new instance if not.</param>
        /// <returns>True if the configuration was successfully loaded, false otherwise.</returns>
        public static bool TryLoadConfig(out Config config)
        {
            lock (_lock)
            {
                try
                {
                    Directory.CreateDirectory(SettingsDirectory);
                    var path = Path.Combine(SettingsDirectory, "Settings.json");

                    if (!File.Exists(path))
                    {
                        config = new Config();
                        SaveConfig(config);
                        return true;
                    }

                    config = JsonSerializer.Deserialize<Config>(File.ReadAllText(path), _jsonOptions);
                    return true;
                }
                catch
                {
                    config = new Config();
                    return false;
                }
            }
        }

        /// <summary>
        /// Saves the configuration to disk.
        /// </summary>
        /// <param name="config">The configuration to save.</param>
        public static void SaveConfig(Config config)
        {
            lock (_lock)
            {
                Directory.CreateDirectory(SettingsDirectory);
                File.WriteAllText(
                    Path.Combine(SettingsDirectory, "Settings.json"),
                    JsonSerializer.Serialize(config, _jsonOptions)
                );
            }
        }

        /// <summary>
        /// Clears all feature caches and saves the config.
        /// This should be called when the game closes or the application is terminated.
        /// </summary>
        public static void ClearAndSaveCaches()
        {
            if (TryLoadConfig(out var config))
            {
                config.ClearFeatureCaches();
                SaveConfig(config);
            }
        }
        #endregion
    }

    /// <summary>
    /// JSON converter for the Keys enum to ensure proper serialization.
    /// </summary>
    public class JsonKeyEnumConverter : JsonConverter<Keys>
    {
        /// <summary>
        /// Reads a Keys enum value from JSON.
        /// </summary>
        public override Keys Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => (Keys)reader.GetInt32();

        /// <summary>
        /// Writes a Keys enum value to JSON.
        /// </summary>
        public override void Write(Utf8JsonWriter writer, Keys value, JsonSerializerOptions options)
            => writer.WriteNumberValue((int)value);
    }
}