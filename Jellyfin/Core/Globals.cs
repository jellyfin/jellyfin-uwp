using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Jellyfin.Models;
using Jellyfin.Services.Interfaces;
using Newtonsoft.Json;
using Unity;

namespace Jellyfin.Core
{
    public class Globals
    {
        #region Singleton

        private static Globals _instance;

        private Globals()
        {
            Container = new UnityContainer();
        }
        
        public static Globals Instance => _instance ?? (_instance = new Globals());

        #endregion

        #region Properties

        /// <summary>
        /// Reference for the unity container.
        /// </summary>
        public IUnityContainer Container { get; set; }

        public const string AuthorizationHeader = "X-Emby-Authorization";

        public string Client
        {
            get { return "Jellyfin Xbox"; }
        }

        public string DeviceName
        {
            get { return "XboxOne"; }
        }

        public string DeviceId
        {
            get { return "abcd1234TODO"; }
        }

        public string Version
        {
            get { return "1.0~b1"; }
        }

        public string AccessToken { get; set; }

        public string Host { get; set; }

        public string AuthorizationValue
        {
            get
            {
                string val = 
                    $"MediaBrowser Client=\"{Client}\", Device=\"{DeviceName}\", DeviceId=\"{DeviceId}\", Version=\"{Version}\"";

                if (!string.IsNullOrEmpty(AccessToken))
                {
                    val += $", Token=\"{AccessToken}\"";
                }

                return val;
            }
        }

        public string ServerId { get; set; }

        public Sessioninfo SessionInfo { get; set; }

        public User User { get; set; }

        public CoreDispatcher UIDispatcher
        {
            get => CoreApplication.MainView.CoreWindow.Dispatcher;
        }

        #endregion

        #region Additional methods

        public void LoadSettings(ISettingsService settingsService)
        {
            bool toClearCache = false;

            foreach (string mandatoryItem in new [] {nameof(Host), nameof(AccessToken), nameof(ServerId), nameof(SessionInfo), nameof(User)})
            {
                if (!settingsService.Any(mandatoryItem))
                {
                    toClearCache = true;
                    break;
                }

                string item = settingsService.GetProperty<string>(mandatoryItem);

                if (string.IsNullOrEmpty(item))
                {
                    toClearCache = true;
                    break;
                }
            }

            if (toClearCache)
            {
                settingsService.Clear();
                return;
            }

            Host = settingsService.GetProperty<string>(nameof(Host));
            AccessToken = settingsService.GetProperty<string>(nameof(AccessToken));
            ServerId = settingsService.GetProperty<string>(nameof(ServerId));
            string sessionInfoJson = settingsService.GetProperty<string>(nameof(SessionInfo));
            SessionInfo = JsonConvert.DeserializeObject<Sessioninfo>(sessionInfoJson);

            string userJson = settingsService.GetProperty<string>(nameof(User));
            User = JsonConvert.DeserializeObject<User>(userJson);
        }

        public void SaveSettings(ISettingsService settingsService)
        {
            settingsService.SetProperty(nameof(Host), Host);
            settingsService.SetProperty(nameof(AccessToken), AccessToken);
            settingsService.SetProperty(nameof(ServerId), ServerId);
            settingsService.SetProperty(nameof(SessionInfo), JsonConvert.SerializeObject(SessionInfo));
            settingsService.SetProperty(nameof(User), JsonConvert.SerializeObject(User));
        }

        #endregion
    }
}
