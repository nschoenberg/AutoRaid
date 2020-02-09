using System.IO.Abstractions.TestingHelpers;
using AutoRaid.Adb;
using AutoRaid.Contracts;
using NSubstitute;
using SharpAdbClient;
using Xunit;

namespace AutoRaid.Tests.Adb
{
    public class AdbServiceTests
    {
        private readonly IAdbService _adbService;
        private readonly IEnvironmentService _environmentServiceMock;
        private readonly MockFileSystem _fileSystemMock;
        private readonly IAdbServer _adbServer;

        public AdbServiceTests()
        {
            _fileSystemMock = new MockFileSystem();
            _adbServer = Substitute.For<IAdbServer>();
            
            var adbClient = Substitute.For<IAdbClient>();
            var syncServiceFactory = Substitute.For<ISyncServiceFactory>();
            _environmentServiceMock = Substitute.For<IEnvironmentService>();

            _adbService = new AdbService(
                _fileSystemMock,
                _adbServer,
                adbClient,
                syncServiceFactory,
                _environmentServiceMock);
        }

        [Fact]
        public void GetScreenshot_ReturnsValidArray()
        {
            var result = _adbService.GetScreenshot();
            Assert.True(result.Length > 100);
        }

        [Fact]
        public void StartAdbServer_StartsWhenNotAlreadyRunning()
        {
            _adbServer.GetStatus().Returns(new AdbServerStatus { IsRunning = false });
            _adbServer.StartServer("", false).ReturnsForAnyArgs(StartServerResult.Started);
            _environmentServiceMock.GetEnvironmentVariable("ANDROID_SDK_ROOT").Returns(@"C:\android-sdk");
            _fileSystemMock.AddFile(@"C:\android-sdk\platform-tools\adb.exe", MockFileData.NullObject);
            
            var started = _adbService.EnsureAdbServerRunning();
            
            Assert.True(started);

        }
    }
}
