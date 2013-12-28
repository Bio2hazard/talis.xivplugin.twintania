// FFXIVAPP.Plugin.Widgets
// WidgetTopMostHelper.cs
//
// © 2013 ZAM Network LLC

using FFXIVAPP.Common.Helpers;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using talis.xivplugin.twintania.Interop;
using talis.xivplugin.twintania.Properties;
using talis.xivplugin.twintania.Windows;

namespace talis.xivplugin.twintania.Helpers
{
    public static class WidgetTopMostHelper
    {
        private static WinAPI.WinEventDelegate _delegate;
        private static IntPtr _mainHandleHook;

        private static Timer SetWindowTimer { get; set; }

        public static void HookWidgetTopMost()
        {
            try
            {
                _delegate = BringWidgetsIntoFocus;
                _mainHandleHook = WinAPI.SetWinEventHook(WinAPI.EVENT_SYSTEM_FOREGROUND, WinAPI.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, _delegate, 0, 0, WinAPI.WINEVENT_OUTOFCONTEXT);
            }
            catch (Exception e)
            {
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
                var stayOnTop = WinAPI.GetActiveWindowTitle()
                                      .ToUpper()
                                      .StartsWith("FINAL FANTASY XIV");
                // If any of the widgets are focused, don't try to hide any of them, or it'll prevent us from moving/closing them
                if (handle == new WindowInteropHelper(Widgets.Instance.TwintaniaHPWidget).Handle)
                {
                    return;
                }
                if (Settings.Default.ShowTwintaniaHPWidgetOnLoad)
                {
                    // Override to keep the Widget on top during test mode
                    if (TwintaniaHPWidgetViewModel.Instance.ForceTop)
                        stayOnTop = true;

                    ToggleTopMost(Widgets.Instance.TwintaniaHPWidget, stayOnTop, force);
                }
            }
            catch (Exception ex)
            {
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
