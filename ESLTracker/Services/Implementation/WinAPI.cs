using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using ESLTracker.Utils.DiagnosticsWrappers;
using NLog;

namespace ESLTracker.Services
{
    public class WinAPI : IWinAPI
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        private IProcessWrapper processWrapper;
        private const string ESLExeProcessName = "The Elder Scrolls Legends";
        private const string LauncherProcessName = "BethesdaNetLauncher";

        public WinAPI() : this(new ProcessWrapper())
        {
        }

        public WinAPI(IProcessWrapper processWrapper)
        {
            this.processWrapper = processWrapper;
        }


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
            Process proc = GetEslProcess();
            IntPtr eslw = proc == null ? IntPtr.Zero : proc.MainWindowHandle;
            return fw == eslw;
        }

        Process eslProcess = null;
        public Process GetEslProcess()
        {
            if ((eslProcess == null)
                || (eslProcess.HasExited))
            {
                try
                {
                    eslProcess = processWrapper.GetProcessesByName(ESLExeProcessName).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    Logger.Info(ex, "Exception while getting ESL process");
                }
            }
            return eslProcess;
        }

        public FileVersionInfo GetEslFileVersionInfo()
        {
            return GetEslProcess()?.MainModule?.FileVersionInfo;
        }

        public bool IsLauncherProcessRunning()
        {
            bool ret = false;
            try
            {
                ret = processWrapper.GetProcessesByName(LauncherProcessName).FirstOrDefault() != null;
            }
            catch (Exception ex)
            {
                Logger.Info(ex, "Exception while getting Launcher process");
            }
            return ret;
        }
    }
}
