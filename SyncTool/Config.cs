using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTool
{
    /// <summary>
    /// Base abstract class for providing configs to different parts of the system.
    /// </summary>
    /// <remarks>
    /// Provides an easy pluggable functionality by being an abstract class for every concrete <see cref="Config"/><br/>
    /// each of which is responsible for a distinct part of the <see cref="Sync"/> internal mechanism.<br/>
    /// </remarks>
    public abstract class Config
    {
        protected OperatingSystem _os = Environment.OSVersion;
        protected virtual string AppFolderName { get; } = "synctool";
        // TODO:
        // Here need to be properties responsible for paths like `Cloud.Location`, `DirectoriesTracker.TrackedRootDirectory`, etc.


        // Provide a separate interface for these. Name needs to be related to the classes that perform this functionality.
        // Like, what's their purpose? Based on the answer, I need to come up with the name for the interface.
        public abstract string FullPathFromRelative(string relativePath);
        public abstract string FullPathFromRelative(FileSystemInfo entry);
        public abstract string RelativeNameOf(string fullPath);
        public abstract string RelativeNameOf(FileSystemInfo fullPath);
    }

    public class CloudConfig : Config
    {
        protected override string AppFolderName => '.' + base.AppFolderName;

        public override string FullPathFromRelative(string relativePath) => throw new NotImplementedException();

        public override string FullPathFromRelative(FileSystemInfo entry)
        {
            throw new NotImplementedException();
        }

        public override string RelativeNameOf(string fullPath)
        {
            throw new NotImplementedException();
        }

        public override string RelativeNameOf(FileSystemInfo fullPath)
        {
            throw new NotImplementedException();
        }
    }

    public class TrackerConfig : Config
}
