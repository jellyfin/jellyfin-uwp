using Jellyfin.Models;
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

        #endregion
    }
}
