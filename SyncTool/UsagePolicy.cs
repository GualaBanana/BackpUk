namespace SyncTool
{
    public static class UsagePolicy
    {
        static public void MustBeTracked(string directory, Tracker tracker)
        {
            if (!tracker.TrackList.Contains(directory))
                throw new ArgumentException("The directory needs to be currently tracked to be removed.");
        }
        static public void MustBeNotTracked(string directory, Tracker tracker)
        {
            if (tracker.TrackList.Contains(directory))
                throw new ArgumentException("Directories that are already tracked can not be added twice. Provide only validated elements");
        }
        static public void MustExist(string directory)
        {
            if (!Directory.Exists(directory)) 
                throw new DirectoryNotFoundException($"The directory that doesn't exist must not be passed to this method: {directory}");
        }
    }
}
