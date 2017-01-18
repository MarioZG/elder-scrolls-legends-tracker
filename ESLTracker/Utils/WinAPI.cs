using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils
{
    public class WinAPI : IWinAPI
    {

        public static IWinAPI Default { get; } = new WinAPI();

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public bool IsGameActive()
        {
            IntPtr fw = GetForegroundWindow();
            Process eslProcess = GetEslProcess();
            IntPtr eslw = eslProcess == null ? IntPtr.Zero : eslProcess.MainWindowHandle;
            return fw == eslw;
        }

        Process eslProcess = null;
        public Process GetEslProcess()
        {
            if ((eslProcess == null)
                || (eslProcess.HasExited))
            {
                eslProcess = Process.GetProcesses().Where(p => p.MainWindowTitle == "The Elder Scrolls: Legends").FirstOrDefault();
            }
            return eslProcess;
        }

        public FileVersionInfo GetEslFileVersionInfo()
        {
            return GetEslProcess()?.MainModule?.FileVersionInfo;
        }

        public bool IsLauncherProcessRunning()
        {
            return Process.GetProcesses().Where(p => p.ProcessName == "BethesdaNetLauncher").FirstOrDefault() != null;

        }
    }
}
