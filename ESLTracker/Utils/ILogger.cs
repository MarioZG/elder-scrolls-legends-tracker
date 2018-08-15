using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils
{
    /// <summary>
    /// https://stackoverflow.com/questions/5646820/logger-wrapper-best-practice
    /// </summary>
    public interface ILogger
    {
        void Log(TraceLevel logLevel, Exception exception, string message, params object[] args);
    }
}
