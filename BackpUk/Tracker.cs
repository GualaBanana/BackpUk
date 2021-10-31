namespace SyncTool
{
    /// <summary>
    /// Manages the file in which the list of directories' names are stored.
    /// </summary>
    public class Tracker : IRelativePathManager
    {
        TrackerConfig _config { get; } = new();

        public static string RootDirectoryToTrack { get; } = @"D:\";  // Config.AskToChooseLogicalDrive();
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
        /// <summary>
        /// List of paths of new nested directories inside already tracked directories.
        /// </summary>
        public List<string> NewDirectories
        {
            get
            {
                var trackList = TrackList;
                List<string> newDirectoriesList = new();

                string directoryToSearch = RootDirectoryToTrack;
                foreach (string trackedDirectoryName in trackList)
                {
                    // Optimization that skips already enumerated directories.
                    if (directoryToSearch.Contains(trackedDirectoryName)) continue;

                    directoryToSearch = trackedDirectoryName;
                    if (Directory.Exists(directoryToSearch)) newDirectoriesList.AddRange(Directory.EnumerateDirectories(directoryToSearch)
                                                                               .Where(directoryName => !trackList.Contains(directoryName)));
                }
                return newDirectoriesList;
            }
        }
        public List<string> RelativeFileNames
        {
            get
            {
                var fileNames = new List<string>();
                foreach (string directoryName in TrackList)
                {
                    if (Directory.Exists(directoryName))
                        fileNames.AddRange(Directory.EnumerateFiles(directoryName));
                }
                return fileNames.Select(file => RelativeName(file)).ToList();
            }
        }

        public string FullNameFromRelative(string relativePath) => Path.Join(RootDirectoryToTrack, relativePath);
        public string RelativeName(string fullPath) => Path.GetRelativePath(RootDirectoryToTrack, fullPath);


        public void Add(string directoryName)
        {
            var subDirectories = Directory.EnumerateDirectories(directoryName, "*", SearchOption.AllDirectories);

            using var writer = new StreamWriter(_config.ComponentLocationPath, append: true);
            writer.WriteLine(directoryName);
            foreach (var subDirectory in subDirectories) writer.WriteLine(subDirectory);
        }
        public void Remove(string directoryName)
        {
            var modifiedTrackList = TrackList;
            modifiedTrackList.Remove(directoryName);

            var subDirectories = Directory.EnumerateDirectories(directoryName, "*", SearchOption.AllDirectories);
            foreach (var subDirectory in subDirectories) modifiedTrackList.Remove(subDirectory);

            using var writer = new StreamWriter(_config.ComponentLocationPath);
            modifiedTrackList.ForEach(dir => writer.WriteLine(dir));
        }
    }
}
