using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using ESLTracker.Utils.DiagnosticsWrappers;
using NLog;

namespace ESLTracker.Utils
{
    public class WinAPI : IWinAPI
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        private IProcessWrapper processWrapper;
        private const string ESLExeProcessName = "The Elder Scrolls Legends";
        private const string LauncherProcessName = "BethesdaNetLauncher";

        internal WinAPI() : this(new ProcessWrapper())
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

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        internal const int GWL_EXSTYLE = -20;
        internal const int WS_EX_TOOLWINDOW = 0x00000080;

        [DllImport("user32.dll")]
        internal static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        public bool IsGameActive()
        {
            IntPtr fw = GetForegroundWindow();
            Process proc = GetEslProcess();
            Logger.Trace($"Foreground window Ptr: {fw}");
            IntPtr eslw = proc == null ? IntPtr.Zero : proc.MainWindowHandle;
            return fw == eslw;
        }

        Process eslProcess = null;
        public Process GetEslProcess()
        {
            try
            {
                if ((eslProcess == null)
                || (eslProcess.HasExited))
                {

                    eslProcess = processWrapper.GetProcessesByName(ESLExeProcessName).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                Logger.Info(ex, "Exception while getting ESL process");
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

        ///https://stackoverflow.com/questions/7162834/determine-if-current-application-is-activated-has-focus
        public bool IsTrackerActive()
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }

            var procId = Process.GetCurrentProcess().Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == procId;
        }
    }
}
