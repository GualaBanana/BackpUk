namespace SyncTool
{
    public interface ISystemComponent
    {
        string FullPathFromRelative(string relativePath);
        string RelativeName(string fullPath);
    }
}
