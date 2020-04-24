using System.Net.Http;
using Windows.UI.Xaml.Media.Imaging;
using Newtonsoft.Json;

namespace Jellyfin.Models
{
    public class Movie : ModelBase
    {
        #region Properties

        public string Id { get; set; }

        #region ImageId

        private string _imageId;

        public string ImageId
        {
            get { return _imageId; }
            set
            {
                _imageId = value;
                DownloadImage();
            }
        }

        #endregion

        public byte[] ImageData { get; set; }

        public string Name { get; set; }

        public string Year { get; set; }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return $"<{Id}> {Name}";
        }

        #endregion

        private void DownloadImage()
        {
            string image =
                "https://jellyfin.pegazus.space/Items/" + Id + "/Images/Primary?maxHeight=300&maxWidth=250&tag=" + ImageId + "&quality=90";

            using (HttpClient cli = new HttpClient())
            {
                cli.DefaultRequestHeaders.Add("X-Emby-Authorization", @"MediaBrowser Client=""Jellyfin Web"", Device=""Chrome"", DeviceId=""TW96aWxsYS81LjAgKFdpbmRvd3MgTlQgMTAuMDsgV2luNjQ7IHg2NCkgQXBwbGVXZWJLaXQvNTM3LjM2IChLSFRNTCwgbGlrZSBHZWNrbykgQ2hyb21lLzc3LjAuMzgzNS4wIFNhZmFyaS81MzcuMzZ8MTU4NzAyMDkxMDM5NQ11"", Version=""10.5.3"", Token=""18a300a01fa74c09b551a66945cbd02f""");

                var result = cli.GetAsync(image).Result;
                ImageData = result.Content.ReadAsByteArrayAsync().Result;
            }
        }
    }
}