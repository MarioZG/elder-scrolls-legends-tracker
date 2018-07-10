using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.GameClient
{
    public interface ILauncherService
    {
        Task<Process> StartGame();
        bool IsSteamInstalled();
        bool IsBethesdaLauncherInstalled();
    }
}
