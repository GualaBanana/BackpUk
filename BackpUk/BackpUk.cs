namespace BackpUk
{
    public class BackpUk : IRelativePathManager
    {
        StorageSpaceTracker _storageSpaceTracker { get; }

        public List<string> RelativeFileNames => 
            Directory.EnumerateFiles(UserSettingsManager.Singleton.UserSettings.BackpUkLocation, "*", SearchOption.AllDirectories)
            .Select(file => RelativeName(file))
            .ToList();
        public string FullNameFromRelative(string relativePath) => Path.Join(UserSettingsManager.Singleton.UserSettings.BackpUkLocation, relativePath);
        public string RelativeName(string fullPath) => Path.GetRelativePath(UserSettingsManager.Singleton.UserSettings.BackpUkLocation, fullPath);


        public BackpUk()
        {
            Directory.CreateDirectory(UserSettingsManager.Singleton.UserSettings.BackpUkLocation);
            _storageSpaceTracker = new(UserSettingsManager.Singleton.UserSettings.BackpUkLocation);
        }
    }
}
