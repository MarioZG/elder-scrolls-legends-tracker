using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ESLTracker.Controls
{
    public class ClickableUrl : Hyperlink
    {
        protected override void OnClick()
        {
            if (this.NavigateUri != null)
            {
                Process.Start(this.NavigateUri.ToString());
            }
        }
    }
}
