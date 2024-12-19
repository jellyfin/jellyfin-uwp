using Jellyfin.Core;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using Windows.Gaming.Input;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Jellyfin.Controls
{
    public sealed partial class JellyfinWebView : UserControl
    {
        private List<Gamepad> _connectedGamepads = new List<Gamepad>();
        private DispatcherTimer _timer;
        private bool _wasBPressed; // used to make the back button latching
        
        public JellyfinWebView()
        {
            this.InitializeComponent();
            // Kicks off the CoreWebView2 creation
            WView.Source = new Uri(Central.Settings.JellyfinServer);

            WView.CoreWebView2Initialized += WView_CoreWebView2Initialized;
            WView.NavigationCompleted += JellyfinWebView_NavigationCompleted;
            SystemNavigationManager.GetForCurrentView().BackRequested += Back_BackRequested;
            
            Gamepad.GamepadAdded += GamepadOnGamepadAdded;
            Gamepad.GamepadRemoved += GamepadOnGamepadRemoved;
            
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(10);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }
        
        private void GamepadOnGamepadRemoved(object sender, Gamepad e)
        {
            _connectedGamepads.Remove(e);
        }

        private void GamepadOnGamepadAdded(object sender, Gamepad e)
        {
            if (!_connectedGamepads.Contains(e))
            {
                _connectedGamepads.Add(e);
            }        
        }

        private void Timer_Tick(object sender, object e)
        {
            foreach (var gamepad in _connectedGamepads)
            {
                GamepadReading reading = gamepad.GetCurrentReading();

                if ((reading.Buttons & GamepadButtons.B) == GamepadButtons.B && !_wasBPressed)
                {
                    // Handle B button pressed
                    if (WView.CanGoBack)
                    {
                        WView.GoBack();
                        _wasBPressed = true;
                    }
                }

                if ((reading.Buttons & GamepadButtons.B) != GamepadButtons.B)
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
                // Log the error or show an error page
                MessageDialog md = new MessageDialog($"Navigation failed {errorStatus}");
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

            if (!appView.IsFullScreenMode)
            {
                return;
            }

            appView.ExitFullScreenMode();
        }
    }
}
