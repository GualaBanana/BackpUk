using System.Text.Json;

namespace BackpUk
{
    // This is the class that will be also responsible for changing the settings (or maybe actually I'll still need to create a separate class for this.
    public class UserSettingsManager
    {
        UserSettingsManager(string backpUkPath) { UserSettings = new(backpUkPath); }
        static UserSettingsManager? _instance;
        public static UserSettingsManager Singleton => _instance ??= new(BackpUkPath!);
        public static string? BackpUkPath;


        public UserSettingsJsonModel? UserSettings { get; private set; }
        // This call to the FileInfo constructor doesn't throw because Config.userSEttingsFileLocation always exist (it's created on the installation).
        static FileInfo _userSettingsFile { get; } = new(Config.UserSettingsFileLocation);


        static readonly JsonSerializerOptions serializerOptions = new()
        {
            WriteIndented = true,
        };
        /// <summary>
        /// Fetches data from the setting file and assigns it to <see cref="UserSettings"/> instance property.
        /// </summary>
        public void Load()
        {
            using var preferenceReader = _userSettingsFile.OpenRead();
            UserSettings = JsonSerializer.Deserialize<UserSettingsJsonModel>(preferenceReader, serializerOptions)!;
        }
        /// <summary>
        /// Saves <see cref="UserSettings">UserSettings</see> to the file storing user's settings.
        /// </summary>
        public void Save()
        {
            using var settingsWriter = _userSettingsFile.OpenWrite();
            JsonSerializer.Serialize(settingsWriter, UserSettings, serializerOptions);
        }
    }
}
