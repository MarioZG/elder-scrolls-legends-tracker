using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Services
{
    public interface ILauncherService
    {

        Task<Process> StartGame(IWinAPI winApi, IMessenger messanger);
        bool IsSteamInstalled();
        bool IsBethesdaLauncherInstalled();
    }
}
