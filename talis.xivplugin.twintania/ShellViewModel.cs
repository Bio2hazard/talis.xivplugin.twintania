// talis.xivplugin.twintania
// ShellViewModel.cs

using FFXIVAPP.Common.Utilities;
using NLog;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using talis.xivplugin.twintania.Interop;
using talis.xivplugin.twintania.Properties;

namespace talis.xivplugin.twintania
{
    public sealed class ShellViewModel : INotifyPropertyChanged
    {
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
            Initializer.LoadSounds();
            Initializer.SetupWidgetTopMost();
            Settings.Default.PropertyChanged += DefaultOnPropertyChanged;
        }

        internal static void Loaded(object sender, RoutedEventArgs e)
        {
            ShellView.View.Loaded -= Loaded;
        }

        private static void DefaultOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var propertyName = propertyChangedEventArgs.PropertyName;
            switch (propertyName)
            {
                case "TwintaniaHPWidgetUIScale":
                    try
                    {
                        Settings.Default.TwintaniaHPWidgetWidth = (int) (250 * Double.Parse(Settings.Default.TwintaniaHPWidgetUIScale));
                        Settings.Default.TwintaniaHPWidgetHeight = (int) (450 * Double.Parse(Settings.Default.TwintaniaHPWidgetUIScale));
                    }
                    catch(Exception ex)
                    {
                        Settings.Default.TwintaniaHPWidgetWidth = 250;
                        Settings.Default.TwintaniaHPWidgetHeight = 450;
                    }
                    break;

                case "TwintaniaHPWidgetClickThroughEnabled":
                    WinAPI.ToggleClickThrough(Widgets.Instance.TwintaniaHPWidget);
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

        private void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }

        #endregion
    }
}
