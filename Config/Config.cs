using System.Text.Json;

namespace BackpUk
{
    public static class Config
    {
        const string AppName = "BackpUk";
        const string UserPreferencesFileName = "settings.json";
        static string LocalAppData { get; } = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppName);
        public static string TrackerFileLocation { get; } = Path.Join(LocalAppData, "track_list");


        static FileInfo _userPreferencesFile { get; } = new(Path.Join(LocalAppData, UserPreferencesFileName));
        public static UserPreferences UserPreferences { get; set; } = _userPreferences ??= Load();
        static UserPreferences? _userPreferences;


        static readonly JsonSerializerOptions serializerOptions = new()
        {
            IgnoreReadOnlyProperties = true,
            WriteIndented = true,
        };
        public static UserPreferences Load()
        {
            using var preferenceReader = _userPreferencesFile.OpenRead();
            return JsonSerializer.Deserialize<UserPreferences>(preferenceReader, serializerOptions)!;
        }

        public static void Save()
        {
            using var preferenceWriter = _userPreferencesFile.OpenWrite();
            JsonSerializer.Serialize(preferenceWriter, UserPreferences, serializerOptions);
            _userPreferences = null;
        }
    }
}