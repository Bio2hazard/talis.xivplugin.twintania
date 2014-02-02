// talis.xivplugin.twintania
// SettingsViewModel.cs

using System.Globalization;
using FFXIVAPP.Common.Helpers;
using FFXIVAPP.Common.Models;
using FFXIVAPP.Common.ViewModelBase;
using NLog;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Talis.XIVPlugin.Twintania.Helpers;
using Talis.XIVPlugin.Twintania.Properties;
using Talis.XIVPlugin.Twintania.Views;
using Talis.XIVPlugin.Twintania.Windows;

namespace Talis.XIVPlugin.Twintania.ViewModels
{
    internal sealed class SettingsViewModel : INotifyPropertyChanged
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

        public ICommand TestDiveBombAlertCommand { get; private set; }

        public ICommand SaveEnrageTimersCommand { get; private set; }
        public ICommand LoadEnrageTimersCommand { get; private set; }
        public ICommand ResetEnrageTimersCommand { get; private set; }

        public ICommand TestEnrageAlertCommand { get; private set; }

        public ICommand SaveTwisterWarningTimerCommand { get; private set; }
        public ICommand LoadTwisterWarningTimerCommand { get; private set; }
        public ICommand ResetTwisterWarningTimerCommand { get; private set; }

        public ICommand TestTwisterAlertCommand { get; private set; }
        public ICommand TestTwisterWarningCommand { get; private set; }

        public ICommand TestTwisterWarningTimerCommand { get; private set; }

        public ICommand SaveDeathSentenceWarningTimerCommand { get; private set; }
        public ICommand LoadDeathSentenceWarningTimerCommand { get; private set; }
        public ICommand ResetDeathSentenceWarningTimerCommand { get; private set; }

        public ICommand TestDeathSentenceAlertCommand { get; private set; }
        public ICommand TestDeathSentenceWarningCommand { get; private set; }

        public ICommand TestDeathSentenceWarningTimerCommand { get; private set; }

        public ICommand TwintaniaWidgetTestStartCommand { get; private set; }
        public ICommand TwintaniaWidgetTestStopCommand { get; private set; }

        public ICommand RefreshSoundListCommand { get; private set; }

        #endregion

        public SettingsViewModel()
        {
            SaveDivebombTimersCommand = new DelegateCommand(SaveDivebombTimers);
            LoadDivebombTimersCommand = new DelegateCommand(LoadDivebombTimers);
            ResetDivebombTimersCommand = new DelegateCommand(ResetDivebombTimers);

            TestDiveBombAlertCommand = new DelegateCommand(TestDivebombAlert);

            SaveEnrageTimersCommand = new DelegateCommand(SaveEnrageTimers);
            LoadEnrageTimersCommand = new DelegateCommand(LoadEnrageTimers);
            ResetEnrageTimersCommand = new DelegateCommand(ResetEnrageTimers);

            TestEnrageAlertCommand = new DelegateCommand(TestEnrageAlert);

            SaveTwisterWarningTimerCommand = new DelegateCommand(SaveTwisterWarningTimer);
            LoadTwisterWarningTimerCommand = new DelegateCommand(LoadTwisterWarningTimer);
            ResetTwisterWarningTimerCommand = new DelegateCommand(ResetTwisterWarningTimer);

            TestTwisterAlertCommand = new DelegateCommand(TestTwisterAlert);
            TestTwisterWarningCommand = new DelegateCommand(TestTwisterWarning);

            TestTwisterWarningTimerCommand = new DelegateCommand(TestTwisterWarningTimer);

            SaveDeathSentenceWarningTimerCommand = new DelegateCommand(SaveDeathSentenceWarningTimer);
            LoadDeathSentenceWarningTimerCommand = new DelegateCommand(LoadDeathSentenceWarningTimer);
            ResetDeathSentenceWarningTimerCommand = new DelegateCommand(ResetDeathSentenceWarningTimer);

            TestDeathSentenceAlertCommand = new DelegateCommand(TestDeathSentenceAlert);
            TestDeathSentenceWarningCommand = new DelegateCommand(TestDeathSentenceWarning);

            TestDeathSentenceWarningTimerCommand = new DelegateCommand(TestDeathSentenceWarningTimer);

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

        public void TestDivebombAlert()
        {
            SoundPlayerHelper.PlayCached(Settings.Default.TwintaniaWidgetDivebombAlertFile, Settings.Default.TwintaniaWidgetDivebombVolume);
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

        public void TestEnrageAlert()
        {
            SoundPlayerHelper.PlayCached(Settings.Default.TwintaniaWidgetEnrageAlertFile, Settings.Default.TwintaniaWidgetEnrageVolume);
        }

        public void SaveTwisterWarningTimer()
        {
            double result;
            string message = "";

            if (Double.TryParse(SettingsView.View.TwintaniaWidgetTwisterWarningTimeBox.Text, out result))
            {
                Settings.Default.TwintaniaWidgetTwisterWarningTime = result;
            }
            else
            {
                message += "Time for Twister is invalid ( " + SettingsView.View.TwintaniaWidgetTwisterWarningTimeBox.Text + " )";
                SettingsView.View.TwintaniaWidgetTwisterWarningTimeBox.Text = Settings.Default.TwintaniaWidgetTwisterWarningTime.ToString(CultureInfo.InvariantCulture);
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

        public void LoadTwisterWarningTimer()
        {
            SettingsView.View.TwintaniaWidgetTwisterWarningTimeBox.Text = Settings.Default.TwintaniaWidgetTwisterWarningTime.ToString(CultureInfo.InvariantCulture);
        }

        public void ResetTwisterWarningTimer()
        {
            var settingsProperty = Settings.Default.Properties["TwintaniaWidgetTwisterWarningTime"];
            if (settingsProperty != null)
            {
                SettingsView.View.TwintaniaWidgetTwisterWarningTimeBox.Text = settingsProperty.DefaultValue.ToString();
            }
        }

        public void TestTwisterAlert()
        {
            SoundPlayerHelper.PlayCached(Settings.Default.TwintaniaWidgetTwisterAlertFile, Settings.Default.TwintaniaWidgetTwisterAlertVolume);
        }
        public void TestTwisterWarning()
        {
            SoundPlayerHelper.PlayCached(Settings.Default.TwintaniaWidgetTwisterWarningFile, Settings.Default.TwintaniaWidgetTwisterWarningVolume);
        }

        public void TestTwisterWarningTimer()
        {
            TwintaniaWidgetViewModel.Instance.TriggerTwister();
        }

        public void SaveDeathSentenceWarningTimer()
        {
            double result;
            string message = "";

            if (Double.TryParse(SettingsView.View.TwintaniaWidgetDeathSentenceWarningTimeBox.Text, out result))
            {
                Settings.Default.TwintaniaWidgetDeathSentenceWarningTime = result;
            }
            else
            {
                message += "Time for Death Sentence is invalid ( " + SettingsView.View.TwintaniaWidgetDeathSentenceWarningTimeBox.Text + " )";
                SettingsView.View.TwintaniaWidgetDeathSentenceWarningTimeBox.Text = Settings.Default.TwintaniaWidgetDeathSentenceWarningTime.ToString(CultureInfo.InvariantCulture);
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

        public void LoadDeathSentenceWarningTimer()
        {
            SettingsView.View.TwintaniaWidgetDeathSentenceWarningTimeBox.Text = Settings.Default.TwintaniaWidgetDeathSentenceWarningTime.ToString(CultureInfo.InvariantCulture);
        }

        public void ResetDeathSentenceWarningTimer()
        {
            var settingsProperty = Settings.Default.Properties["TwintaniaWidgetDeathSentenceWarningTime"];
            if (settingsProperty != null)
            {
                SettingsView.View.TwintaniaWidgetDeathSentenceWarningTimeBox.Text = settingsProperty.DefaultValue.ToString();
            }
        }

        public void TestDeathSentenceAlert()
        {
            SoundPlayerHelper.PlayCached(Settings.Default.TwintaniaWidgetDeathSentenceAlertFile, Settings.Default.TwintaniaWidgetDeathSentenceAlertVolume);
        }
        public void TestDeathSentenceWarning()
        {
            SoundPlayerHelper.PlayCached(Settings.Default.TwintaniaWidgetDeathSentenceWarningFile, Settings.Default.TwintaniaWidgetDeathSentenceWarningVolume);
        }

        public void TestDeathSentenceWarningTimer()
        {
            TwintaniaWidgetViewModel.Instance.TriggerDeathSentence();
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
