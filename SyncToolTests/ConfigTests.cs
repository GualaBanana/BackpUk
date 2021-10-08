using Xunit;
using SyncTool;

namespace SyncToolTests
{
    public class ConfigTests
    {
        [Fact]
        public void Should_ReturnAppropriateInstallationPath_BasedOnCurrentOS()
        {
            Assert.Contains($@"C:\users\{System.Environment.UserName}\", Config.InstallationPath);
        }
    }
}