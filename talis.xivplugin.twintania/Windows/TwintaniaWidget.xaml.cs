// talis.xivplugin.twintania
// TwintaniaWidget.xaml.cs

using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using talis.xivplugin.twintania.Interop;
using talis.xivplugin.twintania.Properties;

namespace talis.xivplugin.twintania.Windows
{
    /// <summary>
    ///     Interaction logic for TwintaniaWidget.xaml
    /// </summary>
    public partial class TwintaniaWidget
    {
        public static TwintaniaWidget View;

        public TwintaniaWidget()
        {
            View = this;
            InitializeComponent();
            View.SourceInitialized += delegate { WinAPI.ToggleClickThrough(this); };
        }

        private void TitleBar_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void WidgetClose_OnClick(object sender, RoutedEventArgs e)
        {
            Settings.Default.ShowTwintaniaWidgetOnLoad = false;
            Close();
        }

        private void TalisWidget_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
