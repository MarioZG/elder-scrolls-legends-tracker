using ESLTracker.Utils;
using ESLTracker.Utils.DiagnosticsWrappers;
using ESLTracker.Utils.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.GameClient
{
    public class LauncherService : ILauncherService
    {
        private const string BethesdaLaunch = "bethesdanet://run/5";
        private const string SteamLaunch = "steam://rungameid/364470";

        private const string BethesdaProtocol = "BethesdaNet";
        private const string SteamProtocol = "steam";

        IWinAPI winApi;
        IMessenger messanger;
        IProcessWrapper processWrapper;

        public LauncherService(IWinAPI winApi, IMessenger messanger, IProcessWrapper processWrapper)
        {
            this.winApi = winApi;
            this.messanger = messanger;
            this.processWrapper = processWrapper;
        }

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


        public async Task<Process> StartGame()
        {
            bool isLauncherRunning = winApi.IsLauncherProcessRunning();
            Process proc = null;
            if (winApi.GetEslProcess() == null)
            {
                proc = LaunchGameFromPlatform();
                if (proc != null)
                {
                    var launcher = proc.StartInfo.FileName.Substring(0, proc.StartInfo.FileName.IndexOf(":", StringComparison.InvariantCulture));
                    messanger.Send(new ApplicationShowBalloonTip("ESL Tracker", "Starting game using " + launcher));
                    await Task.Delay(TimeSpan.FromSeconds(60)); //wait 10 sec
                    if (winApi.GetEslProcess() == null)
                    {
                        messanger.Send(new ApplicationShowBalloonTip("ESL Tracker", "There is problem staring game, please check " + launcher));
                    }
                }
                else
                {
                    messanger.Send(new ApplicationShowBalloonTip("ESL Tracker", "No installed launcher detected (BethesdaNet or Steam)."));
                }
            }
            else
            {
                messanger.Send(new ApplicationShowBalloonTip("ESL Tracker", "Game is already running"));
            }
            return proc;
        }

        private Process LaunchGameFromPlatform()
        {
            if (IsBethesdaLauncherInstalled())
            {
                return processWrapper.Start(BethesdaLaunch);
            }
            else if (IsSteamInstalled())
            {
                return processWrapper.Start(SteamLaunch);
            }
            else
            {
                return null;
            }
        }

    }
}
