using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTool
{
    public class Cloud
    {
        public string Location
        {
            get
            {
                if (Environment.OSVersion.Platform == PlatformID.Unix) return $@"/etc/synctool/$cloud/";
                return $@"C:\users\{Environment.UserName}\.synctool\$cloud\";
            }
        }
        readonly DirectoryInfo _cloud;
        public List<string> RelativeFileNames => EnumerateFiles().Select(file => file.FullName.Replace(Location, String.Empty)).ToList();
        /// <summary>
        /// <inheritdoc cref="FullPathFromRelative(string)"/>.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public string FullPathFromRelative(FileSystemInfo entry) => FullPathFromRelative(entry.FullName);
        /// <summary>
        /// Constructs the path inside the <see cref="Cloud"/> from a relative path.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public string FullPathFromRelative(string relativePath) => Path.Join(Location, relativePath);


        public Cloud()
        {
            _cloud = new(Location);
            // Creates the directory at the path stored inside `_cloud` if it doesn't exist already, otherwise does nothing.
            _cloud.Create();
        }


        public IEnumerable<FileInfo> EnumerateFiles() => _cloud.EnumerateFiles("*", SearchOption.AllDirectories);
    }
}
