using Microsoft.UI.Xaml.Controls;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Gaming.Input;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Jellyfin
{
    public sealed partial class MainPage : Page
    {
        private Gamepad _gamepad = null;

        public MainPage()
        {
            InitializeComponent();

            WebView2.CoreWebView2Initialized += OnCoreWebView2Initialized;

            Gamepad.GamepadAdded += Gamepad_GamepadAdded;
            Gamepad.GamepadRemoved += Gamepad_GamepadRemoved;
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
        }

        private void OnCoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            if (args.Exception == null)
            {
                AddDeviceFormToUserAgent(sender);
            }
        }

        private void AddDeviceFormToUserAgent(WebView2 sender)
        {
            string userAgent = sender.CoreWebView2.Settings.UserAgent;
            // WebView2's UserAgent doesn't contain the device type so jellyfin-web can't detect Xbox
            string deviceForm = AnalyticsInfo.DeviceForm;
            // An "Unknown" device adds no value
            if (!userAgent.Contains(deviceForm) && deviceForm != "Unknown")
            {
                // "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36 Edg/123.0.0.0"
                // becomes
                // "Mozilla/5.0 (Windows NT 10.0; Win64; x64; Xbox Series X) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36 Edg/123.0.0.0"
                const string toReplace = ")";
                string userAgentWithDeviceForm = new Regex(Regex.Escape(toReplace))
                    .Replace(userAgent, "; " + deviceForm + toReplace, 1);
                sender.CoreWebView2.Settings.UserAgent = userAgentWithDeviceForm;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is string url)
            {
                WebView2.Source = new Uri(url);
            }
        }

        private void Gamepad_GamepadAdded(object sender, Gamepad e)
        {
            if (_gamepad == null)
            {
                _gamepad = e;
                Task.Run(() => GamepadInputLoop());
            }
        }

        private void Gamepad_GamepadRemoved(object sender, Gamepad e)
        {
            if (_gamepad == e)
            {
                _gamepad = null;
            }
        }

        private async void GamepadInputLoop()
        {
            while (_gamepad != null)
            {
                var reading = _gamepad.GetCurrentReading();

                if (reading.Buttons.HasFlag(GamepadButtons.A))
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await WebView2.ExecuteScriptAsync("document.activeElement.click();");
                    });
                }
                else if (reading.Buttons.HasFlag(GamepadButtons.B))
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        if (WebView2.CanGoBack) WebView2.GoBack();
                    });
                }

                if (reading.Buttons.HasFlag(GamepadButtons.DPadUp))
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await WebView2.ExecuteScriptAsync("navigateFocusableElements('prev');");
                    });
                }
                else if (reading.Buttons.HasFlag(GamepadButtons.DPadDown))
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await WebView2.ExecuteScriptAsync("navigateFocusableElements('next');");
                    });
                }

                await Task.Delay(100); // Delay to prevent rapid firing
            }
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            // Handle key inputs if needed
        }
    }
}