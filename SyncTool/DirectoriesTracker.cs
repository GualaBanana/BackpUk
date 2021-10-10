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

        public void Add(string directory)
        {
            var subDirectories = Directory.EnumerateDirectories(directory, "*", SearchOption.AllDirectories);

            using var writer = new StreamWriter(_config.ComponentLocationPath, append: true);
            writer.WriteLine(directory);
            foreach (var subDirectory in subDirectories) writer.WriteLine(subDirectory);
        }
        /// <summary>
        /// Removes provided directory from the list of tracked directories.
        /// </summary>
        /// <remarks>
        /// By default, removes only <paramref name="directory"/>, this behaviour can be changed with <paramref name="recursively"/> flag.<br/>
        /// Calling with <paramref name="directory"/> that is not tracked (shouldn't be done) already does nothing.
        /// </remarks>
        /// <param name="directory">the directory to remove from list of tracked directories.</param>
        /// <param name="recursively">if <c>true</c> than removes children directories of the <paramref name="directory"/>, <c>false</c>otherwise.</param>
        public void Remove(string directory)
        {
            var modifiedTrackList = TrackList;
            modifiedTrackList.Remove(directory);

            var subDirectories = Directory.EnumerateDirectories(directory, "*", SearchOption.AllDirectories);
            foreach (var subDirectory in subDirectories) modifiedTrackList.Remove(subDirectory);

            using var writer = new StreamWriter(_config.ComponentLocationPath);
            modifiedTrackList.ForEach(dir => writer.WriteLine(dir));
        }
    }
}
