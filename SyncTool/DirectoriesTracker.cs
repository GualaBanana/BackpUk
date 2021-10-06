using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTool
{
    /// <summary>
    ///     Manages the file in which the list of directories' names are stored.
    /// </summary>
    public class DirectoriesTracker
    {
        readonly OperatingSystem _os = Environment.OSVersion;
        string TrackedRootDirectory
        {
            get
            {
                if (_os.Platform == PlatformID.Unix) return @"/";
                // This is the hardcoded root drive that will be tracked.
                return @"D:\";
            }
        }
        public string GetFileNameRelativeToTrackedRootDirectory(FileInfo file) => file.FullName.Replace(TrackedRootDirectory, String.Empty);
        // Consider moving it to static `Config` class where will be static method `GetTrackFileFullName` or smth.
        readonly string _trackFileName = "track_info";
        string TrackFileFullName
        {
            get
            {
                if (_os.Platform == PlatformID.Unix) return @$"/etc/synctool/{_trackFileName}";
                return @$"C:\users\{Environment.UserName}\.synctool\{_trackFileName}";
            }
        }
        public List<DirectoryInfo> Directories
        {
            get
            {
                var directories = new List<DirectoryInfo>();
                foreach (string directoryName in ListedDirectories)
                {
                    directories.Add(new(directoryName));
                }
                return directories;
            }
        }
        // Can have potential problems. Also, I believe it prints an extra line for some reason. But I'm not sure.
        // Change to private after tests.
        public List<string> ListedDirectories
        {
            get
            {
                using var reader = new StreamReader(TrackFileFullName);
                var listedDirectories = new List<string>();
                while (!reader.EndOfStream) listedDirectories.Add(reader.ReadLine());
                return listedDirectories;
            }
        }

        /// <summary>
        ///     Initializes the instance of <see cref="DirectoriesTracker"/> that manages the file with directory names.
        /// </summary>
        /// <param name="fileName"></param>
        public DirectoriesTracker()
        {
            // Just creates the TrackFile if it doesn't exist already.
            using var _ = File.AppendText(TrackFileFullName);
        }

        // I'm not sure if the check for unique folders should be added here or not.
        public void Add(DirectoryInfo directory)
        {
            var topLevelSubdirectories = directory.EnumerateDirectories();

            using (var writer = new StreamWriter(TrackFileFullName, append: true))
            {
                writer.WriteLine(directory.FullName);
            }
            foreach (var dir in topLevelSubdirectories) Add(dir);
        }
        // At this moment implementation of `Add` and `Remove` differ. `Add` adds folders recursively
        // calling itself, but `Remove` can do the same but using a flag. So far it doesn't complicate anything.
        /// <summary>
        ///     Removes provided directory from the list of tracked directories.
        /// </summary>
        /// <remarks>
        ///     By default, removes only <paramref name="directory"/>, this behaviour can be changed with <paramref name="recursively"/> flag.
        /// </remarks>
        /// <param name="directory">the directory to remove from list of tracked directories.</param>
        /// <param name="recursively">if <c>true</c> than removes children directories of the <paramref name="directory"/>, <c>false</c>otherwise.</param>
        public void Remove(DirectoryInfo directory, bool recursively = false)
        {
            var modifiedDirectoriesList = ListedDirectories;
            modifiedDirectoriesList.Remove(directory.FullName);
            if (recursively)
            {
                var allSubdirectories = directory.EnumerateDirectories("*", SearchOption.AllDirectories);
                foreach (var dir in allSubdirectories)
                {
                    modifiedDirectoriesList.Remove(dir.FullName);
                }
            }
            using var writer = new StreamWriter(TrackFileFullName);
            modifiedDirectoriesList.ForEach(dir => writer.WriteLine(dir));
        }
    }
}
