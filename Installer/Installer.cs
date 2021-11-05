BackpUk.Installer.Installer.Run();

namespace BackpUk.Installer
{
    internal static class Installer
    {
        public static void Run()
        {
            if (IsAlreadyInstalled)
            {
                Console.WriteLine($"{Config.AppName} is already installed.");
                return;
            }

            InitializeLocalAppDataDirectory();
            Console.WriteLine($"{Config.AppName} has been successfully installed.");
        }

        static bool IsAlreadyInstalled => Directory.Exists(Config.LocalAppData);
        static void InitializeLocalAppDataDirectory()
        {
            Directory.CreateDirectory(Config.LocalAppData);
            File.Create(Config.TrackerFileLocation);
            File.Create(Config.UserSettingsFileLocation);
        }
    }
}