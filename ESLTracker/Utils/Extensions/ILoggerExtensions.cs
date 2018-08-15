using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils.Extensions
{
    public static class ILoggerExtensions
    {
        public static void Debug(this ILogger logger, string message)
        {
            logger.Log(TraceLevel.Verbose, null, message, null);
        }

        public static void Trace(this ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.Log(TraceLevel.Verbose, exception,  message, args);
        }

        public static void Info(this ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.Log(TraceLevel.Info, null, message, args);
        }

        public static void Error(this ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.Log(TraceLevel.Error, null, message, args);
        }


        public static void Info(this ILogger logger, string message, params object[] args)
        {
            Info(logger, null, message, args);
        }


        public static void Trace(this ILogger logger, string message, params object[] args)
        {
            Trace(logger, null, message, args);
        }

        public static void Trace(this ILogger logger, string message)
        {
            Trace(logger, null, message, null);
        }


        public static void Error(this ILogger logger, Exception exception)
        {
            Error(logger, exception, null, null);
        }
    }
}
