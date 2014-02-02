// FFXIVAPP.Plugin.Widgets
// WinAPI.cs
//
// © 2013 ZAM Network LLC

using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using NLog;
using Talis.XIVPlugin.Twintania.Helpers;
using Talis.XIVPlugin.Twintania.Properties;

namespace Talis.XIVPlugin.Twintania.Interop
{
    public static class WinAPI
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

        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        private const int WsExTransparent = 0x00000020;
        private const int GwlExstyle = (-20);
        public const uint WineventOutofcontext = 0;
        public const uint EventSystemForeground = 3;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            var buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();
            return GetWindowText(handle, buff, nChars) > 0 ? buff.ToString() : "";
        }

        private static void SetWindowTransparent(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GwlExstyle);
            SetWindowLong(hwnd, GwlExstyle, extendedStyle | WsExTransparent);
        }

        private static void SetWindowLayered(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GwlExstyle);
            extendedStyle &= ~WsExTransparent;
            SetWindowLong(hwnd, GwlExstyle, extendedStyle);
        }

        public static void ToggleClickThrough(Window window)
        {
            try
            {
                var hWnd = new WindowInteropHelper(window).Handle;
                if (Settings.Default.TwintaniaWidgetClickThroughEnabled)
                {
                    SetWindowTransparent(hWnd);
                }
                else
                {
                    SetWindowLayered(hWnd);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(Logger, ex, LogLevel.Error);
            }
        }
    }
}