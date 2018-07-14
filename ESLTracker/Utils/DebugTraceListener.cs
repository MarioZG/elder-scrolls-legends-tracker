using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils
{
    public class DebugTraceListener : TraceListener
    {
        public override void Write(string message)
        {
        }

        public override void WriteLine(string message)
        {
            string[] errorFilter = { "{DisconnectedItem}'" };
            if (!errorFilter.Any(e => message.Contains(e)))
            {
                Debugger.Break();
            }
        }
    }
}
