using System.Linq;
using Windows.Storage;
using Jellyfin.Services.Interfaces;

namespace Jellyfin.Services
{
    public class SettingsService : ISettingsService
    {
        #region Properties

        string CONTAINER_SETTINGS = "APPSETTINGS";

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

        #endregion

        #region Additional methods

        public void Clear()
        {
            ContainerSettings.Values.Clear();
        }

        public void DeleteProperty(string propertyName)
        {
            ContainerSettings.Values.Remove(propertyName);
        }

        public void SetProperty(string propertyName, object value)
        {
            ContainerSettings.Values[propertyName] = value;
        }

        public bool Any(string propertyName)
        {
            return ContainerSettings.Values.Any(q => q.Key == propertyName);
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

        #endregion
    }
}
