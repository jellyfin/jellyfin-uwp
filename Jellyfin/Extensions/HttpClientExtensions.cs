using System.Linq;
using System.Net.Http;
using Jellyfin.Core;

namespace Jellyfin.Extensions
{
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Adds X-Emby-Authentication header.
        /// </summary>
        /// <param name="client"></param>
        public static void AddAuthorizationHeaders(this HttpClient client)
        {
            if (client.DefaultRequestHeaders.Any(q => q.Key == Globals.AuthorizationHeader))
            {
                return;
            }

            client.DefaultRequestHeaders.Add(Globals.AuthorizationHeader, Globals.Instance.AuthorizationValue);
        }
    }
}
