using System.Text.Json;

namespace BackpUk
{
    // This is the class that will be also responsible for changing the settings (or maybe actually I'll still need to create a separate class for this.
    public class UserSettingsManager
    {
        UserSettingsManager(string backpUkPath) { UserSettings = new(backpUkPath); }
        static UserSettingsManager? _instance;
        public static UserSettingsManager Singleton
        {
            get
            {
                if (IsFirstLaunch && PathToBackpUkDirectory is null) throw new ArgumentNullException(nameof(PathToBackpUkDirectory), "BackpUk storage must be set before using the app.");
                return _instance ??= new(PathToBackpUkDirectory!);
                // Null-forgiving operator is used because Singleton is not called unless null check was passed in BackpUker constructor.
            }
        }
        /// <summary>
        /// Has to be set to any valid path when running the app for the first time.
        /// </summary>
        static string? PathToBackpUkDirectory { get; set; }
        public static bool IsFirstLaunch => File.ReadAllBytes(Config.UserSettingsFileLocation).Length == 0;

        public static void RunSetUp()
        {
            if (IsFirstLaunch)
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("Provide the directory where you would like to store your BackpUk:");
                    PathToBackpUkDirectory = Console.ReadLine() ?? string.Empty;
                    if (!Path.IsPathFullyQualified(PathToBackpUkDirectory) && !Environment.GetLogicalDrives().Contains(Path.GetPathRoot(PathToBackpUkDirectory))) break;
                }
                Singleton.Save();
            }

            Singleton.Load();
            Console.Clear();
        }


        public UserSettingsJsonModel UserSettings { get; private set; }
        // This call to the FileInfo constructor doesn't throw because Config.userSettingsFileLocation always exists (it's created on the installation).
        static FileInfo _userSettingsFile { get; } = new(Config.UserSettingsFileLocation);


        static readonly JsonSerializerOptions indented = new() { WriteIndented = true };
        /// <summary>
        /// Fetches data from the setting file and assigns it to <see cref="UserSettings"/> instance property.
        /// </summary>
        public void Load()
        {
            using var settingsReader = _userSettingsFile.OpenRead();
            UserSettings = JsonSerializer.Deserialize<UserSettingsJsonModel>(settingsReader, indented)!;
        }
        /// <summary>
        /// Saves <see cref="UserSettings">UserSettings</see> to the file storing user's settings.
        /// </summary>
        public void Save()
        {
            using var settingsWriter = _userSettingsFile.OpenWrite();
            JsonSerializer.Serialize(settingsWriter, UserSettings, indented);
        }
    }
}
