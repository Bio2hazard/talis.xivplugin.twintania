// talis.xivplugin.twintania
// Widgets.cs

using System;
using talis.xivplugin.twintania.Windows;

namespace talis.xivplugin.twintania
{
    public class Widgets
    {
        private static Widgets _instance;
        private TwintaniaHPWidget _twintaniaHPWidget;

        public static Widgets Instance
        {
            get { return _instance ?? (_instance = new Widgets()); }
            set { _instance = value; }
        }

        public TwintaniaHPWidget TwintaniaHPWidget
        {
            get { return _twintaniaHPWidget ?? (_twintaniaHPWidget = new TwintaniaHPWidget()); }
        }

        public void ShowTwintaniaHPWidget()
        {
            try
            {
                TwintaniaHPWidget.Show();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
