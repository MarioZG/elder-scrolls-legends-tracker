using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils.NLog
{
    public class NLogLoggerProxy<T> : ILogger
    {
        private readonly Logger logger = LogManager.GetLogger(typeof(T).FullName);

        public void Log(TraceLevel logLevel, Exception exception, string message, params object[] args)
        {
            LogLevel nlogLogLevel;
            switch (logLevel)
            {
                case TraceLevel.Off:
                    nlogLogLevel = LogLevel.Off;
                    break;
                case TraceLevel.Error:
                    nlogLogLevel = LogLevel.Error;
                    break;
                case TraceLevel.Warning:
                    nlogLogLevel = LogLevel.Warn;
                    break;
                case TraceLevel.Info:
                    nlogLogLevel = LogLevel.Info;
                    break;
                case TraceLevel.Verbose:
                    nlogLogLevel = LogLevel.Debug;
                    break;
                default:
                    throw new NotImplementedException($"Unknown logLevel passed ({logLevel}");
            }
            logger.Log(nlogLogLevel, exception, message, args);
        }
    }
}
