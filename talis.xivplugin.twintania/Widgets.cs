// talis.xivplugin.twintania
// Widgets.cs

using System;
using NLog;
using talis.xivplugin.twintania.Helpers;
using talis.xivplugin.twintania.Windows;

namespace talis.xivplugin.twintania
{
    public class Widgets
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

        private static Widgets _instance;
        private TwintaniaWidget _twintaniaWidget;

        public static Widgets Instance
        {
            get { return _instance ?? (_instance = new Widgets()); }
            set { _instance = value; }
        }

        public TwintaniaWidget TwintaniaWidget
        {
            get { return _twintaniaWidget ?? (_twintaniaWidget = new TwintaniaWidget()); }
        }

        public void ShowTwintaniaWidget()
        {
            try
            {
                TwintaniaWidget.Show();
            }
            catch (Exception ex)
            {
                LogHelper.Log(Logger, ex, LogLevel.Error);
            }
        }
    }
}
