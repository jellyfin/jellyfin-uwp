using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace jellyfin_uwp
{
    /// <summary>
    /// Webview page
    /// </summary>
    public sealed partial class WebView : Page
    {
        public WebView()
        {
            this.InitializeComponent();

            JellyfinWebView.ContainsFullScreenElementChanged += JellyfinWebView_ContainsFullScreenElementChanged;

            JellyfinWebView.Navigate(new Uri(MainPage.globalSettingsStore.AppURL));
        }

        private void JellyfinWebView_ContainsFullScreenElementChanged(Windows.UI.Xaml.Controls.WebView sender, object args)
        {
            ApplicationView appView = ApplicationView.GetForCurrentView();

            if (sender.ContainsFullScreenElement)
            {
                appView.TryEnterFullScreenMode();
                return;
            }

            if (!appView.IsFullScreenMode)
            {
                return;
            }

            appView.ExitFullScreenMode();
        }

        private void ChangeURL_Click(object sender, RoutedEventArgs e)
        {
            MainPage.jfFrame.Navigate(typeof(AppSettings));
        }
    }
}
