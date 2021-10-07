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
        List<FileInfo> CurrentFiles
        {

            // Enumerate all files in each tracked directory.
            // I don't need to enumerate them with `SearchOption.AllDirectories`
            // as each directory that is tracked will be enumerated anyway.
            get
            {
                // Part of the contract I want to implement:
                // It's actually the same as in `DirectoriesTracker.Add()` method. I think they both can be made more generic.
                if (!AlreadyTracked.Any())
                    throw new InvalidOperationException("Methods that require directories names from `TrackLisk` to function can not be called with the empty `TrackList`.");

                var currentFiles = new List<FileInfo>();
                foreach (DirectoryInfo directory in _tracker.Directories)
                {
                    // Add options when dealing with the deleted directory that is still tracked by `track_list`:
                    //  - create the folder with this name from the list that refers to non-existent directory now
                    //  - remove the directory name from the list (stop tracking)

                    // As the directory being tracked and listed in `track_list` file might be deleted
                    // from the drive but not removed from this tracking list, catch `DirectoryNotFoundException`
                    // and do remove this directory from the list.
                    try
                    {
                        currentFiles.AddRange(directory.EnumerateFiles());
                    }
                    catch (DirectoryNotFoundException exception)
                    {
                        var message = exception.Message;
                        var from = message.IndexOf('\'') + 1;
                        var length = message.LastIndexOf('\'') - from;    // It's always the ending quote.
                        var deletedDirectory = new DirectoryInfo(message.Substring(from, length));

                        _tracker.Remove(deletedDirectory);
                    }
                }
                return currentFiles;
            }
        }
        List<FileInfo> NewFiles
        {
            get
            {
                // `CurrentFiles` breaks in case `_tracker.TrackList` contains directories from another drive.
                // Like if upon running the app user chose logic drive "C:\" but directories being tracked in
                // `TrackList` are the ones from drive "D:\" so `_tracker.RelativeName` works improperly and
                // call to `Path.GetRelativePath(RootDirectoryToTrack, fullPath)` results in the same `fullPath`
                // as this method cuts off nothing because `RootDirectoryToTrack` is not the part of `fullPath`.
                var currentFilesRelativeNames = CurrentFiles.Select(file => _tracker.RelativeName(file.FullName));

                var newFilesRelativeNames = currentFilesRelativeNames.Except(_cloud.RelativeFileNames);
                return newFilesRelativeNames.Select(fileName => new FileInfo(_tracker.FullPathFromRelative(fileName))).ToList();
            }
        }


        public Sync()
        {
            // Dictates the order of components' initialization.
            Directory.CreateDirectory(Config.InstallationPath);
            _cloud = new();
            _tracker = new();
        }


        public void ShowNewFiles() => NewFiles.ForEach(file => Console.WriteLine(file.FullName));
        //List<string> NewDirectories
        //{
        //    get
        //    {
        //        List<string> newDirectories = new();
        //        foreach (var directory in Tracker.ListedDirectories)
        //        {
        //            // If directory contains directories that are not in Tracker.ListedDirectories yet, add them to `newDirectories`.
        //        }
        //    }
        //}

        /// <summary>
        /// <paramref name="directory"/> needs to be checked/valid when passed to this method.
        /// </summary>
        /// <remarks>
        /// If <paramref name="directory"/> is already being tracked (invalid), than this method
        /// must not be called with this argument, as the method doesn't perform any checks itself.
        /// </remarks>
        /// <param name="directory">directory to add to the tracking system of the <see cref="Sync"/>.</param>
        public void StartTracking(DirectoryInfo directory) => _tracker.Add(directory);
        /// <summary>
        /// <inheritdoc cref="StartTracking(DirectoryInfo)"/>.
        /// </summary>
        /// <remarks>
        /// Should not be called with the <paramref name="directory"/> that is not tracked as it does nothing in this case.
        /// </remarks>
        /// <param name="directory">directory to remove from the tracking system of the <see cref="Sync"/>.</param>
        public void StopTracking(DirectoryInfo directory) => _tracker.Remove(directory);
        // Make event that is raised when synchronization is completed. Fosho!

        // After I add the check for directories as well, need to update this method to take it into consideration.
        public void Synchronize()
        {
            var newFiles = NewFiles;    // Cache while synchronizing.
            // Placeholder indicator in case there are no new files to sync. Should be substituted with raising of the event later
            // when the event is ready. Maybe for now should consider using preprocessor directive to optionally compile foreach statement.
            if (!newFiles.Any())
            {
                Console.WriteLine("All files are in sync.");
                return;
            }

            foreach (var file in newFiles)
            {
                if (file.DirectoryName != null && file.DirectoryName != Path.GetPathRoot(file.FullName))
                {
                    var sourceDirectory = new DirectoryInfo(file.DirectoryName);
                    var relativeDirectoryName = _tracker.RelativeName(sourceDirectory.FullName);
                    var directoryFullNameOnCloud = _cloud.FullPathFromRelative(relativeDirectoryName);
                    Directory.CreateDirectory(directoryFullNameOnCloud);
                }

                var fileRelativeName = _tracker.RelativeName(file.FullName);
                var fullFileNameOnCloud = _cloud.FullPathFromRelative(fileRelativeName);
                file.CopyTo(fullFileNameOnCloud);
            }
        }
    }
}
