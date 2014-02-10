// Talis.XIVPlugin.Twintania
// AboutViewModel.cs
// 
// 	

using System;
using System.ComponentModel;
using System.Diagnostics;
using FFXIVAPP.Common.Models;
using FFXIVAPP.Common.ViewModelBase;

namespace Talis.XIVPlugin.Twintania.ViewModels
{
    internal sealed class AboutViewModel : INotifyPropertyChanged
    {
        #region Property Bindings

        private static AboutViewModel _instance;

        public static AboutViewModel Instance
        {
            get { return _instance ?? (_instance = new AboutViewModel()); }
        }

        #endregion

        #region Declarations

        public DelegateCommand OpenGuildWebSiteCommand { get; private set; }
        public DelegateCommand OpenGithubCommand { get; private set; }

        #endregion

        public AboutViewModel()
        {
            OpenGuildWebSiteCommand = new DelegateCommand(OpenGuildWebSite);
            OpenGithubCommand = new DelegateCommand(OpenGithub);
        }

        #region Loading Functions

        #endregion

        #region Utility Functions

        #endregion

        #region Command Bindings

        public void OpenGuildWebSite()
        {
            try
            {
                Process.Start("http://trickplay.chapterfain.com/");
            }
            catch (Exception ex)
            {
                var popupContent = new PopupContent
                {
                    Title = PluginViewModel.Instance.Locale["app_WarningMessage"],
                    Message = ex.Message
                };
                Plugin.PHost.PopupMessage(Plugin.PName, popupContent);
            }
        }

        public void OpenGithub()
        {
            try
            {
                Process.Start("https://github.com/Bio2hazard/talis.xivplugin.twintania");
            }
            catch (Exception ex)
            {
                var popupContent = new PopupContent
                {
                    Title = PluginViewModel.Instance.Locale["app_WarningMessage"],
                    Message = ex.Message
                };
                Plugin.PHost.PopupMessage(Plugin.PName, popupContent);
            }
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion
    }
}
