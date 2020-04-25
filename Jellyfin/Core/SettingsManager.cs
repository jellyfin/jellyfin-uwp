using Windows.Storage;

namespace Jellyfin.Core
{
    public class SettingsManager
    {
        string CONTAINER_SETTINGS = "APPSETTINGS";
        string SETTING_SERVER = "SERVER";

        private ApplicationDataContainer LocalSettings => ApplicationData.Current.LocalSettings;
        private ApplicationDataContainer ContainerSettings
        {
            get
            {
                if (!LocalSettings.Containers.ContainsKey(CONTAINER_SETTINGS))
                {
                    LocalSettings.CreateContainer(CONTAINER_SETTINGS, ApplicationDataCreateDisposition.Always);
                }
                return LocalSettings.Containers[CONTAINER_SETTINGS];
            }
        }

        public bool HasJellyfinServer => !string.IsNullOrEmpty(JellyfinServer);

        public string JellyfinServer
        {
            get => GetProperty<string>(SETTING_SERVER);
            set => SetProperty(SETTING_SERVER, value);
        }

        private void SetProperty(string propertyName, object value)
        {
            ContainerSettings.Values[propertyName] = value;
        }

        public T GetProperty<T>(string propertyName, T defaultValue = default(T))
        {
            object value = ContainerSettings.Values[propertyName];

            if (value != null)
            {
                return (T)value;
            }

            return defaultValue;
        }
    }
}
