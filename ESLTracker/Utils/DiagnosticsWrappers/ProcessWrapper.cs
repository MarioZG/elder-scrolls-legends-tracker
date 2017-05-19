using System;
using System.Diagnostics;

namespace ESLTracker.Utils.DiagnosticsWrappers
{
    public class ProcessWrapper : IProcessWrapper
    {
        public Process[] GetProcesses()
        {
            return Process.GetProcesses();
        }

        public Process[] GetProcessesByName(string processName)
        {
            return Process.GetProcessesByName(processName);
        }

        public Process Start(string fileName)
        {
            return Process.Start(fileName);
        }
    }
}
