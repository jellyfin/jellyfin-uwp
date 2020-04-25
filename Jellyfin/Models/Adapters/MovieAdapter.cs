using System;
using Jellyfin.Services.Interfaces;

namespace Jellyfin.Models.Adapters
{
    /// <summary>
    /// Adapter to map the JSON Item to Movie.
    /// </summary>
    public class MovieAdapter : IAdapter<Item, Movie>
    {
        #region Properties

        private readonly IImageService _imageService;

        #endregion

        #region ctor

        public MovieAdapter(IImageService imageService)
        {
            _imageService = imageService ??
                throw new ArgumentNullException(nameof(imageService));
        }

        #endregion

        #region Additional methods

        public Movie Convert(Item source)
        {
            Movie m = new Movie();

            m.Id = source.Id;
            m.Name = source.Name;
            m.Year = source.ProductionYear.ToString();
            m.ImageId = source.ImageTags.Primary;

            if (!string.IsNullOrEmpty(m.ImageId))
            {
                _imageService.GetImage(m);
            }

            m.HasSubtitles = source.HasSubtitles;
            m.PremiereDate = source.PremiereDate;
            m.Rating = source.CommunityRating.ToString();
            m.Runtime = new TimeSpan(source.RunTimeTicks);
            m.PlaybackPosition = new TimeSpan(source.UserData.PlaybackPositionTicks);
            m.IsPlayed = source.UserData.Played;


            return m;
        }

        #endregion
    }
}
