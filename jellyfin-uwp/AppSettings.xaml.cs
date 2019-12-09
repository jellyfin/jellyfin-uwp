using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
