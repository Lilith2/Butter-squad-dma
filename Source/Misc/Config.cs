using System.Text.Json.Serialization;
using System.Text.Json;

namespace squad_dma
{
    public class Config
    {
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

        [JsonPropertyName("zoomInKey")]
        public Keys ZoomInKey { get; set; } = Keys.Up;

        [JsonPropertyName("zoomOutKey")]
        public Keys ZoomOutKey { get; set; } = Keys.Down;

        [JsonPropertyName("zoomStep")]
        public int ZoomStep { get; set; } = 1;

        [JsonPropertyName("vsync")]
        public bool VSync { get; set; } = false;

        [JsonPropertyName("showEnemyDistance")]
        public bool ShowEnemyDistance { get; set; } = true;

        // Local Soldier Features
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

        [JsonPropertyName("setHideActor")]
        public bool SetHideActor { get; set; } = false;

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

        [JsonPropertyName("keybindSpeedHack")]
        public Keys KeybindSpeedHack { get; set; } = Keys.None;

        [JsonPropertyName("keybindAirStuck")]
        public Keys KeybindAirStuck { get; set; } = Keys.None;

        [JsonPropertyName("keybindHideActor")]
        public Keys KeybindHideActor { get; set; } = Keys.None;

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
    }

    public class JsonKeyEnumConverter : JsonConverter<Keys>
    {
        public override Keys Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => (Keys)reader.GetInt32();

        public override void Write(Utf8JsonWriter writer, Keys value, JsonSerializerOptions options)
            => writer.WriteNumberValue((int)value);
    }
}