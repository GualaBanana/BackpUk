namespace SyncTool
{
    public class Cloud : IRelativePathManager
    {
        CloudConfig _config { get; } = new();
        StorageSpaceTracker _storageSpaceTracker { get; }

        public List<string> RelativeFileNames => 
            Directory.EnumerateFiles(_config.ComponentLocationPath, "*", SearchOption.AllDirectories)
            .Select(file => RelativeName(file))
            .ToList();
        public string FullNameFromRelative(string relativePath) => Path.Join(_config.ComponentLocationPath, relativePath);
        public string RelativeName(string fullPath) => Path.GetRelativePath(_config.ComponentLocationPath, fullPath);


        public Cloud()
        {
            Directory.CreateDirectory(_config.ComponentLocationPath);
            _storageSpaceTracker = new(_config.ComponentLocationPath);
        }
    }
}
