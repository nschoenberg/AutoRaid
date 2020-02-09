using SharpAdbClient;

namespace AutoRaid.Contracts
{
    public interface ISyncServiceFactory
    {
        ISyncService Create(IAdbSocket socket, DeviceData data);
    }
}
