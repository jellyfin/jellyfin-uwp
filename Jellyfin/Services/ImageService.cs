using System.Net.Http;
using System.Threading.Tasks;
using Jellyfin.Core;
using Jellyfin.Extensions;
using Jellyfin.Services.Interfaces;

namespace Jellyfin.Services
{
    public class ImageService : ServiceBase, IImageService
    {
        #region Additional methods
        
        /// <summary>
        /// Retrieves image as byte array by its id.
        /// </summary>
        /// <param name="id">The ID of the media library element.</param>
        /// <param name="imageId">The ID of the image.</param>
        /// <returns></returns>
        public async Task<byte[]> GetImage(string id, string imageId)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            if (string.IsNullOrEmpty(imageId))
            {
                return null;
            }

            string image =
                $"{Globals.Instance.Host}/Items/{id}/Images/Primary?maxHeight=300&maxWidth=250&tag={imageId}&quality=90";

            using (HttpClient cli = new HttpClient())
            {
                cli.AddAuthorizationHeaders();

                HttpResponseMessage result = await cli.GetAsync(image);
                return await result.Content.ReadAsByteArrayAsync();
            }
        }

        #endregion
    }
}