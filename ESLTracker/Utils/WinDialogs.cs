using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils
{
    public class WinDialogs : IWinDialogs
    {
        public string SaveFileDialog(string defaultFielName, string filter, bool addExtension)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = defaultFielName;
            sfd.Filter = filter;
            sfd.AddExtension = addExtension;

            bool? result = sfd.ShowDialog(); 

            if (result.HasValue && result.Value)
            {
                return sfd.FileName;
            }
            else
            {
                return null;
            }
        }
    }
}
