using Jellyfin.Component;
using Jellyfin.Core;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Graphics.Display.Core;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.WebUI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Jellyfin.Controls
{
    public sealed partial class JellyfinWebView : UserControl
    {
        public JellyfinWebView()
        {
            this.InitializeComponent();

            WView.NavigationStarting += JellyfinWebView_NavigationStarting;
            WView.NavigationCompleted += JellyfinWebView_NavigationCompleted;
            WView.NavigationFailed += WView_NavigationFailed;

            SystemNavigationManager.GetForCurrentView().BackRequested += Back_BackRequested;
            this.Loaded += JellyfinWebView_Loaded;

            HdmiDisplayInformation hdmiDisplayInformation = HdmiDisplayInformation.GetForCurrentView();
            if (hdmiDisplayInformation != null)
            {
                hdmiDisplayInformation.DisplayModesChanged += OnDisplayModeChanged;
            }
        }
        private void JellyfinWebView_NavigationStarting(object sender, WebViewNavigationStartingEventArgs e)
        {
            MessageHandler messageHandler = new MessageHandler(Window.Current.Content as Frame);
            WebViewBridge webViewBridge = new WebViewBridge(messageHandler);
            WView.AddWebAllowedObject("WebViewBridge", webViewBridge);
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

        private async Task RedirectLogs()
        {
            Uri uri = new Uri("ms-appx:///Resources/redirectlogs.js");
            StorageFile storageFile = await StorageFile.GetFileFromApplicationUriAsync(uri);
            string redirectLogsJs = await FileIO.ReadTextAsync(storageFile);
            await WView.InvokeScriptAsync("eval", new string[] { redirectLogsJs });
        }

        private async void JellyfinWebView_NavigationCompleted(Windows.UI.Xaml.Controls.WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (Debugger.IsAttached)
            {
                try
                {
                    await RedirectLogs();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to Redirect WebView Logs", ex);
                }
            }

            await InjectNativeShellScript();

            await WView.InvokeScriptAsync("eval", new string[] { "navigator.gamepadInputEmulation = 'mouse';" });
        }

        private async Task InjectNativeShellScript()
        {
            string nativeShellScript = await NativeShellScriptLoader.LoadNativeShellScript();
            try
            {
                await WView.InvokeScriptAsync("eval", new string[] { nativeShellScript });
                Debug.WriteLine("Injected nativeShellScript");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to add NativeShell JS", ex);
            }
        }

        private async void OnDisplayModeChanged(HdmiDisplayInformation sender, object args)
        {
            await WView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                Debug.WriteLine("Display mode has changed.");
                await InjectNativeShellScript();
            });
        }
    }
}
