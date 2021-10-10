namespace SyncTool
{
    /// <summary>
    /// Manages the file in which the list of directories' names are stored.
    /// </summary>
    public class DirectoriesTracker : IRelativePathManager
    {
        readonly TrackerConfig _config = new();

        public string FullPathFromRelative(string relativePath) => Path.Join(RootDirectoryToTrack, relativePath);
        public string RelativeName(string fullPath) => Path.GetRelativePath(RootDirectoryToTrack, fullPath);

        public string RootDirectoryToTrack { get; } = @"D:\";  // Config.AskToChooseLogicalDrive();
        public List<string> TrackList
        {
            get
            {
                using var reader = new StreamReader(_config.ComponentLocationPath);
                var trackListFileContents = new List<string>();
                string? line;
                while ((line = reader.ReadLine()) != null && line != String.Empty) trackListFileContents.Add(line);
                return trackListFileContents;
            }
        }
        // TODO:
        // Add the event for the empty `TrackList` that is raised when no directories are tracked yet.
        // Users will be able to declare their own subscribers that will execute only when they are subscribed to my event.
        // E.g. when a gallery app is opened on a phone, the gallery can subscribe to my event to perform
        // actions when my event is raised. This functionality is the responsiblity of the first if statement in this method for now.
        // This observer interface just must be an interface that will be called something like "IEmptyTrackListManager
        public void Add(string directory)
        {
            //if (!TrackList.Any())
            //    throw new NotImplementedException("Implement the Subscriber pattern and " +
            //        "declare 'I<something_related_to_managing_empty_track_lists_or_just_empty_lists_in_general>'");

            var topLevelSubdirectories = Directory.EnumerateDirectories(directory);

            using (var writer = new StreamWriter(_config.ComponentLocationPath, append: true))
            {
                writer.WriteLine(directory);
            }
            foreach (var dir in topLevelSubdirectories) Add(dir);
        }
        // At this moment implementation of `Add` and `Remove` differ. `Add` adds folders recursively
        // calling itself, but `Remove` can do the same but using a flag. So far it doesn't complicate anything.
        /// <summary>
        /// Removes provided directory from the list of tracked directories.
        /// </summary>
        /// <remarks>
        /// By default, removes only <paramref name="directory"/>, this behaviour can be changed with <paramref name="recursively"/> flag.<br/>
        /// Calling with <paramref name="directory"/> that is not tracked (shouldn't be done) already does nothing.
        /// </remarks>
        /// <param name="directory">the directory to remove from list of tracked directories.</param>
        /// <param name="recursively">if <c>true</c> than removes children directories of the <paramref name="directory"/>, <c>false</c>otherwise.</param>
        public void Remove(string directory, bool recursively = false)
        {
            var modifiedDirectoriesList = TrackList;
            modifiedDirectoriesList.Remove(directory);
            if (recursively)
            {
                var allSubdirectories = Directory.EnumerateDirectories(directory, "*", SearchOption.AllDirectories);
                foreach (var dir in allSubdirectories)
                {
                    modifiedDirectoriesList.Remove(dir);
                }
            }
            using var writer = new StreamWriter(_config.ComponentLocationPath);
            modifiedDirectoriesList.ForEach(dir => writer.WriteLine(dir));
        }

        [Obsolete("Instantiates StreamWriter on each call so is not efficient in loops, thus not used, yet.")]
        private void WriteToTrackingFile(string content, bool append)
        {
            using var writer = new StreamWriter(_config.ComponentLocationPath, append);
            writer.WriteLine(content);
        }
    }
}
