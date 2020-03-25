using Jellyfin.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Jellyfin.Controls
{
    public sealed partial class JellyfinWebView : UserControl
    {
        public JellyfinWebView()
        {
            this.InitializeComponent();

            WView.ContainsFullScreenElementChanged += JellyfinWebView_ContainsFullScreenElementChanged;
            WView.NavigationCompleted += JellyfinWebView_NavigationCompleted;
            WView.NavigationFailed += WView_NavigationFailed;

            SystemNavigationManager.GetForCurrentView().BackRequested += Back_BackRequested;
            this.Loaded += JellyfinWebView_Loaded;            
        }

        private async void WView_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            MessageDialog md = new MessageDialog("Navigation failed");
            await md.ShowAsync();
        }

        private void JellyfinWebView_Loaded(object sender, RoutedEventArgs e)
        {
            WView.Navigate(new Uri(Central.Settings.JellyfinServer));
        }

        private void Back_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (WView.CanGoBack)
            {
                WView.GoBack();
            }
            e.Handled = true;
        }

        private async void JellyfinWebView_NavigationCompleted(Windows.UI.Xaml.Controls.WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            await WView.InvokeScriptAsync("eval", new string[] { "navigator.gamepadInputEmulation = 'gamepad';" });
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
    }
}
