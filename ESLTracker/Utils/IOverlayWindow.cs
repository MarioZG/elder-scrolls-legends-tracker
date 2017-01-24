using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils
{
    public interface IOverlayWindow
    {
        void UpdateVisibilty(bool isGameActive, bool isMainWIndowActive, bool isOtherWindowActive);
        bool ShowOnScreen { get; set; }
    }
}
