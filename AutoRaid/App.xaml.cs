using System.IO.Abstractions;
using Prism.Ioc;
using AutoRaid.Views;
using System.Windows;
using AutoRaid.Adb;
using AutoRaid.Contracts;
using SharpAdbClient;

namespace AutoRaid
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IEnvironmentService, EnvironmentService>();
            containerRegistry.RegisterSingleton<IFileSystem, FileSystem>();
            containerRegistry.RegisterSingleton<ISyncServiceFactory, SyncServiceFactory>();
            containerRegistry.RegisterInstance(AdbServer.Instance);
            containerRegistry.RegisterInstance(AdbClient.Instance);
            containerRegistry.RegisterSingleton<IAdbService, AdbService>();
        }
    }
}
