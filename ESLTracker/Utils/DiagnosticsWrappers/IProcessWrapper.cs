using System.Diagnostics;

namespace ESLTracker.Utils.DiagnosticsWrappers
{
    public interface IProcessWrapper
    {
        Process Start(string fileName);
        Process[] GetProcessesByName(string processName);
        Process[] GetProcesses();
    }
}
