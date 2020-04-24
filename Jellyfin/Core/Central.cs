using Unity;

namespace Jellyfin.Core
{
    public static class Central
    {
        public static SettingsManager Settings { get; } = new SettingsManager();
        
    }
}
