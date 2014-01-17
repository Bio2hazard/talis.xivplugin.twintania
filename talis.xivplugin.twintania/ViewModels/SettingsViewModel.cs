// talis.xivplugin.twintania
// SettingsViewModel.cs

using System.Globalization;
using FFXIVAPP.Common.Models;
using FFXIVAPP.Common.ViewModelBase;
using NLog;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using talis.xivplugin.twintania.Helpers;
using talis.xivplugin.twintania.Properties;
using talis.xivplugin.twintania.Views;
using talis.xivplugin.twintania.Windows;

namespace talis.xivplugin.twintania.ViewModels
{
    internal sealed class SettingsViewModel : INotifyPropertyChanged
    {

        #region Property Bindings

        private static SettingsViewModel _instance;

        public static SettingsViewModel Instance
        {
            get { return _instance ?? (_instance = new SettingsViewModel()); }
        }

        #endregion

        #region Declarations

        public ICommand SaveDivebombTimersCommand { get; private set; }
        public ICommand LoadDivebombTimersCommand { get; private set; }
        public ICommand ResetDivebombTimersCommand { get; private set; }

        public ICommand SaveEnrageTimersCommand { get; private set; }
        public ICommand LoadEnrageTimersCommand { get; private set; }
        public ICommand ResetEnrageTimersCommand { get; private set; }

        public ICommand TwintaniaWidgetTestStartCommand { get; private set; }
        public ICommand TwintaniaWidgetTestStopCommand { get; private set; }

        public ICommand RefreshSoundListCommand { get; private set; }

        #endregion

        public SettingsViewModel()
        {
            SaveDivebombTimersCommand = new DelegateCommand(SaveDivebombTimers);
            LoadDivebombTimersCommand = new DelegateCommand(LoadDivebombTimers);
            ResetDivebombTimersCommand = new DelegateCommand(ResetDivebombTimers);

            SaveEnrageTimersCommand = new DelegateCommand(SaveEnrageTimers);
            LoadEnrageTimersCommand = new DelegateCommand(LoadEnrageTimers);
            ResetEnrageTimersCommand = new DelegateCommand(ResetEnrageTimers);

            TwintaniaWidgetTestStartCommand = new DelegateCommand(TwintaniaWidgetTestStart);
            TwintaniaWidgetTestStopCommand = new DelegateCommand(TwintaniaWidgetTestStop);

            RefreshSoundListCommand = new DelegateCommand(RefreshSoundList);
        }

        #region Loading Functions

        #endregion

        #region Utility Functions

        #endregion

        #region Command Bindings

        private static void RefreshSoundList()
        {
            Initializer.LoadAndCacheSounds();
        }

        public void SaveDivebombTimers()
        {
            double result;
            string message = "";

            if(Double.TryParse(SettingsView.View.TwintaniaWidgetDivebombTimeFastBox.Text, out result))
            {
                Settings.Default.TwintaniaWidgetDivebombTimeFast = result;
            }
            else
            {
                message += "Delay for Fast Divebombs is invalid ( " + SettingsView.View.TwintaniaWidgetDivebombTimeFastBox.Text + " )";
                SettingsView.View.TwintaniaWidgetDivebombTimeFastBox.Text = Settings.Default.TwintaniaWidgetDivebombTimeFast.ToString(CultureInfo.InvariantCulture);
            }

            if (Double.TryParse(SettingsView.View.TwintaniaWidgetDivebombTimeSlowBox.Text, out result))
            {
                Settings.Default.TwintaniaWidgetDivebombTimeSlow = result;
            }
            else
            {
                message += "Delay for Slow Divebombs is invalid ( " + SettingsView.View.TwintaniaWidgetDivebombTimeSlowBox.Text + " )";
                SettingsView.View.TwintaniaWidgetDivebombTimeSlowBox.Text = Settings.Default.TwintaniaWidgetDivebombTimeSlow.ToString(CultureInfo.InvariantCulture);
            }

            if (message.Length > 0)
            {
                var popupContent = new PopupContent
                {
                    Title = PluginViewModel.Instance.Locale["app_WarningMessage"],
                    Message = message
                };
                Plugin.PHost.PopupMessage(Plugin.PName, popupContent);
            }
        }

        public void LoadDivebombTimers()
        {
            SettingsView.View.TwintaniaWidgetDivebombTimeFastBox.Text = Settings.Default.TwintaniaWidgetDivebombTimeFast.ToString(CultureInfo.InvariantCulture);
            SettingsView.View.TwintaniaWidgetDivebombTimeSlowBox.Text = Settings.Default.TwintaniaWidgetDivebombTimeSlow.ToString(CultureInfo.InvariantCulture);
        }

        public void ResetDivebombTimers()
        {
            var settingsProperty = Settings.Default.Properties["TwintaniaWidgetDivebombTimeFast"];
            if (settingsProperty != null)
            {
                SettingsView.View.TwintaniaWidgetDivebombTimeFastBox.Text = settingsProperty.DefaultValue.ToString();
            }
            var property = Settings.Default.Properties["TwintaniaWidgetDivebombTimeSlow"];
            if (property != null)
            {
                SettingsView.View.TwintaniaWidgetDivebombTimeSlowBox.Text = property.DefaultValue.ToString();
            }
        }

        public void SaveEnrageTimers()
        {
            double result;
            string message = "";

            if (Double.TryParse(SettingsView.View.TwintaniaWidgetEnrageTimeBox.Text, out result))
            {
                Settings.Default.TwintaniaWidgetEnrageTime = result;
            }
            else
            {
                message += "Time for Enrage is invalid ( " + SettingsView.View.TwintaniaWidgetEnrageTimeBox.Text + " )";
                SettingsView.View.TwintaniaWidgetEnrageTimeBox.Text = Settings.Default.TwintaniaWidgetEnrageTime.ToString(CultureInfo.InvariantCulture);
            }

            if (message.Length > 0)
            {
                var popupContent = new PopupContent
                {
                    Title = PluginViewModel.Instance.Locale["app_WarningMessage"],
                    Message = message
                };
                Plugin.PHost.PopupMessage(Plugin.PName, popupContent);
            }
        }

        public void LoadEnrageTimers()
        {
            SettingsView.View.TwintaniaWidgetEnrageTimeBox.Text = Settings.Default.TwintaniaWidgetEnrageTime.ToString(CultureInfo.InvariantCulture);
        }

        public void ResetEnrageTimers()
        {
            var settingsProperty = Settings.Default.Properties["TwintaniaWidgetEnrageTime"];
            if (settingsProperty != null)
            {
                SettingsView.View.TwintaniaWidgetEnrageTimeBox.Text = settingsProperty.DefaultValue.ToString();
            }
        }

        public void TwintaniaWidgetTestStart()
        {
            TwintaniaWidgetViewModel.Instance.TestModeStart();
        }

        public void TwintaniaWidgetTestStop()
        {
            TwintaniaWidgetViewModel.Instance.TestModeStop();
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion
    }
}
