namespace BackpUk
{
    class BackpUker
    {
        BackpUk _backpUk { get; }
        Tracker _tracker { get; }

        public List<string> TrackedDirectories => _tracker.TrackList;
        static int PackedItemsCount { get; set; } = 0;


        public BackpUker()
        {
            // Dictates the order of components' initialization.
            Directory.CreateDirectory(Config.InstallationPath);
            _backpUk = new();
            _tracker = new();
        }


        public void StartTracking(string directoryName)
        {
            UsagePolicy.MustBeOnRightDrive(directoryName);
            UsagePolicy.MustExist(directoryName);
            UsagePolicy.MustBeNotTracked(directoryName, _tracker);

            _tracker.Add(directoryName);
        }
        public void StopTracking(string directoryName)
        {
            UsagePolicy.MustBeTracked(directoryName, _tracker);

            _tracker.Remove(directoryName);
        }
        public void PutInBackpUk()
        {
            PackedItemsCount = 0;

            PackDirectories();
            PackFiles(source: _tracker, destination: _backpUk);

            if (PackedItemsCount > 0) PackingCompleted?.Invoke(null, PackedItemsCount);
        }
        public void FetchFromBackpUk()
        {
            PackFiles(source: _backpUk, destination: _tracker);
            UnpackingCompleted?.Invoke(null, PackedItemsCount);
        }

        void PackDirectories()
        {
            var newNestedDirectories = _tracker.NewDirectories;

            newNestedDirectories.ForEach(directoryName => StartTracking(directoryName));
            // Do not return from this method prematurely as the following line of code needs to create all tracked directories.
            foreach (var directoryName in _tracker.TrackList.Select(directory => _tracker.RelativeName(directory)))
            {
                var directoryFullNameInBackpUk = _backpUk.FullNameFromRelative(directoryName);

                if (!Directory.Exists(directoryFullNameInBackpUk))
                {
                    Directory.CreateDirectory(directoryFullNameInBackpUk);
                    PackedItemsCount++;
                }
            }
        }
        static void PackFiles(IRelativePathManager source, IRelativePathManager destination)
        {
            var newFiles = source.RelativeFileNames.Except(destination.RelativeFileNames);

            if (!newFiles.Any()) return;

            foreach (string fileName in newFiles)
            {
                string? fileLocation;
                if ((fileLocation = Path.GetDirectoryName(fileName)) != null)
                {
                    string destinationDirectory = destination.FullNameFromRelative(fileLocation);
                    Directory.CreateDirectory(destinationDirectory);
                }

                if (!File.Exists(fileName))
                {
                    string from = source.FullNameFromRelative(fileName);
                    string to = destination.FullNameFromRelative(fileName);
                    File.Copy(from, to);
                }
            }

            PackedItemsCount += newFiles.Count();
        }


        public event EventHandler<int>? PackingCompleted;
        public event EventHandler<int>? UnpackingCompleted;
    }
}
