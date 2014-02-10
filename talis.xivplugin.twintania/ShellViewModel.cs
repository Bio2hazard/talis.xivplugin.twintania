// Talis.XIVPlugin.Twintania
// ShellViewModel.cs
// 
// 	

using System;
using System.ComponentModel;
using System.Windows;
using NLog;
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
                if (FFXIVAPP.Common.Constants.EnableNLog || Settings.Default.TwintaniaWidgetAdvancedLogging)
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
            LogHelper.ToggleAdvancedLogging();
            Initializer.LoadAndCacheSounds();
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
                    catch (Exception ex)
                    {
                        LogHelper.Log(Logger, ex, LogLevel.Error);
                        Settings.Default.TwintaniaWidgetWidth = 250;
                        Settings.Default.TwintaniaWidgetHeight = 450;
                    }
                    break;

                case "TwintaniaWidgetClickThroughEnabled":
                    WinAPI.ToggleClickThrough(Widgets.Instance.TwintaniaWidget);
                    break;
                case "TwintaniaWidgetAdvancedLogging":
                    LogHelper.ToggleAdvancedLogging();
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
