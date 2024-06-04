using Windows.Data.Json;
namespace Jellyfin.Component
{
    public interface IMessageHandler
    {
        void HandleJsonNotification(JsonObject jsonObject);
    }
}
