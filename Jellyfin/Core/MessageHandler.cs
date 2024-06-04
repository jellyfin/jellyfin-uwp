using Jellyfin.Component;
using Jellyfin.Utils;
using Jellyfin.Views;
using System.Diagnostics;
using Windows.Data.Json;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
namespace Jellyfin.Core
{
    public class MessageHandler : IMessageHandler
    {
        private readonly Frame frame;
        private readonly FullScreenManager fullScreenManager;
        public MessageHandler(Frame frame)
        {
            this.frame = frame;
            fullScreenManager = new FullScreenManager();
        }
        public async void HandleJsonNotification(JsonObject json)
        {
            string eventType = json.GetNamedString("type");
            JsonObject args = json.GetNamedObject("args");

            if (eventType == "enableFullscreen")
            {
                await fullScreenManager.EnableFullscreenAsync(args);
            }
            else if (eventType == "disableFullscreen")
            {
                fullScreenManager.DisableFullScreen();
            }
            else if (eventType == "selectServer")
            {
                Central.Settings.JellyfinServer = null;
                _ = frame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    frame.Navigate(typeof(OnBoarding));
                });
            }
            else if (eventType == "openClientSettings")
            {
                _ = frame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    frame.Navigate(typeof(Settings));
                });
            }
            else if (eventType == "exit")
            {
                Exit();
            }
            else
            {
                Debug.WriteLine($"Unexpected JSON message: {eventType}");
            }
        }
        private void Exit()
        {
            Application.Current.Exit();
        }
    }
}