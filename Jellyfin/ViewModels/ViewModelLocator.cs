using Jellyfin.Core;
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

        private readonly IUnityContainer container;

        #endregion

        public ViewModelLocator()
        {
            container = Globals.Instance.Container;

            container.RegisterInstance(new MainViewModel());
            container.RegisterInstance(new MovieDetailViewModel());
            container.RegisterInstance(new MovieViewModel());
            container.RegisterInstance(new MediaPlaybackViewModel());
        }

        public MainViewModel MainPage
        {
            get { return container.Resolve<MainViewModel>(); }
        }

        public MovieDetailViewModel MovieDetailPage
        {
            get { return container.Resolve<MovieDetailViewModel>(); }
        }

        public MovieViewModel MoviePage
        {
            get { return container.Resolve<MovieViewModel>(); }
        }

        public MediaPlaybackViewModel MediaPlaybackPage
        {
            get { return container.Resolve<MediaPlaybackViewModel>(); }
        }
    }
}
