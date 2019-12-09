using Windows.Storage;

namespace jellyfin_uwp
{
    public class SettingsStore
    {

        private ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        public string AppURL {
            get => (string)(_localSettings.Values["settings.app.AppURL"] ?? string.Empty);
            set => _localSettings.Values["settings.app.AppURL"] = value;
        }

        public bool AppURLValid {
            get => (bool)(_localSettings.Values["settings.app.AppURLValid"] ?? false);
            set => _localSettings.Values["settings.app.AppURLValid"] = value;
        }

        public bool AppURLIsJellyfin {
            get => (bool)(_localSettings.Values["settings.app.AppURLIsJellyfin"] ?? false);
            set => _localSettings.Values["settings.app.AppURLIsJellyfin"] = value;
        }

    }
}
