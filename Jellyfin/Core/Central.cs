using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Core
{
    public static class Central
    {
        public static SettingsManager Settings { get; } = new SettingsManager();
    }
}
