namespace AutoRaid.Adb
{
    public interface IAdbService
    {
        byte[] GetScreenshot();
        bool EnsureAdbServerRunning();
    }
}