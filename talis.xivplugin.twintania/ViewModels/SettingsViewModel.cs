// talis.xivplugin.twintania
// SettingsViewModel.cs

using FFXIVAPP.Common.Events;
using FFXIVAPP.Common.Models;
using FFXIVAPP.Common.Utilities;
using FFXIVAPP.Common.ViewModelBase;
using NLog;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
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

        public ICommand TwintaniaHPWidgetTestStartCommand { get; private set; }
        public ICommand TwintaniaHPWidgetTestStopCommand { get; private set; }

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

            TwintaniaHPWidgetTestStartCommand = new DelegateCommand(TwintaniaHPWidgetTestStart);
            TwintaniaHPWidgetTestStopCommand = new DelegateCommand(TwintaniaHPWidgetTestStop);

            RefreshSoundListCommand = new DelegateCommand(RefreshSoundList);
        }

        #region Loading Functions

        #endregion

        #region Utility Functions

        #endregion

        #region Command Bindings

        private static void RefreshSoundList()
        {
            Initializer.LoadSounds();
        }

        public void SaveDivebombTimers()
        {
            double result;
            string message = "";

            if(Double.TryParse(SettingsView.View.TwintaniaHPWidgetDivebombTimeFastBox.Text, out result))
            {
                Settings.Default.TwintaniaHPWidgetDivebombTimeFast = result;
            }
            else
            {
                message += "Delay for Fast Divebombs is invalid ( " + SettingsView.View.TwintaniaHPWidgetDivebombTimeFastBox.Text.ToString() + " )";
                SettingsView.View.TwintaniaHPWidgetDivebombTimeFastBox.Text = Settings.Default.TwintaniaHPWidgetDivebombTimeFast.ToString();
            }

            if (Double.TryParse(SettingsView.View.TwintaniaHPWidgetDivebombTimeSlowBox.Text, out result))
            {
                Settings.Default.TwintaniaHPWidgetDivebombTimeSlow = result;
            }
            else
            {
                message += "Delay for Slow Divebombs is invalid ( " + SettingsView.View.TwintaniaHPWidgetDivebombTimeSlowBox.Text.ToString() + " )";
                SettingsView.View.TwintaniaHPWidgetDivebombTimeSlowBox.Text = Settings.Default.TwintaniaHPWidgetDivebombTimeSlow.ToString();
            }

            if (message.Length > 0)
            {
                MessageBox.Show(message, "Invalid Time Specified");
            }
        }

        public void LoadDivebombTimers()
        {
            SettingsView.View.TwintaniaHPWidgetDivebombTimeFastBox.Text = Settings.Default.TwintaniaHPWidgetDivebombTimeFast.ToString();
            SettingsView.View.TwintaniaHPWidgetDivebombTimeSlowBox.Text = Settings.Default.TwintaniaHPWidgetDivebombTimeSlow.ToString();
        }

        public void ResetDivebombTimers()
        {
            SettingsView.View.TwintaniaHPWidgetDivebombTimeFastBox.Text = Settings.Default.Properties["TwintaniaHPWidgetDivebombTimeFast"].DefaultValue.ToString();
            SettingsView.View.TwintaniaHPWidgetDivebombTimeSlowBox.Text = Settings.Default.Properties["TwintaniaHPWidgetDivebombTimeSlow"].DefaultValue.ToString();
        }

        public void SaveEnrageTimers()
        {
            double result;
            string message = "";

            if (Double.TryParse(SettingsView.View.TwintaniaHPWidgetEnrageTimeBox.Text, out result))
            {
                Settings.Default.TwintaniaHPWidgetEnrageTime = result;
            }
            else
            {
                message += "Time for Enrage is invalid ( " + SettingsView.View.TwintaniaHPWidgetEnrageTimeBox.Text.ToString() + " )";
                SettingsView.View.TwintaniaHPWidgetEnrageTimeBox.Text = Settings.Default.TwintaniaHPWidgetEnrageTime.ToString();
            }

            if (message.Length > 0)
            {
                MessageBox.Show(message, "Invalid Time Specified");
            }
        }

        public void LoadEnrageTimers()
        {
            SettingsView.View.TwintaniaHPWidgetEnrageTimeBox.Text = Settings.Default.TwintaniaHPWidgetEnrageTime.ToString();
        }

        public void ResetEnrageTimers()
        {
            SettingsView.View.TwintaniaHPWidgetEnrageTimeBox.Text = Settings.Default.Properties["TwintaniaHPWidgetEnrageTime"].DefaultValue.ToString();
        }

        public void TwintaniaHPWidgetTestStart()
        {
            TwintaniaHPWidgetViewModel.Instance.TestModeStart();
        }

        public void TwintaniaHPWidgetTestStop()
        {
            TwintaniaHPWidgetViewModel.Instance.TestModeStop();
        }

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
