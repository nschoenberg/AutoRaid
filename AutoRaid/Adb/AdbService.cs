using System;
using SharpAdbClient;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using AutoRaid.Contracts;

namespace AutoRaid.Adb
{
    public sealed class AdbService : IAdbService
    {
        private readonly IFileSystem _fileSystem;
        private readonly IAdbServer _adbServer;
        private readonly IAdbClient _adbClient;
        private readonly ISyncServiceFactory _syncServiceFactory;
        private readonly IEnvironmentService _environmentService;

        public AdbService(
            IFileSystem fileSystem, 
            IAdbServer adbServer, 
            IAdbClient adbClient, 
            ISyncServiceFactory syncServiceFactory,
            IEnvironmentService environmentService)
        {
            _fileSystem = fileSystem;
            _adbServer = adbServer;
            _adbClient = adbClient;
            _syncServiceFactory = syncServiceFactory;
            _environmentService = environmentService;
        }

        public byte[] GetScreenshot()
        {
            var isConnected = EnsureAdbServerRunning();
            if (isConnected == false)
            {
                return Array.Empty<byte>();
            }

            var device = _adbClient.GetDevices()[0];
            const string screenshotPathOnDevice = "/sdcard/DCIM/raid.png";

            _adbClient.ExecuteRemoteCommand($"screencap -p {screenshotPathOnDevice}", device, new DebugReceiver());

            using var memoryStream = new MemoryStream();
            using var adbSocket = new AdbSocket(_adbClient.EndPoint);
            using var syncService = _syncServiceFactory.Create(adbSocket, device);
            syncService.Pull(screenshotPathOnDevice, memoryStream, null, CancellationToken.None);
            return memoryStream.ToArray();
        }

        public bool EnsureAdbServerRunning()
        {
            var status = _adbServer.GetStatus();
            if (status.IsRunning == false)
            {
                var androidSdkRoot = _environmentService.GetEnvironmentVariable("ANDROID_SDK_ROOT");
                var adbPath = _fileSystem.Path.Combine(androidSdkRoot, "platform-tools", "adb.exe");
                if (_fileSystem.File.Exists(adbPath))
                {
                    var startResult = _adbServer.StartServer(adbPath, restartServerIfNewer: false);
                    return startResult == StartServerResult.Started;
                }
            }

            return true;
        }
    }
}
