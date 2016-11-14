using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker
{
    public class WindowsUtils
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        public static bool IsGameActive()
        {
            IntPtr fw = GetForegroundWindow();
            Process eslProcess = Process.GetProcesses().Where(p => p.MainWindowTitle == "The Elder Scrolls: Legends").FirstOrDefault();
            IntPtr eslw = eslProcess == null ? IntPtr.Zero : eslProcess.MainWindowHandle ;
            return fw == eslw;
        }
    }
}
