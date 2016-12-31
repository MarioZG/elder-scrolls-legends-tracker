using System.Diagnostics;

namespace ESLTracker.Utils
{
    public interface IWinAPI
    {
        Process GetEslProcess();
        bool IsLauncherProcessRunning();
        bool IsGameActive();
        FileVersionInfo GetEslFileVersionInfo();
    }
}