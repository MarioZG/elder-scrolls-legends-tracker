using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Services
{
    public interface IWinDialogs
    {
        string SaveFileDialog(string defaultFielName, string filter, bool addExtension);
    }
}
