namespace SyncTool
{
    class Sync
    {
        Cloud _cloud { get; }
        Tracker _tracker { get; }

        public List<string> TrackedDirectories => _tracker.TrackList;
        static int JustSyncedItemsCount { get; set; } = 0;


        public Sync()
        {
            // Dictates the order of components' initialization.
            Directory.CreateDirectory(Config.InstallationPath);
            _cloud = new();
            _tracker = new();
        }


        public void StartTracking(string directory)
        {
            UsagePolicy.MustBeOnRightDrive(directory);
            UsagePolicy.MustExist(directory);
            UsagePolicy.MustBeNotTracked(directory, _tracker);

            _tracker.Add(directory);
        }
        public void StopTracking(string directory)
        {
            UsagePolicy.MustBeTracked(directory, _tracker);

            _tracker.Remove(directory);
        }
        public void Synchronize()
        {
            JustSyncedItemsCount = 0;

            SynchronizeDirectories();
            SynchronizeFiles(source: _tracker, destination: _cloud);

            if (JustSyncedItemsCount > 0) SyncCompleted?.Invoke("Synchronization completed.", JustSyncedItemsCount);
        }
        public void SynchronizeWithCloud() => SynchronizeFiles(_cloud, _tracker);
        void SynchronizeDirectories()
        {
            var newNestedDirectories = _tracker.NewDirectories;

            newNestedDirectories.ForEach(directory => StartTracking(directory));
            // Do not return from this method prematurely as the following line of code needs to create all tracked directories.
            foreach (var directory in _tracker.TrackList.Select(directory => _tracker.RelativeName(directory)))
            {
                var directoryFullNameOnCloud = _cloud.FullNameFromRelative(directory);

                if (!Directory.Exists(directoryFullNameOnCloud))
                {
                    Directory.CreateDirectory(directoryFullNameOnCloud);
                    JustSyncedItemsCount++;
                }
            }
        }
        static void SynchronizeFiles(IRelativePathManager source, IRelativePathManager destination)
        {
            var newFiles = source.RelativeFileNames.Except(destination.RelativeFileNames);

            if (!newFiles.Any()) return;

            foreach (string file in newFiles)
            {
                string? fileLocation;
                if ((fileLocation = Path.GetDirectoryName(file)) != null)
                {
                    string destinationDirectory = destination.FullNameFromRelative(fileLocation);
                    Directory.CreateDirectory(destinationDirectory);
                }

                if (!File.Exists(file))
                {
                    string from = source.FullNameFromRelative(file);
                    string to = destination.FullNameFromRelative(file);
                    File.Copy(from, to);
                }
            }

            JustSyncedItemsCount += newFiles.Count();
        }


        public event EventHandler<int>? SyncCompleted;
    }
}
