using System;
using System.Threading.Tasks;
using Windows.System;
using Jellyfin.Models;
using Jellyfin.Services.Interfaces;
using Jellyfin.Views;

namespace Jellyfin.ViewModels
{
    public class MovieDetailViewModel : JellyfinViewModelBase
    {
        #region Properties

        #region SelectedMovie

        private MovieDetail _selectedMovie;

        public MovieDetail SelectedMovie
        {
            get { return _selectedMovie; }
            set
            {
                _selectedMovie = value;
                RaisePropertyChanged(nameof(SelectedMovie));
            }
        }

        #endregion

        /// <summary>
        /// Reference for the movie service.
        /// </summary>
        private readonly IMovieService _movieService;

        #endregion

        #region ctor

        public MovieDetailViewModel(IMovieService movieService)
        {
            _movieService = movieService ??
                    throw new ArgumentNullException(nameof(movieService));
        }

        #endregion

        #region Additional methods

        protected override void Execute(string commandParameter)
        {
            switch (commandParameter)
            {
                case "Play":
                    Play();
                    break;
                default:
                    base.Execute(commandParameter);
                    break;
            }
        }

        public async Task GetMovieDetails(Movie movie)
        {
            SelectedMovie = await _movieService.GetMovieDetails(movie.Id);
        }

        private void Play()
        {
            NavigationService.Navigate(typeof(MediaPlaybackView), SelectedMovie);
        }

        #endregion

        public bool HandleKeyPressed(VirtualKey key)
        {
            switch (key)
            {
                case VirtualKey.Escape:
                    NavigationService.GoBack();
                    return true;
                default:
                    return false;
            }
        }
    }
}