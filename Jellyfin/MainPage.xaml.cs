using System;
using Windows.UI.Xaml.Controls;
using Windows.Gaming.Input;
using Windows.UI.Core;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Jellyfin
{
    public sealed partial class MainPage : Page
    {
        private Gamepad _gamepad = null;

        public MainPage()
        {
            InitializeComponent();

            Gamepad.GamepadAdded += Gamepad_GamepadAdded;
            Gamepad.GamepadRemoved += Gamepad_GamepadRemoved;
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
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