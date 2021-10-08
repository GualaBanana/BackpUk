namespace SyncTool
{
    public static class UsagePolicy
    {
        static public void MustBeTracked(DirectoryInfo directory, DirectoriesTracker tracker)
        {
            if (!tracker.TrackList.Contains(directory.FullName))
                throw new ArgumentException("The directory needs to be currently tracked to be removed.");
        }
        static public void MustBeNotTracked(DirectoryInfo directory, DirectoriesTracker tracker)
        {
            if (tracker.TrackList.Contains(directory.FullName))
                throw new ArgumentException("Directories that are already tracked can not be added twice. Provide only validated elements");
        }
        static public void MustExist(DirectoryInfo directory)
        {
            if (!directory.Exists) 
                throw new DirectoryNotFoundException($"The directory that doesn't exist must not be passed to this method: {directory}");
        }
    }
}
