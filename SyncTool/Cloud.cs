using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTool
{
    public class Cloud
    {
        static string Location
        {
            get
            {
                if (Environment.OSVersion.Platform == PlatformID.Unix) return $@"/etc/synctool/$cloud/";
                return $@"C:\users\{Environment.UserName}\.synctool\$cloud\";
            }
        }
        // Doesn't throw an exception if the directory at the given path doesn't exist so can be initialized upon declaration.
        readonly DirectoryInfo _cloud = new(Location);
        public List<string> RelativeFileNames => EnumerateFiles().Select(file => file.FullName.Replace(Location, String.Empty)).ToList();


        public Cloud()
        {
            // Creates the directory at the path stored inside `_cloud` if it doesn't exist already, otherwise does nothing.
            _cloud.Create();
        }


        public IEnumerable<FileInfo> EnumerateFiles() => _cloud.EnumerateFiles("*", SearchOption.AllDirectories);
    }
}
