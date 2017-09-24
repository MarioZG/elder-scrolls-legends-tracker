using System.Diagnostics;

namespace ESLTracker.Services
{
    public interface IWinAPI
    {
        Process GetEslProcess();
        bool IsLauncherProcessRunning();
        bool IsGameActive();
        FileVersionInfo GetEslFileVersionInfo();
        bool IsTrackerActive();
    }
}