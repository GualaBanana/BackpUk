namespace SyncTool
{
    /// <summary>
    /// Manages the file in which the list of directories' names are stored.
    /// </summary>
    public class Tracker : IRelativePathManager
    {
        readonly TrackerConfig _config = new();

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
        public List<string> NewDirectories
        {
            get
            {
                var trackList = TrackList;
                List<string> newDirectoriesList = new();

                string directoryToSearch = RootDirectoryToTrack;
                foreach (string trackedDirectory in trackList)
                {
                    // Optimization that skips already enumerated directories.
                    if (directoryToSearch.Contains(trackedDirectory)) continue;

                    directoryToSearch = trackedDirectory;
                    if (Directory.Exists(directoryToSearch)) newDirectoriesList.AddRange(Directory.EnumerateDirectories(directoryToSearch)
                                                                               .Where(directory => !trackList.Contains(directory)));
                }
                return newDirectoriesList;
            }
        }
        public List<string> RelativeFileNames
        {
            get
            {
                var fileNames = new List<string>();
                foreach (string directory in TrackList)
                {
                    if (Directory.Exists(directory))
                        fileNames.AddRange(Directory.EnumerateFiles(directory));
                }
                return fileNames.Select(file => RelativeName(file)).ToList();
            }
        }

        public string FullPathFromRelative(string relativePath) => Path.Join(RootDirectoryToTrack, relativePath);
        public string RelativeName(string fullPath) => Path.GetRelativePath(RootDirectoryToTrack, fullPath);


        public void Add(string directory)
        {
            var subDirectories = Directory.EnumerateDirectories(directory, "*", SearchOption.AllDirectories);

            using var writer = new StreamWriter(_config.ComponentLocationPath, append: true);
            writer.WriteLine(directory);
            foreach (var subDirectory in subDirectories) writer.WriteLine(subDirectory);
        }
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
