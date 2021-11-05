using System.Text.Json;

namespace BackpUk
{
    // This is the class that will be also responsible for changing the settings (or maybe actually I'll still need to create a separate class for this.
    public class UserSettingsManager
    {
        // Just a Singleton implementation.
        UserSettingsManager() { }
        static UserSettingsManager? _instance;
        public static UserSettingsManager Singleton => _instance ??= new();

        public UserSettingsJsonModel SettingsModel;
        // This call to the FileInfo constructor doesn't throw because Config.userSEttingsFileLocation always exist (it's created on the installation).
        static FileInfo _userSettingsFile { get; } = new(Config.UserSettingsFileLocation);


        static readonly JsonSerializerOptions serializerOptions = new()
        {
            IgnoreReadOnlyProperties = true,
            WriteIndented = true,
        };
        /// <summary>
        /// Fetches data from the setting file and assigns it to <see cref="SettingsModel"/> instance property.
        /// </summary>
        public void Load()
        {
            using var preferenceReader = _userSettingsFile.OpenRead();
            SettingsModel = JsonSerializer.Deserialize<UserSettingsJsonModel>(preferenceReader, serializerOptions)!;
        }

        public void Save()
        {
            using var preferenceWriter = _userSettingsFile.OpenWrite();
            JsonSerializer.Serialize(preferenceWriter, SettingsModel, serializerOptions);
            SettingsModel = null;    // Not sure if it's still relevant.
        }
    }

    /// <summary>
    /// Uncoupled model of the user preferences fetched from the file.
    /// </summary>
    public class UserSettingsJsonModel
    {
        internal UserSettingsJsonModel() { }
        string BackpUkDirectoryName { get; } = "$backpuk";
        public string? PathToBackpUkDirectory
        {
            get => pathToBackpUkDirectory;
            internal set
            {
                pathToBackpUkDirectory = value;
                // This save of changing the property and consequently the field in json file needs to be done in the Settings class and not here.
                // So Settings class needs to wrap Config.Save() and it actually needs to be ITS responsibility to make a call to Config.Save().
                // Beacause if this is done directly in this property, the setter throws an exception caused by multiple sources trying to access
                // .json file when Config.UserPreferences is being initialized, as it calls Config.Load() which changes this PathToBackpUk property
                // which in turn tries to call Config.Save() as its state is being changed due to loading values from .json and assigning them to this property.

                // Config.Save();  // Saves changes to the persistent storage.
            }
        }
        string? pathToBackpUkDirectory;
        // Made it internal for now as the user doesn't have an option to change this setting, yet. Likely, will be changed later.
        internal string BackpUkLocation => Path.Join(PathToBackpUkDirectory, BackpUkDirectoryName);
    }
}
