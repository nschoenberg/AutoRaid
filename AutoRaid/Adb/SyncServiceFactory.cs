using System;
using AutoRaid.Contracts;
using SharpAdbClient;

namespace AutoRaid.Adb
{
    public class SyncServiceFactory : ISyncServiceFactory
    {
        public ISyncService Create(IAdbSocket socket, DeviceData data)
        {
            return new SyncService(socket, data);
        }
    }
}
