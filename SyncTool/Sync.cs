using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTool
{
    class Sync
    {
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
    // Consider making Cloud a separate class.
    DirectoryInfo Cloud
        {
            get
            {
                if (_os.Platform == PlatformID.Unix) return new($@"/etc/{AppFolderName}");
                // This is the hardcoded root drive that is implied as system drive by default.
                return new($@"C:\users\{Environment.UserName}\{AppFolderName}\$cloud");
            }
        }
        string GetFileNameRelativeToCloud(FileInfo file) => file.FullName.Replace(Cloud.FullName, String.Empty);


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
                    currentFiles.AddRange(directory.GetFiles());
                }
                return currentFiles;
            }
        }
        List<string> NewFiles
        {
            get
            {
                var currentFilesRelativeNames = CurrentFiles.Select(file => _tracker.GetFileNameRelativeToTrackedRootDirectory(file));
                var cloudFilesRelativeNames = Cloud.EnumerateFiles().Select(file => GetFileNameRelativeToCloud(file));
                return currentFilesRelativeNames.Except(cloudFilesRelativeNames).ToList();
            }
        }


        public Sync()
        {
            Directory.CreateDirectory(AppFolderFullName);
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


        public void Synchronize()
        {
            
        }
    }
}
