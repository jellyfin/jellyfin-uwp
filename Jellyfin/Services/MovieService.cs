using System;
using System.Collections.Generic;
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

        public const string ListMoviesEndpoint = "/Users/authenticatebyname";

        /// <summary>
        /// Reference for the movie adapter.
        /// </summary>
        private readonly IAdapter<Item, Movie> _movieAdapter;

        #endregion

        #region ctor

        public MovieService(IAdapter<Item, Movie> movieAdapter)
        {
            _movieAdapter = movieAdapter ??
                throw new ArgumentNullException(nameof(movieAdapter));
        }

        #endregion

        #region Additional methods

        public async Task<IEnumerable<Movie>> GetMovies()
        {
            string movies =
                Globals.Instance.Host + $"/Users/{Globals.Instance.User.Id}/Items?IncludeItemTypes=Movie&Recursive=true&Fields=PrimaryImageAspectRatio%2CMediaSourceCount%2CBasicSyncInfo&ImageTypeLimit=1&EnableImageTypes=Primary%2CBackdrop%2CBanner%2CThumb&StartIndex=0&Limit=100000&ParentId=f137a2dd21bbc1b99aa5c0f6bf02a805";

            List<Movie> movieList = new List<Movie>();

            using (HttpClient cli = new HttpClient())
            {
                cli.AddAuthorizationHeaders();

                HttpResponseMessage result = await cli.GetAsync(movies);

                if (!result.IsSuccessStatusCode)
                {
                    return new List<Movie>();
                }

                string jsonResult = await result.Content.ReadAsStringAsync();

                JellyfinMovieResult resultSet = JsonConvert.DeserializeObject<JellyfinMovieResult>(jsonResult);
                
                foreach (Item item in resultSet.Items)
                {
                    movieList.Add(_movieAdapter.Convert(item));
                }
            }

            return movieList;
        }

        #endregion
    }
}
