using Jellyfin.Core;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Gaming.Input;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.System;
using Windows.System.Threading; // Ensure correct DispatcherPriority usage

namespace Jellyfin.Controls
{
    public sealed partial class JellyfinWebView : UserControl, IDisposable
    {
        private List<Gamepad> _connectedGamepads = new List<Gamepad>();
        private readonly DispatcherTimer _gamepadPollingTimer;
        private bool _wasBPressed;
        private readonly Stopwatch _buttonTimer = new Stopwatch();
        private const int ButtonPressCooldownMs = 250; // Time-based button press handling
        private readonly DispatcherQueue _dispatcherQueue;

        public JellyfinWebView()
        {
            this.InitializeComponent();

            // Get current DispatcherQueue for UI thread access
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            // Set WebView source
            WView.Source = new Uri(Central.Settings.JellyfinServer);

            WView.CoreWebView2Initialized += WView_CoreWebView2Initialized;
            WView.NavigationCompleted += JellyfinWebView_NavigationCompleted;
            SystemNavigationManager.GetForCurrentView().BackRequested += Back_BackRequested;

            // Handle Gamepad events
            Gamepad.GamepadAdded += (sender, e) => { if (!_connectedGamepads.Contains(e)) _connectedGamepads.Add(e); };
            Gamepad.GamepadRemoved += (sender, e) => { _connectedGamepads.Remove(e); };

            var ss = new DispatcherTimer();

            // Initialize and start DispatcherTimer
            _gamepadPollingTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(10),
            };
            _gamepadPollingTimer.Tick += GamepadPollingTimer_Tick;
            _gamepadPollingTimer.Start();

            _buttonTimer.Start(); // Start timing button presses
        }

        private void GamepadPollingTimer_Tick(object sender, object e)
        {
            if (_dispatcherQueue.HasThreadAccess)
            {
                ProcessGamepadInput();
            }
            else
            {
                _dispatcherQueue.TryEnqueue(DispatcherQueuePriority.High, ProcessGamepadInput);
            }
        }

        private void ProcessGamepadInput()
        {
            foreach (var gamepad in _connectedGamepads)
            {
                GamepadReading reading = gamepad.GetCurrentReading();

                bool isBPressed = (reading.Buttons & GamepadButtons.B) == GamepadButtons.B;

                // Ensure time-based button press detection
                if (isBPressed && !_wasBPressed && _buttonTimer.ElapsedMilliseconds >= ButtonPressCooldownMs)
                {
                    if (WView.CanGoBack)
                    {
                        WView.GoBack();
                    }
                    _wasBPressed = true;
                    _buttonTimer.Restart(); // Reset cooldown timer
                }
                else if (!isBPressed)
                {
                    _wasBPressed = false;
                }
            }
        }

        private void WView_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            WView.CoreWebView2.ContainsFullScreenElementChanged += JellyfinWebView_ContainsFullScreenElementChanged;
        }

        private void Back_BackRequested(object sender, BackRequestedEventArgs args)
        {
            if (WView.CanGoBack)
            {
                WView.GoBack();
            }
            args.Handled = true;
        }

        private async void JellyfinWebView_NavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            if (!args.IsSuccess)
            {
                // Handle a navigation failure to the server
                CoreWebView2WebErrorStatus errorStatus = args.WebErrorStatus;
                MessageDialog md = new MessageDialog($"Navigation failed: {errorStatus}");
                await md.ShowAsync();
            }

            await WView.ExecuteScriptAsync("navigator.gamepadInputEmulation = 'mouse';");
        }

        private void JellyfinWebView_ContainsFullScreenElementChanged(CoreWebView2 sender, object args)
        {
            ApplicationView appView = ApplicationView.GetForCurrentView();

            if (sender.ContainsFullScreenElement)
            {
                appView.TryEnterFullScreenMode();
                return;
            }

            if (appView.IsFullScreenMode)
            {
                appView.ExitFullScreenMode();
            }
        }

        // Dispose method to clean up resources
        public void Dispose()
        {
            if (_gamepadPollingTimer != null)
            {
                _gamepadPollingTimer.Stop();
                _gamepadPollingTimer.Tick -= GamepadPollingTimer_Tick;
            }
        }
    }
}
