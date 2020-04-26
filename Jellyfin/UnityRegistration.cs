using Jellyfin.Core;
using Jellyfin.Models;
using Jellyfin.Models.Adapters;
using Jellyfin.Services;
using Jellyfin.Services.Interfaces;
using Unity;

namespace Jellyfin
{
    /// <summary>
    /// Provides access to registered instances used by this application.
    /// </summary>
    public sealed class UnityRegistration
    {
        /// <summary>
        /// Registers type mappings for Unity.
        /// </summary>
        public static void RegisterTypes()
        {
            IUnityContainer container = Globals.Instance.Container;
            
            container.RegisterType<IJellyfinNavigationService, JellyfinNavigationService>();

            container.RegisterType<IAdapter<Item, Movie>, MovieAdapter>();
            container.RegisterType<IAdapter<MovieDetailsResult, MovieDetail>, MovieDetailAdapter>();

            container.RegisterType<IMovieService, MovieService>();
            container.RegisterType<ILoginService, LoginService>();
            container.RegisterType<IImageService, ImageService>();
            container.RegisterType<ISettingsService, SettingsService>();
        }
    }
}
