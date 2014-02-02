// FFXIVAPP.Plugin.Widgets
// WidgetTopMostHelper.cs
//
// © 2013 ZAM Network LLC

using System.Linq;
using FFXIVAPP.Common.RegularExpressions;
using FFXIVAPP.Common.Helpers;
using System;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using NLog;
using Talis.XIVPlugin.Twintania.Interop;
using Talis.XIVPlugin.Twintania.Properties;
using Talis.XIVPlugin.Twintania.Windows;

namespace Talis.XIVPlugin.Twintania.Helpers
{
    public static class WidgetTopMostHelper
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

        private static WinAPI.WinEventDelegate _delegate;
        private static IntPtr _mainHandleHook;

        private static WindowInteropHelper _twintaniaInteropHelper;

        private static WindowInteropHelper TwintaniaInteropHelper
        {
            get { return _twintaniaInteropHelper ?? (_twintaniaInteropHelper = new WindowInteropHelper(Widgets.Instance.TwintaniaWidget)); }
        }

        private static Timer SetWindowTimer { get; set; }

        public static void HookWidgetTopMost()
        {
            try
            {
                _delegate = BringWidgetsIntoFocus;
                _mainHandleHook = WinAPI.SetWinEventHook(WinAPI.EventSystemForeground, WinAPI.EventSystemForeground, IntPtr.Zero, _delegate, 0, 0, WinAPI.WineventOutofcontext);
            }
            catch (Exception ex)
            {
                LogHelper.Log(Logger, ex, LogLevel.Error);
            }
            SetWindowTimer = new Timer(1000);
            SetWindowTimer.Elapsed += SetWindowTimerOnElapsed;
            SetWindowTimer.Start();
        }

        private static void SetWindowTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            DispatcherHelper.Invoke(() => BringWidgetsIntoFocus(), DispatcherPriority.Normal);
        }

        private static void BringWidgetsIntoFocus(IntPtr hwineventhook, uint eventtype, IntPtr hwnd, int idobject, int idchild, uint dweventthread, uint dwmseventtime)
        {
            BringWidgetsIntoFocus(true);
        }

        private static void BringWidgetsIntoFocus(bool force = false)
        {
            try
            {
                var handle = WinAPI.GetForegroundWindow();
                var activeTitle = WinAPI.GetActiveWindowTitle();

                bool stayOnTop = System.Windows.Application.Current.Windows.OfType<Window>().Any(w => w.Title == activeTitle) 
                                 || Regex.IsMatch(activeTitle.ToUpper(), @"^(FINAL FANTASY XIV)", SharedRegEx.DefaultOptions);

                // If any of the widgets are focused, don't try to hide any of them, or it'll prevent us from moving/closing them
                if (handle == TwintaniaInteropHelper.Handle)
                {
                    return;
                }
                if (Settings.Default.ShowTwintaniaWidgetOnLoad)
                {
                    // Override to keep the Widget on top during test mode
                    if (TwintaniaWidgetViewModel.Instance.ForceTop)
                        stayOnTop = true;

                    ToggleTopMost(Widgets.Instance.TwintaniaWidget, stayOnTop, force);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(Logger, ex, LogLevel.Error);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="window"></param>
        /// <param name="stayOnTop"></param>
        /// <param name="force"></param>
        private static void ToggleTopMost(Window window, bool stayOnTop, bool force)
        {
            if (window.Topmost && stayOnTop && !force)
            {
                return;
            }
            window.Topmost = false;
            if (!stayOnTop)
            {
                if (window.IsVisible)
                {
                    window.Hide();
                }
                return;
            }
            window.Topmost = true;
            if (!window.IsVisible)
            {
                window.Show();
            }
        }
    }
}
