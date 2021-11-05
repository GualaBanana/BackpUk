namespace BackpUk
{
    // Always valid static app-specific information.
    public static class Config
    {
        public const string AppName = "BackpUk";
        public static string LocalAppData { get; } = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppName);
        public static string TrackerFileLocation { get; } = Path.Join(LocalAppData, "track_list");
        public static string UserSettingsFileLocation { get; } = Path.Join(LocalAppData, "settings.json");
    }
}