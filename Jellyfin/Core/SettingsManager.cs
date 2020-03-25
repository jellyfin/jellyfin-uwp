using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public bool HasJellyfinServer => !String.IsNullOrEmpty(JellyfinServer);

        public String JellyfinServer
        {
            get => GetProperty<String>(SETTING_SERVER);
            set => SetProperty(SETTING_SERVER, value);
        }

        private void SetProperty(String propertyName, object value)
        {
            ContainerSettings.Values[propertyName] = value;
        }

        public T GetProperty<T>(String propertyName, T defaultValue = default(T))
        {
            var value = ContainerSettings.Values[propertyName];

            if (value != null)
            {
                return (T)value;
            }

            return defaultValue;
        }
    }
}
