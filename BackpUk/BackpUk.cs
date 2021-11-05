namespace BackpUk
{
    public class BackpUk : IRelativePathManager
    {
        StorageSpaceTracker _storageSpaceTracker { get; }

        public List<string> RelativeFileNames => 
            Directory.EnumerateFiles(Config.UserPreferences.BackpUkLocation, "*", SearchOption.AllDirectories)
            .Select(file => RelativeName(file))
            .ToList();
        public string FullNameFromRelative(string relativePath) => Path.Join(Config.UserPreferences.BackpUkLocation, relativePath);
        public string RelativeName(string fullPath) => Path.GetRelativePath(Config.UserPreferences.BackpUkLocation, fullPath);


        public BackpUk()
        {
            Directory.CreateDirectory(Config.UserPreferences.BackpUkLocation);
            _storageSpaceTracker = new(Config.UserPreferences.BackpUkLocation);
        }
    }
}
