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

        public bool IsGameActive()
        {
            IntPtr fw = GetForegroundWindow();
            Process eslProcess = GetEslProcess();
            IntPtr eslw = eslProcess == null ? IntPtr.Zero : eslProcess.MainWindowHandle;
            return fw == eslw;
        }

        public Process GetEslProcess()
        {
            return Process.GetProcesses().Where(p => p.MainWindowTitle == "The Elder Scrolls: Legends").FirstOrDefault();
        }
    }
}
