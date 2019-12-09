using System;
using System.IO;
using System.Net;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace jellyfin_uwp
{
    /// <summary>
    /// App settings page
    /// </summary>
    public sealed partial class AppSettings : Page
    {

        const string STRING_ERR_URLINVALID = "That URL is invalid";
        const string STRING_ERR_URLBAD = "A web server couldn't be found at that URL";
        const string STRING_ERR_URLNOTJF = "That URL doesn't point to Jellyfin";

        public AppSettings()
        {
            this.InitializeComponent();

            AppURL.Text = MainPage.globalSettingsStore.AppURL;
            CheckURLValidAsync(AppURL.Text);
        }

        private void SettingsChanged(object sender, object args)
        {
            MainPage.globalSettingsStore.AppURL = AppURL.Text;
            CheckURLValidAsync(AppURL.Text);
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            MainPage.jfFrame.Navigate(typeof(WebView));
        }

        private async void CheckURLValidAsync(string uriString)
        {
            MainPage.globalSettingsStore.AppURLValid = false;
            MainPage.globalSettingsStore.AppURLIsJellyfin = false;
            DoneButton.IsEnabled = false;
            ErrorText.Text = string.Empty;
            ErrorText.Visibility = Visibility.Collapsed;

            // also do a check for valid url
            if (!Uri.IsWellFormedUriString(AppURL.Text, UriKind.Absolute))
            {
                ErrorText.Text = STRING_ERR_URLINVALID;
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            Uri testUri = new Uri(AppURL.Text);

            // check scheme
            if (testUri.Scheme != "http" && testUri.Scheme != "https")
            {
                ErrorText.Text = STRING_ERR_URLINVALID;
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            // check URL exists
            HttpWebRequest request;
            HttpWebResponse response;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(testUri);
                response = (HttpWebResponse)(await request.GetResponseAsync());
            } catch (Exception)
            {
                ErrorText.Text = STRING_ERR_URLBAD;
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                ErrorText.Text = STRING_ERR_URLBAD;
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            MainPage.globalSettingsStore.AppURLValid = true;

            // also do a check that url points to Jellyfin web client
            if (!response.Headers.ToString().Contains("Emby"))
            {
                ErrorText.Text = STRING_ERR_URLNOTJF;
                ErrorText.Visibility = Visibility.Visible;
            }

            MainPage.globalSettingsStore.AppURLIsJellyfin = true;
            ErrorText.Visibility = Visibility.Collapsed;
            DoneButton.IsEnabled = true;
        }
    }
}
