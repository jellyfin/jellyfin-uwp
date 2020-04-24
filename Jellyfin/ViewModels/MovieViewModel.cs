using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using Jellyfin.Models;
using Jellyfin.Models.Adapters;
using Jellyfin.Views;
using Newtonsoft.Json;

namespace Jellyfin.ViewModels
{
    public class MovieViewModel : JellyfinViewModelBase
    {
        #region Properties

        #region Movies

        private ObservableCollection<Movie> _movies = new ObservableCollection<Movie>();

        public ObservableCollection<Movie> Movies
        {
            get { return _movies; }
            set { _movies = value; }
        }

        #endregion

        #endregion

        #region ctor

        public MovieViewModel()
        {
            Load();
        }

        #endregion

        #region Additional methods

        protected override void Execute(string commandParameter)
        {
            switch (commandParameter)
            {
                case "Test":
                    //Frame.Navigate()
                    break;
                default:
                    base.Execute(commandParameter);
                    break;
            }
        }

        public void Load()
        {
            String movies =
                "https://jellyfin.pegazus.space/Users/c1f3ab737ef0467085161dc72999020b/Items?SortBy=SortName%2CProductionYear&SortOrder=Ascending&IncludeItemTypes=Movie&Recursive=true&Fields=PrimaryImageAspectRatio%2CMediaSourceCount%2CBasicSyncInfo&ImageTypeLimit=1&EnableImageTypes=Primary%2CBackdrop%2CBanner%2CThumb&StartIndex=0&Limit=100000&ParentId=f137a2dd21bbc1b99aa5c0f6bf02a805";

            using (HttpClient cli = new HttpClient())
            {
                cli.DefaultRequestHeaders.Add("X-Emby-Authorization", @"MediaBrowser Client=""Jellyfin Web"", Device=""Chrome"", DeviceId=""TW96aWxsYS81LjAgKFdpbmRvd3MgTlQgMTAuMDsgV2luNjQ7IHg2NCkgQXBwbGVXZWJLaXQvNTM3LjM2IChLSFRNTCwgbGlrZSBHZWNrbykgQ2hyb21lLzc3LjAuMzgzNS4wIFNhZmFyaS81MzcuMzZ8MTU4NzAyMDkxMDM5NQ11"", Version=""10.5.3"", Token=""18a300a01fa74c09b551a66945cbd02f""");

                var result = cli.GetAsync(movies).Result;
                var jsonResult = result.Content.ReadAsStringAsync().Result;

                var resultSet = JsonConvert.DeserializeObject<JellyfinMovieResult>(jsonResult);

                resultSet.ToString();
                MovieAdapter mAdapter = new MovieAdapter();

                foreach (var item in resultSet.Items)
                {
                    Movies.Add(mAdapter.Convert(item));
                }
            }
        }

        #endregion

        public void NavigateToMovie(Movie movie)
        {
            NavigationService.Navigate(typeof(MovieDetailView), movie);
        }
    }
}