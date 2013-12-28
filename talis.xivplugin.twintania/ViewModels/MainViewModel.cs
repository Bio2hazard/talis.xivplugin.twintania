// talis.xivplugin.twintania
// MainViewModel.cs

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FFXIVAPP.Common.ViewModelBase;
using talis.xivplugin.twintania.Properties;

namespace talis.xivplugin.twintania.ViewModels
{
    internal sealed class MainViewModel : INotifyPropertyChanged
    {
        #region Property Bindings

        private static MainViewModel _instance;

        public static MainViewModel Instance
        {
            get { return _instance ?? (_instance = new MainViewModel()); }
        }

        #endregion

        #region Declarations

        public ICommand ResetTwintaniaHPWidgetCommand { get; private set; }
        public ICommand OpenTwintaniaHPWidgetCommand { get; private set; }

        #endregion

        #region Loading Functions

        #endregion

        public MainViewModel()
        {
            ResetTwintaniaHPWidgetCommand = new DelegateCommand(ResetTwintaniaHPWidget);
            OpenTwintaniaHPWidgetCommand = new DelegateCommand(OpenTwintaniaHPWidget);
        }

        #region Utility Functions

        #endregion

        #region Command Bindings

        public void ResetTwintaniaHPWidget()
        {
            Settings.Default.TwintaniaHPWidgetTop = 100;
            Settings.Default.TwintaniaHPWidgetLeft = 100;
        }

        public void OpenTwintaniaHPWidget()
        {
            Settings.Default.ShowTwintaniaHPWidgetOnLoad = true;
            Widgets.Instance.ShowTwintaniaHPWidget();
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
