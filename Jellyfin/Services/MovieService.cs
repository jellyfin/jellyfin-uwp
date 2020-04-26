using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Jellyfin.Core;
using Jellyfin.Extensions;
using Jellyfin.Models;
using Jellyfin.Models.Adapters;
using Jellyfin.Services.Interfaces;
using Newtonsoft.Json;

namespace Jellyfin.Services
{
    public class MovieService : ServiceBase, IMovieService
    {
        #region Properties

        /// <summary>
        /// Queue for downloading movie images.
        /// </summary>
        public TaskQueue<Movie> MovieImageDownloadQueue { get; set; }

        public string ListMoviesEndpoint
        {
            get =>
                $"{Globals.Instance.Host}/Users/{Globals.Instance.User.Id}/Items?IncludeItemTypes=Movie&Recursive=true&Fields=PrimaryImageAspectRatio%2CMediaSourceCount%2CBasicSyncInfo&ImageTypeLimit=1&EnableImageTypes=Primary%2CBackdrop%2CBanner%2CThumb&StartIndex=0&Limit=100000"
            ;
        }

        public string GetMovieDetailsEndpoint
        {
            get => $"{Globals.Instance.Host}/Users/{Globals.Instance.User.Id}/Items/";
        }

        public string GetRelatedMoviesEndpoint
        {
            get => $"{Globals.Instance.Host}/Items/{{0}}/Similar?userId={Globals.Instance.User.Id}&limit=12&fields=PrimaryImageAspectRatio";
        }

        /// <summary>
        /// Reference for the movie adapter.
        /// </summary>
        private readonly IAdapter<Item, Movie> _movieAdapter;

        /// <summary>
        /// Reference for the movie details adapter.
        /// </summary>
        private readonly IAdapter<MovieDetailsResult, MovieDetail> _movieDetailsAdapter;

        /// <summary>
        /// Reference for the image service.
        /// </summary>
        private readonly IImageService _imageService;

        #endregion

        #region ctor

        public MovieService(IAdapter<Item, Movie> movieAdapter, IAdapter<MovieDetailsResult, MovieDetail> movieDetailsAdapter, IImageService imageService)
        {
            _movieAdapter = movieAdapter ??
                throw new ArgumentNullException(nameof(movieAdapter));

            _movieDetailsAdapter = movieDetailsAdapter ??
                throw new ArgumentNullException(nameof(movieDetailsAdapter));

            _imageService = imageService ??
                throw new ArgumentNullException(nameof(imageService));

            MovieImageDownloadQueue = new TaskQueue<Movie>(1, ProcessMovieImages);
        }
        
        #endregion

        #region Additional methods

        public async Task<IEnumerable<Movie>> GetMovies()
        {
            List<Movie> movieList = new List<Movie>();

            using (HttpClient cli = new HttpClient())
            {
                cli.AddAuthorizationHeaders();

                HttpResponseMessage result = await cli.GetAsync(ListMoviesEndpoint);

                if (!result.IsSuccessStatusCode)
                {
                    return new List<Movie>();
                }

                string jsonResult = await result.Content.ReadAsStringAsync();

                JellyfinMovieResult resultSet = JsonConvert.DeserializeObject<JellyfinMovieResult>(jsonResult);
                
                foreach (Item item in resultSet.Items)
                {
                    Movie movie = _movieAdapter.Convert(item);
                    movieList.Add(movie);
                    MovieImageDownloadQueue.EnqueueTask(movie);
                }
            }

            return movieList;
        }

        public async Task<MovieDetail> GetMovieDetails(string movieId)
        {
            try
            {
                using (HttpClient cli = new HttpClient())
                {
                    cli.AddAuthorizationHeaders();

                    HttpResponseMessage result = cli.GetAsync($"{GetMovieDetailsEndpoint}{movieId}").Result;

                    if (!result.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    string jsonResult = await result.Content.ReadAsStringAsync();

                    MovieDetailsResult resultSet = JsonConvert.DeserializeObject<MovieDetailsResult>(jsonResult);

                    var item = _movieDetailsAdapter.Convert(resultSet);
                    MovieImageDownloadQueue.EnqueueTask(item);
                    return item;
                }
            }
            catch (Exception xc)
            {
                Debugger.Break();
            }

            return null;
        }

        public async Task<IEnumerable<Movie>> GetRelatedMovies(string movieId)
        {
            List<Movie> movieList = new List<Movie>();

            using (HttpClient cli = new HttpClient())
            {
                cli.AddAuthorizationHeaders();

                HttpResponseMessage result = await cli.GetAsync(string.Format(GetRelatedMoviesEndpoint, movieId));

                if (!result.IsSuccessStatusCode)
                {
                    return new List<Movie>();
                }

                string jsonResult = await result.Content.ReadAsStringAsync();

                JellyfinMovieResult resultSet = JsonConvert.DeserializeObject<JellyfinMovieResult>(jsonResult);

                foreach (Item item in resultSet.Items)
                {
                    Movie movie = _movieAdapter.Convert(item);
                    movieList.Add(movie);
                    MovieImageDownloadQueue.EnqueueTask(movie);
                }
            }

            return movieList;
        }

        private void ProcessMovieImages(Movie movie)
        {
            if (!string.IsNullOrEmpty(movie.ImageId))
            {
                movie.ImageData =
                    _imageService.GetImage(movie.Id, movie.ImageId).Result;
                
            }
        }

        #endregion
    }
}
