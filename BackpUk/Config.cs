namespace BackpUk
{
    public static class Config
    {
        static string DirectoryName
        {
            get
            {
                if (OperatingSystem.IsWindows()) return ".backpuk";
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
        public static string BackpUkLocation { get; } = Path.Join(@"E:\", "$backpuk");
        public static string TrackerFileLocation { get; } = Path.Join(InstallationPath, "track_list"); 
    }
}
