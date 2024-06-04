using System;
using System.Diagnostics;
using Windows.Data.Json;
using Windows.Foundation.Metadata;

namespace Jellyfin.Component
{
    /**
    * Workaround for not being able to call window.external.notify
    * from a non-https & non-whitelisted uri
    * See https://stackoverflow.com/a/60301805 for more info
    */
    [AllowForWeb]
    public sealed class WebViewBridge
    {
        private readonly IMessageHandler messageHandler;

        public WebViewBridge(IMessageHandler messageHandler)
        {
            this.messageHandler = messageHandler;
        }

        public void PostMessage(string message)
        {
            try
            {
                if (JsonObject.TryParse(message, out JsonObject jsonObject))
                {
                    messageHandler.HandleJsonNotification(jsonObject);
                }
                else
                {
                    Debug.WriteLine($"Failed to parse message as JSON: {message}");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to process PostMessage with message: {message}", e);
            }
        }

        public void LogMessage(string message)
        {
            Debug.WriteLine(message);
        }
    }
}