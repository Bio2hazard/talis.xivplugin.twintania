// talis.xivplugin.twintania
// ShellViewModel.cs

using NLog;
using System;
using System.ComponentModel;
using System.Windows;
using NLog.Config;
using NLog.Targets;
using Talis.XIVPlugin.Twintania.Helpers;
using Talis.XIVPlugin.Twintania.Interop;
using Talis.XIVPlugin.Twintania.Properties;

namespace Talis.XIVPlugin.Twintania
{
    public sealed class ShellViewModel : INotifyPropertyChanged
    {
        #region Logger
        private static Logger _logger;
        private static Logger Logger
        {
            get
            {
                if (FFXIVAPP.Common.Constants.EnableNLog)
                {
                    return _logger ?? (_logger = LogManager.GetCurrentClassLogger());
                }
                return null;
            }
        }
        #endregion

        #region Property Bindings

        private static ShellViewModel _instance;

        public static ShellViewModel Instance
        {
            get { return _instance ?? (_instance = new ShellViewModel()); }
        }

        #endregion

        #region Declarations

        #endregion

        public ShellViewModel()
        {
            Initializer.LoadSettings();
            Initializer.SetupWidgetTopMost();
            Settings.Default.PropertyChanged += DefaultOnPropertyChanged;
        }

        internal static void Loaded(object sender, RoutedEventArgs e)
        {
            ShellView.View.Loaded -= Loaded;
            Initializer.LoadAndCacheSounds();

            // WIP: Debug Logging for Twintania Plugin
            /*
            FileTarget fileTarget = new FileTarget();
            LogManager.Configuration.AddTarget("file", fileTarget);

            fileTarget.FileName = "${basedir}/Logs/TwintaniaErrors/TwintaniaPlugin-${date:format=yyyy-MM-dd}.log";
            fileTarget.Layout = "${longdate} ${level} ${logger} ${message} Ex:${exception:innerFormat=ToString:maxInnerExceptionLevel=15:format=ToString}";
            LoggingRule fileRule = new LoggingRule("Talis.XIVPlugin.Twintania*", LogLevel.Debug, fileTarget);
            LogManager.Configuration.LoggingRules.Add(fileRule);

            LogManager.ReconfigExistingLoggers();
             */
        }

        private static void DefaultOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var propertyName = propertyChangedEventArgs.PropertyName;
            switch (propertyName)
            {
                case "TwintaniaWidgetUIScale":
                    try
                    {
                        Settings.Default.TwintaniaWidgetWidth = (int) (250 * Double.Parse(Settings.Default.TwintaniaWidgetUIScale));
                        Settings.Default.TwintaniaWidgetHeight = (int) (450 * Double.Parse(Settings.Default.TwintaniaWidgetUIScale));
                    }
                    catch(Exception ex)
                    {
                        LogHelper.Log(Logger, ex, LogLevel.Error);
                        Settings.Default.TwintaniaWidgetWidth = 250;
                        Settings.Default.TwintaniaWidgetHeight = 450;
                    }
                    break;

                case "TwintaniaWidgetClickThroughEnabled":
                    WinAPI.ToggleClickThrough(Widgets.Instance.TwintaniaWidget);
                    break;
            }
        }

        #region Loading Functions

        #endregion

        #region Utility Functions

        #endregion

        #region Command Bindings

        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion
    }
}
