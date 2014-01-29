// talis.xivplugin.twintania
// ShellViewModel.cs

using NLog;
using System;
using System.ComponentModel;
using System.Windows;
using talis.xivplugin.twintania.Helpers;
using talis.xivplugin.twintania.Interop;
using talis.xivplugin.twintania.Properties;

namespace talis.xivplugin.twintania
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
