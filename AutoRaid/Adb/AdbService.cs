using System;
using SharpAdbClient;
using System.IO;
using System.IO.Abstractions;
using System.Net;
using System.Threading;

namespace AutoRaid.Adb
{
    public sealed class AdbService : IAdbService, IDisposable
    {
        private readonly IFileSystem _fileSystem;
        private readonly IAdbServer _adbServer;
        private readonly IAdbClient _adbClient;
        private readonly ISyncService _syncService;

        public AdbService(
            IFileSystem fileSystem, 
            IAdbServer adbServer, 
            IAdbClient adbClient, 
            ISyncService syncService)
        {
            _fileSystem = fileSystem;
            _adbServer = adbServer;
            _adbClient = adbClient;
            _syncService = syncService;
        }

        public byte[] GetScreenshot()
        {
            var device = _adbClient.GetDevices()[0];
            var screenshotPathOnDevice = "/sdcard/DCIM/raid.png";

            _adbClient.ExecuteRemoteCommand($"screencap -p {screenshotPathOnDevice}", device, new DebugReceiver());

            using (var socket = new AdbSocket(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort)))
            {
                using (var memoryStream = new MemoryStream())
                {
                    _syncService.Pull(screenshotPathOnDevice, memoryStream, null, CancellationToken.None);
                    return memoryStream.ToArray();
                }
                
            }
        }

        public bool StartAdbServer()
        {
            var status = AdbServer.Instance.GetStatus();
            if (status.IsRunning == false)
            {
                var androidSdkRoot = Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT");
                var adbPath = Path.Combine(androidSdkRoot, "platform-tools", "adb.exe");
                if (File.Exists(adbPath))
                {
                    var startResult = AdbServer.Instance.StartServer(adbPath, restartServerIfNewer: false);
                    return startResult == StartServerResult.Started;
                }
            }

            return false;
        }

        public void Dispose() {
            _syncService.Dispose();
        }
    }
}
