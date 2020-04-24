using System;

namespace Jellyfin.Models.Adapters
{
    public class MovieAdapter : IAdapter<Item, Movie>
    {
        public Movie Convert(Item source)
        {
            Movie m = new Movie();

            m.Id = source.Id;
            m.Name = source.Name;
            m.Year = source.ProductionYear.ToString();
            m.ImageId = source.ImageTags.Primary;

            m.HasSubtitles = source.HasSubtitles;
            m.PremiereDate = source.PremiereDate;
            m.Rating = source.CommunityRating.ToString();
            m.Runtime = new TimeSpan(source.RunTimeTicks);
            m.PlaybackPosition = new TimeSpan(source.UserData.PlaybackPositionTicks);
            m.IsPlayed = source.UserData.Played;


            return m;
        }
    }
}
