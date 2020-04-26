using System;
using Windows.UI.Core;
using Jellyfin.Core;

namespace Jellyfin.Models
{
    /// <summary>
    /// Movie model representation
    /// </summary>
    public class Movie : ModelBase
    {
        #region Properties

        /// <summary>
        /// The ID of the movie.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The Image ID of the movie.
        /// </summary>
        public string ImageId { get; set; }

        
        #region ToolCommand

        private byte[] _imageData;

        /// <summary>
        /// The byte array of the downloaded data of the image.
        /// </summary>
        public byte[] ImageData
        {
            get { return _imageData; }
            set
            {
                _imageData = value;
                Globals.Instance.UIDispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () =>
                    {
                        RaisePropertyChanged(nameof(ImageData));
                    });
            }
        }

        #endregion

        /// <summary>
        /// The title of the movie.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The year of the release.
        /// See field ProductionYear
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// Inidicates whether the media element has subtitles.
        /// </summary>
        public bool HasSubtitles { get; set; }

        /// <summary>
        /// The date when it went to premiere. Might be not used at all...
        /// </summary>
        [Obsolete]
        public DateTimeOffset PremiereDate { get; set; }

        /// <summary>
        /// The community rating of the movie.
        /// See field CommunityRating.
        /// </summary>
        public string CommunityRating { get; set; }

        /// <summary>
        /// The official rating (R, PG13, ...)
        /// </summary>
        public string OfficialRating { get; set; }

        /// <summary>
        /// The length of the movie.
        /// </summary>
        public TimeSpan Runtime { get; set; }

        public string FormattedRuntime
        {
            get { return Runtime.Hours + " hr " + Runtime.Minutes + " min"; }
        }

        public TimeSpan PlaybackPosition { get; set; }

        public TimeSpan PlaybackRemaining
        {
            get { return Runtime - PlaybackPosition; }
        }

        public string FormattedPlaybackRemaining
        {
            get
            {
                if (PlaybackRemaining == TimeSpan.Zero)
                {
                    return string.Empty;
                }

                if (PlaybackRemaining == Runtime)
                {
                    return " • Unplayed";
                }

                return $" • {PlaybackRemaining.Hours} hr {PlaybackRemaining.Minutes} min remaining";
            }
        }

        public bool IsPlayed { get; set; }

        /// <summary>
        /// The selected video type to playback.
        /// See MediaStreams[TYPE=VIDEO].DisplayTitle
        /// </summary>
        public string VideoType { get; set; }

        /// <summary>
        /// The selected audio type to playback.
        /// See MediaStreams[TYPE=AUDIO].DisplayTitle
        /// </summary>
        public string AudioType { get; set; }

        /// <summary>
        /// The genres of the movie.
        /// </summary>
        public string[] Genres { get; set; }

        public string FormattedGenres
        {
            get => string.Join(", ", Genres);
        }

        /// <summary>
        /// The "overview" of the movie
        /// See field Overview
        /// </summary>
        public string Description { get; set; }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return $"<{Id}> {Name}";
        }

        #endregion
    }
}