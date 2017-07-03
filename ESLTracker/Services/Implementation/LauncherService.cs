using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.Utils.DiagnosticsWrappers;

namespace ESLTracker.Services
{
    public class LauncherService : ILauncherService
    {
        private const string BethesdaLaunch = "bethesdanet://run/5";
        private const string SteamLaunch = "steam://rungameid/364470";

        private const string BethesdaProtocol = "BethesdaNet";
        private const string SteamProtocol = "steam";

        public bool IsBethesdaLauncherInstalled()
        {
            var key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(BethesdaProtocol);
            return key != null;
        }

        public bool IsSteamInstalled()
        {
            var key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(SteamProtocol);
            return key != null;
        }

        public Process StartGame()
        {
            if (IsBethesdaLauncherInstalled())
            {
                return new ProcessWrapper().Start(BethesdaLaunch);
            }
            else if (IsSteamInstalled())
            {
                return new ProcessWrapper().Start(SteamLaunch);
            }
            else
            {
                return null;
            }
        }
    }
}
