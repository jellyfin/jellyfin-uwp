namespace Jellyfin.Services.Interfaces
{
    public interface ISettingsService
    {
        bool Any(string propertyName);

        void DeleteProperty(string propertyName);

        void Clear();

        void SetProperty(string propertyName, object value);

        T GetProperty<T>(string propertyName, T defaultValue = default(T));
    }
}
