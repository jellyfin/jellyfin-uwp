using System;

namespace Jellyfin.Models
{

    public class MovieDetailsResult
    {
        public string Name { get; set; }
        public string OriginalTitle { get; set; }
        public string ServerId { get; set; }
        public string Id { get; set; }
        public string Etag { get; set; }
        public DateTime DateCreated { get; set; }
        public bool CanDelete { get; set; }
        public bool CanDownload { get; set; }
        public bool HasSubtitles { get; set; }
        public string Container { get; set; }
        public string SortName { get; set; }
        public DateTime PremiereDate { get; set; }
        public Externalurl[] ExternalUrls { get; set; }
        public Mediasource[] MediaSources { get; set; }
        public int CriticRating { get; set; }
        public string[] ProductionLocations { get; set; }
        public string Path { get; set; }
        public bool EnableMediaSourceDisplay { get; set; }
        public string OfficialRating { get; set; }
        public string Overview { get; set; }
        public string[] Taglines { get; set; }
        public string[] Genres { get; set; }
        public float CommunityRating { get; set; }
        public long RunTimeTicks { get; set; }
        public string PlayAccess { get; set; }
        public int ProductionYear { get; set; }
        public Remotetrailer[] RemoteTrailers { get; set; }
        public Providerids ProviderIds { get; set; }
        public bool IsHD { get; set; }
        public bool IsFolder { get; set; }
        public string ParentId { get; set; }
        public string Type { get; set; }
        public Person[] People { get; set; }
        public Studio[] Studios { get; set; }
        public Genreitem[] GenreItems { get; set; }
        public int LocalTrailerCount { get; set; }
        public Userdata UserData { get; set; }
        public int SpecialFeatureCount { get; set; }
        public string DisplayPreferencesId { get; set; }
        public object[] Tags { get; set; }
        public float PrimaryImageAspectRatio { get; set; }
        public Mediastream1[] MediaStreams { get; set; }
        public string VideoType { get; set; }
        public Imagetags ImageTags { get; set; }
        public string[] BackdropImageTags { get; set; }
        public object[] ScreenshotImageTags { get; set; }
        public Chapter[] Chapters { get; set; }
        public string LocationType { get; set; }
        public string MediaType { get; set; }
        public object[] LockedFields { get; set; }
        public bool LockData { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Providerids
    {
        public string Tmdb { get; set; }
        public string Imdb { get; set; }
        public string TmdbCollection { get; set; }
    }

    public class Userdata
    {
        public float PlayedPercentage { get; set; }
        public long PlaybackPositionTicks { get; set; }
        public int PlayCount { get; set; }
        public bool IsFavorite { get; set; }
        public DateTime LastPlayedDate { get; set; }
        public bool Played { get; set; }
        public string Key { get; set; }
    }

    public class Imagetags
    {
        public string Primary { get; set; }
    }

    public class Externalurl
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class Mediasource
    {
        public string Protocol { get; set; }
        public string Id { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
        public string Container { get; set; }
        public long Size { get; set; }
        public string Name { get; set; }
        public bool IsRemote { get; set; }
        public string ETag { get; set; }
        public long RunTimeTicks { get; set; }
        public bool ReadAtNativeFramerate { get; set; }
        public bool IgnoreDts { get; set; }
        public bool IgnoreIndex { get; set; }
        public bool GenPtsInput { get; set; }
        public bool SupportsTranscoding { get; set; }
        public bool SupportsDirectStream { get; set; }
        public bool SupportsDirectPlay { get; set; }
        public bool IsInfiniteStream { get; set; }
        public bool RequiresOpening { get; set; }
        public bool RequiresClosing { get; set; }
        public bool RequiresLooping { get; set; }
        public bool SupportsProbing { get; set; }
        public string VideoType { get; set; }
        public Mediastream[] MediaStreams { get; set; }
        public object[] MediaAttachments { get; set; }
        public object[] Formats { get; set; }
        public int Bitrate { get; set; }
        public Requiredhttpheaders RequiredHttpHeaders { get; set; }
        public int DefaultAudioStreamIndex { get; set; }
        public int DefaultSubtitleStreamIndex { get; set; }
    }

    public class Requiredhttpheaders
    {
    }

    public class Mediastream
    {
        public string Codec { get; set; }
        public string CodecTag { get; set; }
        public string Language { get; set; }
        public string TimeBase { get; set; }
        public string CodecTimeBase { get; set; }
        public string VideoRange { get; set; }
        public string DisplayTitle { get; set; }
        public string NalLengthSize { get; set; }
        public bool IsInterlaced { get; set; }
        public bool IsAVC { get; set; }
        public int BitRate { get; set; }
        public int BitDepth { get; set; }
        public int RefFrames { get; set; }
        public bool IsDefault { get; set; }
        public bool IsForced { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public float AverageFrameRate { get; set; }
        public float RealFrameRate { get; set; }
        public string Profile { get; set; }
        public string Type { get; set; }
        public string AspectRatio { get; set; }
        public int Index { get; set; }
        public bool IsExternal { get; set; }
        public bool IsTextSubtitleStream { get; set; }
        public bool SupportsExternalStream { get; set; }
        public string PixelFormat { get; set; }
        public int Level { get; set; }
        public string ChannelLayout { get; set; }
        public int Channels { get; set; }
        public int SampleRate { get; set; }
        public string localizedUndefined { get; set; }
        public string localizedDefault { get; set; }
        public string localizedForced { get; set; }
        public string Path { get; set; }
    }

    public class Remotetrailer
    {
        public string Url { get; set; }
        public string Name { get; set; }
    }

    public class Person
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Role { get; set; }
        public string Type { get; set; }
        public string PrimaryImageTag { get; set; }
    }

    public class Studio
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }

    public class Genreitem
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }

    public class Mediastream1
    {
        public string Codec { get; set; }
        public string CodecTag { get; set; }
        public string Language { get; set; }
        public string TimeBase { get; set; }
        public string CodecTimeBase { get; set; }
        public string VideoRange { get; set; }
        public string DisplayTitle { get; set; }
        public string NalLengthSize { get; set; }
        public bool IsInterlaced { get; set; }
        public bool IsAVC { get; set; }
        public int BitRate { get; set; }
        public int BitDepth { get; set; }
        public int RefFrames { get; set; }
        public bool IsDefault { get; set; }
        public bool IsForced { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public float AverageFrameRate { get; set; }
        public float RealFrameRate { get; set; }
        public string Profile { get; set; }
        public string Type { get; set; }
        public string AspectRatio { get; set; }
        public int Index { get; set; }
        public bool IsExternal { get; set; }
        public bool IsTextSubtitleStream { get; set; }
        public bool SupportsExternalStream { get; set; }
        public string PixelFormat { get; set; }
        public int Level { get; set; }
        public string ChannelLayout { get; set; }
        public int Channels { get; set; }
        public int SampleRate { get; set; }
        public string localizedUndefined { get; set; }
        public string localizedDefault { get; set; }
        public string localizedForced { get; set; }
        public string Path { get; set; }
    }

    public class Chapter
    {
        public long StartPositionTicks { get; set; }
        public string Name { get; set; }
        public DateTime ImageDateModified { get; set; }
    }

}
