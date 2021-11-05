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
            // I need to check
            // if this directory actually exists to determine whether the app can be run. Otherwise, some kind of message
            // about app files corruption should be shown to the user. And maybe some options like to reinstall it. But I need
            // to think about whether it should be done here or in Config class or rather even in some other class
            // like AppFilesValidator whose sole purpose will be to check if all the files needed for running the app are valid
            // and not corrupted or deleted which will be run in Program.cs every time the app starts.

            UserSettingsManager.RunSetUp();
            // Dictates the order of components' initialization.
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

            if (PackedItemsCount == 0)
            {
                Console.WriteLine("All items are already packed.");
                return;
            }

            PackingCompleted?.Invoke(null, PackedItemsCount);
        }
        public void FetchFromBackpUk()
        {
            PackedItemsCount = 0;

            PackFiles(source: _backpUk, destination: _tracker);
            if (PackedItemsCount == 0)
            {
                Console.WriteLine("All items are already unpacked.");
                return;
            }

            UnpackingCompleted?.Invoke(null, PackedItemsCount);
        }

        void PackDirectories()
        {
            var newNestedDirectories = _tracker.NewDirectories;

            newNestedDirectories.ForEach(directoryName => StartTracking(directoryName));
            // Do not return from this method prematurely as the following block of code needs to create all tracked directories.
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
            var sourceFiles = source.RelativeFileNames;

            foreach (string fileName in sourceFiles)
            {
                string from, to;

                if (File.Exists(to = destination.FullNameFromRelative(fileName))) continue;
                from = source.FullNameFromRelative(fileName);

                string? fileParentsDirectories;
                if ((fileParentsDirectories = Path.GetDirectoryName(fileName)) != null)
                {
                    Directory.CreateDirectory(destination.FullNameFromRelative(fileParentsDirectories));
                }
                
                File.Copy(from, to);
                PackedItemsCount++;
            }
        }


        public event EventHandler<int>? PackingCompleted;
        public event EventHandler<int>? UnpackingCompleted;
    }
}
