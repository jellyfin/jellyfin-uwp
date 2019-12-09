using System;
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

            JellyfinWebView.Navigate(new Uri(MainPage.globalSettingsStore.AppURL));
        }

        private void ChangeURL_Click(object sender, RoutedEventArgs e)
        {
            MainPage.jfFrame.Navigate(typeof(AppSettings));
        }
    }
}
