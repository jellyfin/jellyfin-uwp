using System;
using System.Net;
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
    public class LoginService : ServiceBase, ILoginService
    {
        #region Properties

        public const string AuthenticateByNameEndpoint = "/Users/authenticatebyname";

        #endregion

        #region Additional methods

        public async Task<bool> CheckUrl(string host)
        {
            // also do a check for valid url
            if (!Uri.IsWellFormedUriString(host, UriKind.Absolute))
            {
                return false;
            }

            Uri testUri = new Uri(host);

            // check scheme
            if (testUri.Scheme != "http" && testUri.Scheme != "https")
            {
                return false;
            }

            // check URL exists
            HttpWebRequest request;
            HttpWebResponse response;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(testUri);
                response = (HttpWebResponse)(await request.GetResponseAsync());
            }
            catch (Exception)
            {
                return false;
            }

            if (response == null || response.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }

            // also do a check that url points to Jellyfin web client
            if (!response.Headers.ToString().Contains("Emby"))
            {
                return false;
            }

            return true;
        }

        public async Task<bool> Login(string host, LoginModel loginModel)
        {
            try
            {
                if (!await CheckUrl(host))
                {
                    return false;
                }

                string json = JsonConvert.SerializeObject(loginModel);

                using (HttpClient client = new HttpClient())
                {
                    client.AddAuthorizationHeaders();
                    client.DefaultRequestHeaders.Accept
                        .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string authUrl = $"{host}{AuthenticateByNameEndpoint}";

                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage result = await client.PostAsync(authUrl, content);
                    if (!result.IsSuccessStatusCode)
                    {
                        return false;
                    }

                    string resultText = await result.Content.ReadAsStringAsync();
                    LoginResult jsonObject = JsonConvert.DeserializeObject<LoginResult>(resultText);
                    
                    Globals.Instance.AccessToken = jsonObject.AccessToken;
                    Globals.Instance.ServerId = jsonObject.ServerId;
                    Globals.Instance.SessionInfo = jsonObject.SessionInfo;
                    Globals.Instance.User = jsonObject.User;
                    Globals.Instance.Host = host;
                }

                return true;
            }
            catch (Exception xc)
            {
                return false;
            }
        }

        #endregion
    }
}