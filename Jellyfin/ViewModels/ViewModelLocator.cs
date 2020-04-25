using Jellyfin.Core;
using Jellyfin.Services.Interfaces;
using Unity;

namespace Jellyfin.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary> 
    public class ViewModelLocator
    {
        #region Properties

        /// <summary>
        /// Reference for Unity Container.
        /// </summary>
        private IUnityContainer _container;

        #endregion

        #region ctor

        public ViewModelLocator()
        {
            ConfigureViewModelMappings();
        }

        private void ConfigureViewModelMappings()
        {
            _container = Globals.Instance.Container;

            IMovieService movieService = _container.Resolve<IMovieService>();
            ILoginService loginService = _container.Resolve<ILoginService>();
            ISettingsService settingsService = _container.Resolve<ISettingsService>();
            IJellyfinNavigationService navigationService = _container.Resolve<IJellyfinNavigationService>();

            _container.RegisterInstance(new MainViewModel());
            _container.RegisterInstance(new MovieDetailViewModel());
            _container.RegisterInstance(new MovieListViewModel(movieService));
            _container.RegisterInstance(new MediaPlaybackViewModel());
            _container.RegisterInstance(new LoginViewModel(loginService, settingsService));
            _container.RegisterInstance(new MenuViewModel(settingsService, navigationService));
        }

        #endregion

        #region Additional methods

        /// <summary>
        /// Mapping for Main Page - Main View model.
        /// </summary>
        public MainViewModel MainPage
        {
            get => _container.Resolve<MainViewModel>();
        }

        /// <summary>
        /// Mapping for Movie Details Page - Movie Details View model.
        /// </summary>
        public MovieDetailViewModel MovieDetailPage
        {
            get => _container.Resolve<MovieDetailViewModel>();
        }

        /// <summary>
        /// Mapping for Movie List Page - Movie list View Model.
        /// </summary>
        public MovieListViewModel MovieListPage
        {
            get => _container.Resolve<MovieListViewModel>();
        }

        /// <summary>
        /// Mapping for Media Playback Page - Media Playback View Model.
        /// </summary>
        public MediaPlaybackViewModel MediaPlaybackPage
        {
            get => _container.Resolve<MediaPlaybackViewModel>();
        }

        /// <summary>
        /// Mapping for Login Page - Login View Model.
        /// </summary>
        public LoginViewModel LoginPage
        {
            get => _container.Resolve<LoginViewModel>();
        }

        /// <summary>
        /// Mapping for Menu Page => Menu View Model.
        /// </summary>
        public MenuViewModel MenuPage
        {
            get => _container.Resolve<MenuViewModel>();
        }

        #endregion
    }
}
