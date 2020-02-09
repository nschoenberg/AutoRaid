using System;
using System.IO;
using AutoRaid.Adb;
using JetBrains.Annotations;
using NSubstitute;
using SharpAdbClient;
using Xunit;

namespace AutoRaid.Tests.Adb
{
    public class AdbUtilTests
    {

        public AdbUtilTests()
        {
            IAdbServer adbServer = Substitute.For<IAdbServer>();
            System.IO.Abstractions.IFileSystem fileSystem;
        }

        [Fact]
        public void GetScreenshot_ReturnsValidArray()
        {
            var result = new AdbService().GetScreenshot();
            Assert.True(result.Length > 100);
        }

        [Fact]
        public void StartAdbServer_StartsWhenNotAlreadyRunning()
        {
            
            var adbUtil = new AdbService();
            
            var started = adbUtil.StartAdbServer();
            Assert.True(started);
        }
    }
}
