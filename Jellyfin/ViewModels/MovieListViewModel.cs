using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Models;
using Jellyfin.Services.Interfaces;
using Jellyfin.Views;

namespace Jellyfin.ViewModels
{
    /// <summary>
    /// View model for listing the movies available.
    /// </summary>
    public class MovieListViewModel : JellyfinViewModelBase
    {
        #region Properties

        #region Movies

        public ObservableCollection<Movie> Movies { get; set; }
            = new ObservableCollection<Movie>();

        #endregion

        /// <summary>
        /// Reference for the movie service.
        /// </summary>
        private readonly IMovieService _movieService;

        #endregion

        #region ctor

        public MovieListViewModel(IMovieService movieService)
        {
            _movieService = movieService ??
                throw new ArgumentNullException(nameof(movieService));
        }

        #endregion

        #region Additional methods
        
        /// <summary>
        /// Loads all the movies available.
        /// </summary>
        public async Task Load()
        {
            IList<Movie> movies = (await _movieService.GetMovies()).ToList();
            foreach (Movie movie in movies.OrderBy(q => q.Name))
            {
                Movies.Add(movie);
            }
        }

        #endregion

        public void NavigateToMovie(Movie movie)
        {
            NavigationService.Navigate(typeof(MovieDetailView), movie);
        }
    }
}