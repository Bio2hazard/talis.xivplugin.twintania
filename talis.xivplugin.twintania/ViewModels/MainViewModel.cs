// Talis.XIVPlugin.Twintania
// MainViewModel.cs
// 
// 	

using System.ComponentModel;
using System.Windows.Input;
using FFXIVAPP.Common.ViewModelBase;
using Talis.XIVPlugin.Twintania.Properties;

namespace Talis.XIVPlugin.Twintania.ViewModels
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

        public ICommand ResetTwintaniaWidgetCommand { get; private set; }
        public ICommand OpenTwintaniaWidgetCommand { get; private set; }

        #endregion

        #region Loading Functions

        #endregion

        public MainViewModel()
        {
            ResetTwintaniaWidgetCommand = new DelegateCommand(ResetTwintaniaWidget);
            OpenTwintaniaWidgetCommand = new DelegateCommand(OpenTwintaniaWidget);
        }

        #region Utility Functions

        #endregion

        #region Command Bindings

        public void ResetTwintaniaWidget()
        {
            Settings.Default.TwintaniaWidgetTop = 100;
            Settings.Default.TwintaniaWidgetLeft = 100;
        }

        public void OpenTwintaniaWidget()
        {
            Settings.Default.ShowTwintaniaWidgetOnLoad = true;
            Widgets.Instance.ShowTwintaniaWidget();
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion
    }
}
