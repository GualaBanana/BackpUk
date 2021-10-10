namespace SyncTool
{
    /// <summary>
    /// Abstract base class for providing configs to different parts of the system.
    /// </summary>
    /// <remarks>
    /// Provides an easy pluggable functionality by being an abstract class for every concrete <see cref="Config"/><br/>
    /// each of which is responsible for a distinct part of the <see cref="Sync"/> internal mechanism.<br/>
    /// </remarks>
    public abstract class Config
    {
        static string DirectoryName
        {
            get
            {
                if (OperatingSystem.IsWindows()) return ".synctool";
                throw new ArgumentException($"The AppRootDirectoryName is undetermined for the current operating system.", nameof(Environment.OSVersion));
            }
        }
        public static string InstallationPath
        {
            get
            {
                // Need to change it to determine the root system drive in a provided environment at runtime.
                if (OperatingSystem.IsWindows()) return $@"C:\users\{Environment.UserName}\{DirectoryName}\";
                throw new ArgumentException("The InstallationPath is undetermined for the current operating system.", nameof(Environment.OSVersion));
            }
        }


        protected abstract string ComponentName { get; }
        public string ComponentLocationPath => Path.Join(InstallationPath, ComponentName);
    }

    public class CloudConfig : Config
    {
        protected override string ComponentName { get; } = "$cloud";
    }

    public class TrackerConfig : Config
    {
        protected override string ComponentName { get; } = "track_list";


        public TrackerConfig()
        {
            using var _ = File.AppendText(ComponentLocationPath);
        }
    }
}
