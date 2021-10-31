namespace BackpUk
{
    public class BackpUk : IRelativePathManager
    {
        StorageSpaceTracker _storageSpaceTracker { get; }

        public List<string> RelativeFileNames => 
            Directory.EnumerateFiles(Config.BackpUkLocation, "*", SearchOption.AllDirectories)
            .Select(file => RelativeName(file))
            .ToList();
        public string FullNameFromRelative(string relativePath) => Path.Join(Config.BackpUkLocation, relativePath);
        public string RelativeName(string fullPath) => Path.GetRelativePath(Config.BackpUkLocation, fullPath);


        public BackpUk()
        {
            Directory.CreateDirectory(Config.BackpUkLocation);
            _storageSpaceTracker = new(Config.BackpUkLocation);
        }
    }
}
