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
                    try
                    {
                        currentFiles.AddRange(directory.GetFiles());
                    }
                    catch (Exception) { }
                }
                return currentFiles;
            }
        }
        List<string> NewFiles
        {
            get
            {
                var currentFilesRelativeNames = CurrentFiles.Select(file => _tracker.RelativeToTrackedRootDirectoryNameOf(file));
                return currentFilesRelativeNames.Except(_cloud.RelativeFileNames).ToList();
                //return _cloud.RelativeFileNames.ToList();
            }
        }


        public Sync()
        {
            Directory.CreateDirectory(AppFolderFullName);
            _cloud = new();
            _tracker = new();
        }


        public void ShowNewFiles() => NewFiles.ForEach(file => Console.WriteLine(file));
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

        // This provided `directory` needs to be checked somewhere (most likely somewhere higher in abstraction, even before calling 
        // `SyncTool` methods. Yeah, like when you are looking at options of a directory on your phone. If the directory is already 
        // being tracked, than there is no such option as to `StartTracking`. Same here. `StartTracking` mustn't be called with invalid
        // arguments as it doesn't perform any checks, this is not its responsibility.

        // I think they should take `DirectoryInfo` instances instead of strings. Will see later.
        public void StartTracking(IEnumerable<string> directories)
        {
            foreach (var directory in directories) StartTracking(directory);
        }
        public void StartTracking(string directory) => _tracker.Add(new(directory));
        public void Synchronize()
        {
            
        }
    }
}
