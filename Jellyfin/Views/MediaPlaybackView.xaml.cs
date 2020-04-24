// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.Streaming.Adaptive;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
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
            this.InitializeComponent();

        }

        public async Task Start(string id)
        {
            //Uri uri = new Uri("https://jellyfin.pegazus.space/videos/aeca3453-d856-dc2e-d45e-c94bd1664d26/main.m3u8?DeviceId=TW96aWxsYS81LjAgKFdpbmRvd3MgTlQgMTAuMDsgV2luNjQ7IHg2NCkgQXBwbGVXZWJLaXQvNTM3LjM2IChLSFRNTCwgbGlrZSBHZWNrbykgQ2hyb21lLzc3LjAuMzgzNS4wIFNhZmFyaS81MzcuMzZ8MTU4NzAyMDkxMDM5NQ11&MediaSourceId=aeca3453d856dc2ed45ec94bd1664d26&VideoCodec=h264&AudioCodec=mp3,aac&AudioStreamIndex=1&VideoBitrate=680000&AudioBitrate=120000&PlaySessionId=c0e173166acb4dc3b3e88574ce21dc70&api_key=18a300a01fa74c09b551a66945cbd02f&TranscodingMaxAudioChannels=2&RequireAvc=false&Tag=17591644969ff82b8c86bb1037ccd2f9&SegmentContainer=ts&MinSegments=1&BreakOnNonKeyFrames=True");

            string videoUrl =
                "https://jellyfin.pegazus.space/Videos/" + id + "/stream.mov?Static=true&mediaSourceId=" + id + "&deviceId=TW96aWxsYS81LjAgKFdpbmRvd3MgTlQgMTAuMDsgV2luNjQ7IHg2NCkgQXBwbGVXZWJLaXQvNTM3LjM2IChLSFRNTCwgbGlrZSBHZWNrbykgQ2hyb21lLzc3LjAuMzgzNS4wIFNhZmFyaS81MzcuMzZ8MTU4NzAyMDkxMDM5NQ11&api_key=18a300a01fa74c09b551a66945cbd02f&Tag=beb6ef9128431e67c421e4cb890cf84f";

            Uri uri = new Uri(videoUrl);

            AdaptiveMediaSourceCreationResult result = await AdaptiveMediaSource.CreateFromUriAsync(uri);

            if (result.Status == AdaptiveMediaSourceCreationStatus.Success)
            {
                AdaptiveMediaSource ams = result.MediaSource;
                mediaPlayerElement.SetMediaPlayer(new MediaPlayer());
                mediaPlayerElement.MediaPlayer.Source = MediaSource.CreateFromAdaptiveMediaSource(ams);
                mediaPlayerElement.MediaPlayer.Play();

                
                ams.InitialBitrate = ams.AvailableBitrates.Max<uint>();

                //Register for download requests
                //ams.DownloadRequested += DownloadRequested;

                //Register for download failure and completion events
                //ams.DownloadCompleted += DownloadCompleted;
                //ams.DownloadFailed += DownloadFailed;

                //Register for bitrate change events
                //ams.DownloadBitrateChanged += DownloadBitrateChanged;
                //ams.PlaybackBitrateChanged += PlaybackBitrateChanged;

                //Register for diagnostic event
                ams.Diagnostics.DiagnosticAvailable += DiagnosticAvailable;
            }
            else
            {
                // Handle failure to create the adaptive media source
                mediaPlayerElement.SetMediaPlayer(new MediaPlayer());
                mediaPlayerElement.MediaPlayer.BufferingEnded += MediaPlayerOnBufferingEnded;
                mediaPlayerElement.MediaPlayer.Source = MediaSource.CreateFromUri(uri);
                mediaPlayerElement.MediaPlayer.Play();
                //mediaPlayerElement.MediaPlayer.Pause();
            }
        }

        private void MediaPlayerOnBufferingEnded(MediaPlayer sender, object args)
        {
            sender.ToString();
        }

        private void DiagnosticAvailable(AdaptiveMediaSourceDiagnostics sender, AdaptiveMediaSourceDiagnosticAvailableEventArgs args)
        {
            args.ToString();
        }
        
        private void MediaPlaybackView_OnPreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if ((DataContext as MediaPlaybackViewModel).HandleKeyPressed(e.Key))
            {
                mediaPlayerElement.MediaPlayer.Pause();
                e.Handled = true;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Movie movie = e.Parameter as Movie;
            Start(movie.Id);
        }

        private void MediaPlayerElement_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            (DataContext as MediaPlaybackViewModel).HandleKeyPressed(e.Key);
            e.Handled = true;
        }
    }
}
