using System;
using Newtonsoft.Json;

namespace Jellyfin.Models
{
    public class JellyfinMovieResult
    {
        [JsonProperty("Items")]
        public Item[] Items { get; set; }

        [JsonProperty("TotalRecordCount")]
        public long TotalRecordCount { get; set; }

        [JsonProperty("StartIndex")]
        public long StartIndex { get; set; }
    }

    public class Item
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("ServerId")]
        public string ServerId { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("HasSubtitles")]
        public bool HasSubtitles { get; set; }

        [JsonProperty("Container")]
        public string Container { get; set; }

        [JsonProperty("PremiereDate")]
        public DateTimeOffset PremiereDate { get; set; }

        [JsonProperty("CriticRating")]
        public long? CriticRating { get; set; }

        [JsonProperty("OfficialRating")]
        public string OfficialRating { get; set; }

        [JsonProperty("CommunityRating")]
        public double CommunityRating { get; set; }

        [JsonProperty("RunTimeTicks")]
        public long RunTimeTicks { get; set; }

        [JsonProperty("ProductionYear")]
        public long ProductionYear { get; set; }

        [JsonProperty("IsFolder")]
        public bool IsFolder { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("UserData")]
        public UserData UserData { get; set; }

        [JsonProperty("PrimaryImageAspectRatio")]
        public double PrimaryImageAspectRatio { get; set; }

        [JsonProperty("VideoType")]
        public string VideoType { get; set; }

        [JsonProperty("ImageTags")]
        public ImageTags ImageTags { get; set; }

        [JsonProperty("BackdropImageTags")]
        public string[] BackdropImageTags { get; set; }

        [JsonProperty("LocationType")]
        public string LocationType { get; set; }

        [JsonProperty("MediaType")]
        public string MediaType { get; set; }
    }

    public class ImageTags
    {
        [JsonProperty("Primary")]
        public string Primary { get; set; }
    }

    public class UserData
    {
        [JsonProperty("PlaybackPositionTicks")]
        public long PlaybackPositionTicks { get; set; }

        [JsonProperty("PlayCount")]
        public long PlayCount { get; set; }

        [JsonProperty("IsFavorite")]
        public bool IsFavorite { get; set; }

        [JsonProperty("Played")]
        public bool Played { get; set; }

        [JsonProperty("Key")]
        public long Key { get; set; }

        [JsonProperty("LastPlayedDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? LastPlayedDate { get; set; }

        [JsonProperty("PlayedPercentage", NullValueHandling = NullValueHandling.Ignore)]
        public double? PlayedPercentage { get; set; }
    }
}
