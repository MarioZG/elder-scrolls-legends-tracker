using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ESLTracker.Utils.SystemWindowsWrappers
{
    public class ClipboardWrapper : IClipboardWrapper
    {

        public void SetText(string data)
        {
            Clipboard.SetText(data);
        }
    }
}
