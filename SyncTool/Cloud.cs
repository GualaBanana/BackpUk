namespace SyncTool
{
    public class Cloud : IRelativePathManager
    {
        readonly CloudConfig _config = new();
        readonly DirectoryInfo _cloud;

        public List<string> RelativeFileNames => _cloud.EnumerateFiles("*", SearchOption.AllDirectories)
            .Select(file => RelativeName(file.FullName))
            .ToList();
        public string FullPathFromRelative(string relativePath) => Path.Join(_config.ComponentLocationPath, relativePath);
        public string RelativeName(string fullPath) => Path.GetRelativePath(_config.ComponentLocationPath, fullPath);


        public Cloud()
        {
            _cloud = _config.FileSystemInfoObject;
            _cloud.Create();    // Does nothing if the directory at the given path already exists.
        }
    }
}
