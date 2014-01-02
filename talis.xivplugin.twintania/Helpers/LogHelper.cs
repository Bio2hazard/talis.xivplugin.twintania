using NLog;
using System;

namespace talis.xivplugin.twintania.Helpers
{
    public static class LogHelper
    {
        public static void Log(Logger logger, string message, LogLevel level = null)
        {
            if (logger == null)
                return;

            if (level == null)
                level = LogLevel.Trace;

            logger.Log(level, message);
        }

        public static void Log(Logger logger, Exception exception, LogLevel level = null)
        {
            if (logger == null)
                return;

            if (level == null)
                level = LogLevel.Error;

            logger.Log(level, exception);
        }

        public static void Log(Logger logger, string message, Exception exception, LogLevel level = null)
        {
            if (logger == null)
                return;

            if (level == null)
                level = LogLevel.Error;

            logger.Log(level, message, exception);
        }
    }
}
