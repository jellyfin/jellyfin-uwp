using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Jellyfin.Core;
using Jellyfin.Extensions;
using Jellyfin.Models;
using Jellyfin.Services.Interfaces;
using Newtonsoft.Json;

namespace Jellyfin.Services
{
    public class ImageService : ServiceBase, IImageService
    {
        #region Additional methods
        
        /// <summary>
        /// Retrieves image for a movie.
        /// TODO to extend to be generic for any media type.
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        public async Task GetImage(Movie movie)
        {
            if (movie == null)
            {
                return;
            }

            string image =
                $"{Globals.Instance.Host}/Items/{movie.Id}/Images/Primary?maxHeight=300&maxWidth=250&tag={movie.ImageId}&quality=90";

            using (HttpClient cli = new HttpClient())
            {
                cli.AddAuthorizationHeaders();

                HttpResponseMessage result = await cli.GetAsync(image);
                movie.ImageData = await result.Content.ReadAsByteArrayAsync();
            }
        }

        #endregion
    }
}