namespace SyncTool
{
    class Sync
    {
        readonly Cloud _cloud;
        readonly DirectoriesTracker _tracker;

        /// <summary>
        /// <see cref="List{T}"/> of directories' names that are already being tracked by the app.
        /// </summary>
        public List<string> AlreadyTracked => _tracker.TrackList;
        List<string> CurrentFiles
        {

            // Enumerate all files in each tracked directory.
            // I don't need to enumerate them with `SearchOption.AllDirectories`
            // as each directory that is tracked will be enumerated anyway.
            get
            {       
                var currentFiles = new List<string>();
                var trackedAndNewDirectories = _tracker.TrackList;
                trackedAndNewDirectories.AddRange(NewDirectories);

                foreach (string directory in trackedAndNewDirectories)
                {
                    // Add options when dealing with the deleted directory that is still tracked by `track_list`:
                    //  - create the folder with this name from the list that refers to non-existent directory now
                    //  - remove the directory name from the list (stop tracking)

                    // As the directory being tracked and listed in `track_list` file might be deleted
                    // from the drive but not removed from this tracking list, catch `DirectoryNotFoundException`
                    // and do remove this directory from the list.
                    try
                    {
                        currentFiles.AddRange(Directory.EnumerateFiles(directory));
                    }
                    catch (DirectoryNotFoundException exception)
                    {
                        var message = exception.Message;
                        var from = message.IndexOf('\'') + 1;
                        var length = message.LastIndexOf('\'') - from;    // It's always the ending quote.
                        var deletedDirectory = message.Substring(from, length);

                        _tracker.Remove(deletedDirectory);
                    }
                }
                return currentFiles;
            }
        }
        List<string> NewFilesOnDrive
        {
            get
            {
                var currentFilesRelativeNames = CurrentFiles.Select(file => _tracker.RelativeName(file));
                var newFilesRelativeNames = currentFilesRelativeNames.Except(_cloud.RelativeFileNames);
                return newFilesRelativeNames.Select(fileName => _tracker.FullPathFromRelative(fileName)).ToList();
            }
        }
        List<string> NewFilesOnCloud
        {
            get
            {
                var newFilesRelativeNames = _cloud.RelativeFileNames.Except(CurrentFiles.Select(file => _tracker.RelativeName(file)));
                return newFilesRelativeNames.Select(fileName => _cloud.FullPathFromRelative(fileName)).ToList();
            }
        }
        List<string> NewDirectories
        {
            get
            {
                List<string> trackList = _tracker.TrackList;
                List<string> newDirectoriesList = new();

                string parent = _tracker.RootDirectoryToTrack;
                foreach (string trackedDirectory in trackList)
                {
                    // Optimization that skips already enumerated directories.
                    if (parent.Contains(trackedDirectory)) continue;

                    parent = trackedDirectory;
                    try
                    {
                        newDirectoriesList.AddRange(Directory.EnumerateDirectories(parent).Where(directory => !trackList.Contains(directory)));
                    }
                    catch (DirectoryNotFoundException) { }
                }
                return newDirectoriesList;
            }
        }


        public Sync()
        {
            // Dictates the order of components' initialization.
            Directory.CreateDirectory(Config.InstallationPath);
            _cloud = new();
            _tracker = new();
        }


        public void ShowNewFiles() => NewFilesOnDrive.ForEach(file => Console.WriteLine(file));


        /// <summary>
        /// <paramref name="directory"/> needs to be checked/valid when passed to this method.
        /// </summary>
        /// <remarks>
        /// If <paramref name="directory"/> is already being tracked (invalid), than this method
        /// must not be called with this argument, as the method doesn't perform any checks itself.
        /// </remarks>
        /// <param name="directory">directory to add to the tracking system of the <see cref="Sync"/>.</param>
        public void StartTracking(string directory)
        {
            UsagePolicy.MustExist(directory);
            UsagePolicy.MustBeNotTracked(directory, _tracker);

            _tracker.Add(directory);
        }
        /// <summary>
        /// <inheritdoc cref="StartTracking(DirectoryInfo)"/>.
        /// </summary>
        /// <remarks>
        /// Should not be called with the <paramref name="directory"/> that is not tracked as it does nothing in this case.
        /// </remarks>
        /// <param name="directory">directory to remove from the tracking system of the <see cref="Sync"/>.</param>
        public void StopTracking(string directory)
        {
            UsagePolicy.MustBeTracked(directory, _tracker);

            _tracker.Remove(directory);
        }
        // Make event that is raised when synchronization is completed. Fosho!
        public void Synchronize()
        {
            
            // Placeholder for calling some functions that will propose to start tracking some directories of the user because
            // at this point nothing is being tracked, thus there is nothing to synchronize.
            if (!_tracker.TrackList.Any())
            {
                Console.WriteLine("None of your directories are being synced.");
                // Propose to start syncing something here.
                return;
            }
            // Placeholder indicator in case there are no new files to sync. Should be substituted with raising of the event later
            // when the event is ready. Maybe for now should consider using preprocessor directive to optionally compile foreach statement.
            //if (!NewFilesOnDrive.Any())
            //{
            //    Console.WriteLine("All files are in sync with the cloud.");
            //    return;
            //}
            SynchronizeDirectories();
            SynchronizeFiles(_tracker, _cloud);
        }
        public void SynchronizeWithCloud() => SynchronizeFiles(_cloud, _tracker);
        void SynchronizeDirectories() => NewDirectories.ForEach(newDirectory => StartTracking(newDirectory));
        // Maybe change source and destination to some kind of enum like `SearchOption` with two enum constants:
        // FromCloud
        // FromStorage
        void SynchronizeFiles(IRelativePathManager source, IRelativePathManager destination)
        {
            var newFiles = source == _tracker ? NewFilesOnDrive : NewFilesOnCloud;

            foreach (var file in newFiles)
            {
                // If the directory is a root, return value is null, as well.
                string? fileParent;
                if ((fileParent = Path.GetDirectoryName(file)) != null)
                {
                    var sourceRelativeDirectoryName = source.RelativeName(fileParent);
                    var destinationDirectoryPath = destination.FullPathFromRelative(sourceRelativeDirectoryName);
                    Directory.CreateDirectory(destinationDirectoryPath);
                }

                var sourceRelativeName = source.RelativeName(file);
                var destinationFilePath = destination.FullPathFromRelative(sourceRelativeName);
                File.Copy(file, destinationFilePath);
            }
        }
    }
}
