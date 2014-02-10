// Talis.XIVPlugin.Twintania
// LogHelper.cs
// 
// 	

using System;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using Talis.XIVPlugin.Twintania.Properties;

namespace Talis.XIVPlugin.Twintania.Helpers
{
    public static class LogHelper
    {
        #region Logger

        private static Logger _logger;

        private static Logger Logger
        {
            get
            {
                if (FFXIVAPP.Common.Constants.EnableNLog || Settings.Default.TwintaniaWidgetAdvancedLogging)
                {
                    return _logger ?? (_logger = LogManager.GetCurrentClassLogger());
                }
                return null;
            }
        }

        #endregion

        private static FileTarget _twintaniaFileTarget;
        private static LoggingRule _twintaniaLoggingRule;

        static LogHelper()
        {
            LogManager.Configuration.AddTarget("twintania", TwintaniaFileTarget);
            LogManager.ReconfigExistingLoggers();
        }

        private static FileTarget TwintaniaFileTarget
        {
            get
            {
                if (_twintaniaFileTarget == null)
                {
                    var layout = new CsvLayout();
                    _twintaniaFileTarget = new FileTarget
                    {
                        FileName = "${basedir}/Logs/TwintaniaDebug/TwintaniaPlugin-${date:format=yyyy-MM-dd}.log",
                        Layout = "${longdate}|${level}|${pad:padCharacter= :padding=-33:fixedLength=false:inner=${replace:searchFor=Talis.XIVPlugin.Twintania.:wholeWords=false:replaceWith=:ignoreCase=false:regex=false:inner=${logger}}}|${message}|${exception:innerFormat=ToString:maxInnerExceptionLevel=15:format=ToString}"
                    };
                }
                return _twintaniaFileTarget;
            }
        }

        private static LoggingRule TwintaniaLoggingRule
        {
            get { return _twintaniaLoggingRule ?? (_twintaniaLoggingRule = new LoggingRule("Talis.XIVPlugin.Twintania*", LogLevel.Debug, TwintaniaFileTarget)); }
        }

        public static void ToggleAdvancedLogging()
        {
            if (Settings.Default.TwintaniaWidgetAdvancedLogging)
            {
                LogManager.Configuration.LoggingRules.Add(TwintaniaLoggingRule);
                Log(Logger, "Advanced Logging Enabled", LogLevel.Debug);
            }
            else
            {
                LogManager.Configuration.LoggingRules.Remove(TwintaniaLoggingRule);
            }
        }

        public static void Log(Logger logger, string message, LogLevel level = null)
        {
            if (logger == null)
            {
                return;
            }

            if (level == null)
            {
                level = LogLevel.Trace;
            }

            logger.Log(level, message);
        }

        public static void Log(Logger logger, Exception exception, LogLevel level = null)
        {
            if (logger == null)
            {
                return;
            }

            if (level == null)
            {
                level = LogLevel.Error;
            }

            logger.Log(level, exception);
        }

        public static void Log(Logger logger, string message, Exception exception, LogLevel level = null)
        {
            if (logger == null)
            {
                return;
            }

            if (level == null)
            {
                level = LogLevel.Error;
            }

            logger.Log(level, message, exception);
        }
    }
}
