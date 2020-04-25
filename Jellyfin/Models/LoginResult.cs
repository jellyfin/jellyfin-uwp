using System;

namespace Jellyfin.Models
{

    public class LoginResult
    {
        public User User { get; set; }
        public Sessioninfo SessionInfo { get; set; }
        public string AccessToken { get; set; }
        public string ServerId { get; set; }
    }

    public class User
    {
        public string Name { get; set; }
        public string ServerId { get; set; }
        public string Id { get; set; }
        public bool HasPassword { get; set; }
        public bool HasConfiguredPassword { get; set; }
        public bool HasConfiguredEasyPassword { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime LastActivityDate { get; set; }
        public Configuration Configuration { get; set; }
        public Policy Policy { get; set; }
    }

    public class Configuration
    {
        public string AudioLanguagePreference { get; set; }
        public bool PlayDefaultAudioTrack { get; set; }
        public string SubtitleLanguagePreference { get; set; }
        public bool DisplayMissingEpisodes { get; set; }
        public object[] GroupedFolders { get; set; }
        public string SubtitleMode { get; set; }
        public bool DisplayCollectionsView { get; set; }
        public bool EnableLocalPassword { get; set; }
        public object[] OrderedViews { get; set; }
        public object[] LatestItemsExcludes { get; set; }
        public object[] MyMediaExcludes { get; set; }
        public bool HidePlayedInLatest { get; set; }
        public bool RememberAudioSelections { get; set; }
        public bool RememberSubtitleSelections { get; set; }
        public bool EnableNextEpisodeAutoPlay { get; set; }
    }

    public class Policy
    {
        public bool IsAdministrator { get; set; }
        public bool IsHidden { get; set; }
        public bool IsDisabled { get; set; }
        public object[] BlockedTags { get; set; }
        public bool EnableUserPreferenceAccess { get; set; }
        public object[] AccessSchedules { get; set; }
        public object[] BlockUnratedItems { get; set; }
        public bool EnableRemoteControlOfOtherUsers { get; set; }
        public bool EnableSharedDeviceControl { get; set; }
        public bool EnableRemoteAccess { get; set; }
        public bool EnableLiveTvManagement { get; set; }
        public bool EnableLiveTvAccess { get; set; }
        public bool EnableMediaPlayback { get; set; }
        public bool EnableAudioPlaybackTranscoding { get; set; }
        public bool EnableVideoPlaybackTranscoding { get; set; }
        public bool EnablePlaybackRemuxing { get; set; }
        public bool ForceRemoteSourceTranscoding { get; set; }
        public bool EnableContentDeletion { get; set; }
        public object[] EnableContentDeletionFromFolders { get; set; }
        public bool EnableContentDownloading { get; set; }
        public bool EnableSyncTranscoding { get; set; }
        public bool EnableMediaConversion { get; set; }
        public object[] EnabledDevices { get; set; }
        public bool EnableAllDevices { get; set; }
        public object[] EnabledChannels { get; set; }
        public bool EnableAllChannels { get; set; }
        public object[] EnabledFolders { get; set; }
        public bool EnableAllFolders { get; set; }
        public int InvalidLoginAttemptCount { get; set; }
        public int LoginAttemptsBeforeLockout { get; set; }
        public bool EnablePublicSharing { get; set; }
        public int RemoteClientBitrateLimit { get; set; }
        public string AuthenticationProviderId { get; set; }
    }

    public class Sessioninfo
    {
        public Playstate PlayState { get; set; }
        public object[] AdditionalUsers { get; set; }
        public Capabilities Capabilities { get; set; }
        public string RemoteEndPoint { get; set; }
        public object[] PlayableMediaTypes { get; set; }
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Client { get; set; }
        public DateTime LastActivityDate { get; set; }
        public DateTime LastPlaybackCheckIn { get; set; }
        public string DeviceName { get; set; }
        public string DeviceId { get; set; }
        public string ApplicationVersion { get; set; }
        public object[] SupportedCommands { get; set; }
        public bool IsActive { get; set; }
        public bool SupportsMediaControl { get; set; }
        public bool SupportsRemoteControl { get; set; }
        public bool HasCustomDeviceName { get; set; }
        public string ServerId { get; set; }
    }

    public class Playstate
    {
        public bool CanSeek { get; set; }
        public bool IsPaused { get; set; }
        public bool IsMuted { get; set; }
        public string RepeatMode { get; set; }
    }

    public class Capabilities
    {
        public object[] PlayableMediaTypes { get; set; }
        public object[] SupportedCommands { get; set; }
        public bool SupportsMediaControl { get; set; }
        public bool SupportsContentUploading { get; set; }
        public bool SupportsPersistentIdentifier { get; set; }
        public bool SupportsSync { get; set; }
    }

}
