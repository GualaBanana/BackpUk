namespace SyncTool
{
    public interface IRelativePathManager
    {
        string FullNameFromRelative(string relativePath);
        string RelativeName(string fullPath);
        List<string> RelativeFileNames { get; }
    }
}
