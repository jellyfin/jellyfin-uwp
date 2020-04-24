using Jellyfin.Core;
using Jellyfin.Services;
using Jellyfin.ViewModels;
using Unity;
using Unity.Lifetime;

namespace Jellyfin
{
    /// <summary>
    /// Provides access to the view model classes used by this application.
    /// </summary>
    public sealed class UnityRegistration
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="RegisterTypes"/> class.
        /// </summary>
        public static void RegisterTypes()
        {
            var container = Globals.Instance.Container;
            
            container.RegisterType<IJellyfinNavigationService, JellyfinNavigationService>();
        }
    }
}
