using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ESLTracker.Utils.Behaviors
{
    public class DragDropEventInfo
    {
        public object Data { get; set; }
        public UIElement Source { get; set; }
    }
}
