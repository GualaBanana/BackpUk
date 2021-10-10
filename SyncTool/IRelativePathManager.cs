namespace SyncTool
{
    public interface IRelativePathManager
    {
        string FullPathFromRelative(string relativePath);
        string RelativeName(string fullPath);
        List<string> RelativeFileNames { get; }
    }
}
