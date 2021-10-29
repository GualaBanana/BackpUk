namespace SyncTool
{
    public static class UsagePolicy
    {
        public static void MustBeTracked(string directory, Tracker tracker)
        {
            if (!tracker.TrackList.Contains(directory))
                throw new ArgumentException("The directory needs to be currently tracked to be removed.", nameof(directory));
        }
        public static void MustBeNotTracked(string directory, Tracker tracker)
        {
            if (tracker.TrackList.Contains(directory))
                throw new ArgumentException("Directories that are already tracked can not be added twice. Provide only validated elements", nameof(directory));
        }
        public static void MustExist(string directory)
        {
            if (!Directory.Exists(directory)) 
                throw new DirectoryNotFoundException($"The directory that doesn't exist must not be passed to this method: {directory}");
        }
        public static void MustBeOnRightDrive(string directory)
        {
            if (Directory.GetDirectoryRoot(directory) != Tracker.RootDirectoryToTrack)
                throw new ArgumentException($"The provided directory must be on the tracked drive - {Tracker.RootDirectoryToTrack}.", nameof(directory));
        }
    }
}
