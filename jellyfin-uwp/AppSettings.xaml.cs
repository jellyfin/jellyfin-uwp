using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace jellyfin_uwp
{
    /// <summary>
    /// App settings page
    /// </summary>
    public sealed partial class AppSettings : Page
    {
        public AppSettings()
        {
            this.InitializeComponent();

            AppURL.Text = MainPage.globalSettingsStore.AppURL;
        }

        private void SettingsChanged(object sender, object args)
        {
            MainPage.globalSettingsStore.AppURL = AppURL.Text;
            // also do a check for valid url
            // also do a check that url points to Jellyfin web client?
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            MainPage.jfFrame.Navigate(typeof(WebView));
        }
    }
}
