using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTool
{
    class Sync
    {
        // Create static `Config` class that will contain static properties for getting OS dependent config info.
        // For initializing these fields at the beginning of classes' declarations.
        // Move this OS dependent config logic to a spearate config class that will be responsible for this.
        readonly OperatingSystem _os = Environment.OSVersion;
        string AppFolderName
        {
            get
            {
                if (_os.Platform == PlatformID.Unix) return "synctool";
                return ".synctool";
            }
        }
        string AppFolderFullName
        {
            get
            {
                if (_os.Platform == PlatformID.Unix) return $@"/etc/{AppFolderName}";
                return $@"C:\users\{Environment.UserName}\{AppFolderName}";
            }
        }
        readonly Cloud _cloud;
        readonly DirectoriesTracker _tracker;
        List<FileInfo> CurrentFiles
        {
            // Enumerate all files in each tracked directory.
            // I don't need to enumerate them with `SearchOption.AllDirectories`
            // as each directory that is tracked will be enumerated anyway.
            get
            {
                var currentFiles = new List<FileInfo>();
                foreach (DirectoryInfo directory in _tracker.Directories)
                {
                    // Add options when dealing with the deleted directory that is still tracked by `track_info`:
                    //  - create the folder with this name from the list that refers to non-existent directory now
                    //  - remove the directory name from the list (stop tracking)

                    // As the directory being tracked and listed in `track_info` file might be deleted
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
                var currentFilesRelativeNames = CurrentFiles.Select(file => _tracker.RelativeNameOf(file));

                var newFilesRelativeNames = currentFilesRelativeNames.Except(_cloud.RelativeFileNames);
                return newFilesRelativeNames.Select(fileName => new FileInfo(_tracker.FullPathFromRelative(fileName))).ToList();
            }
        }


        public Sync()
        {
            Directory.CreateDirectory(AppFolderFullName);
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
        // After I add the check for directories as well, need to update this method to take it into consideration.
        public void Synchronize()
        {
            var newFiles = NewFiles;    // Cache while synchronizing.
            // Placeholder indicator in case there are no new files to sync.
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
                    var relativeDirectoryName = _tracker.RelativeNameOf(sourceDirectory);
                    var directoryFullNameOnCloud = _cloud.FullPathFromRelative(relativeDirectoryName);
                    Directory.CreateDirectory(directoryFullNameOnCloud);
                }

                var fullFileNameOnCloud = Path.Join(_cloud.Location, _tracker.RelativeNameOf(file));
                file.CopyTo(fullFileNameOnCloud);
            }
        }
    }
}
