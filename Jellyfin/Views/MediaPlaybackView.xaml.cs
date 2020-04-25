using System;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Jellyfin.Core;
using Jellyfin.Models;
using Jellyfin.ViewModels;

namespace Jellyfin.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MediaPlaybackView
    {
        public MediaPlaybackView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Starts playing back the video with the provided id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task Start(string id)
        {
            string videoUrl =
                Globals.Instance.Host + "/Videos/" + id + "/stream.mov?Static=true&mediaSourceId=" + id + "&deviceId=" + Globals.Instance.SessionInfo.DeviceId + "&api_key=" + Globals.Instance.AccessToken + "&Tag=beb6ef9128431e67c421e4cb890cf84f";

            Uri uri = new Uri(videoUrl);

            mediaPlayerElement.SetMediaPlayer(new MediaPlayer());
            mediaPlayerElement.MediaPlayer.Source = MediaSource.CreateFromUri(uri);
            mediaPlayerElement.MediaPlayer.Play();
        }
        
        /// <summary>
        /// Handles that if the controller B is pressed, stops the playback.
        /// TODO add OSD and everything
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediaPlaybackView_OnPreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if ((DataContext as MediaPlaybackViewModel).HandleKeyPressed(e.Key))
            {
                mediaPlayerElement.MediaPlayer.Pause();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles to start playing back the movie passed from the previous frame.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Movie movie = e.Parameter as Movie;
            Start(movie.Id);
        }

        /// <summary>
        /// Handles that if the controller B is pressed, stops the playback.
        /// TODO add OSD and everything
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediaPlayerElement_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            (DataContext as MediaPlaybackViewModel).HandleKeyPressed(e.Key);
            e.Handled = true;
        }
    }
}
