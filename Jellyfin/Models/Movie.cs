using System;
using System.Net.Http;
using Jellyfin.Extensions;

namespace Jellyfin.Models
{
    /// <summary>
    /// Movie model representation
    /// </summary>
    public class Movie
    {
        #region Properties

        public string Id { get; set; }

        public string ImageId { get; set; }

        public byte[] ImageData { get; set; }

        public string Name { get; set; }

        public string Year { get; set; }

        public bool HasSubtitles { get; set; }

        public DateTimeOffset PremiereDate { get; set; }

        public string Rating { get; set; }

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
                    return string.Empty;
                }

                return $" • {PlaybackRemaining.Hours} hr {PlaybackRemaining.Minutes} min remaining";
            }
        }

        public bool IsPlayed { get; set; }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return $"<{Id}> {Name}";
        }

        #endregion
    }
}