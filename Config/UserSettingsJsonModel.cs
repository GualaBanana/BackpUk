namespace BackpUk
{

    public class UserSettingsJsonModel
    {
        public UserSettingsJsonModel(string pathToBackpUkDirectory) => PathToBackpUkDirectory = pathToBackpUkDirectory;
        public string BackpUkDirectoryName { get; private set; } = "$backpuk";
        // This save of changing the property and consequently the field in json file needs to be done in the UserSettingsManager class and not here.
        // So UserSettingsManager class needs to wrap Config.Save() and it actually needs to be ITS responsibility to make the call to it.
        // Beacause if this is done directly in this property, the setter throws an exception caused by multiple sources trying to access
        // .json file when Config.UserPreferences is being initialized, as it calls Config.Load() which changes this PathToBackpUk property
        // which in turn tries to call Config.Save() as its state is being changed due to loading values from .json and assigning them to this property.

        // Config.Save();  // Saves changes to the persistent storage.
        public string? PathToBackpUkDirectory { get; private set; }
        public string BackpUkLocation => Path.Join(PathToBackpUkDirectory, BackpUkDirectoryName);
    }
}
