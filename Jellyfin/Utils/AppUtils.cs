using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Profile;

namespace Jellyfin.Utils
{
    public class AppUtils
    {
        static bool? _isXbox = null;
        public static Boolean IsXbox
        {
            get
            {
                if (!_isXbox.HasValue)
                {
                    var deviceType = GetDeviceFormFactorType();
                    _isXbox = deviceType == DeviceFormFactorType.Xbox || deviceType == DeviceFormFactorType.Holographic;
                }

                return _isXbox.Value;
            }
        }

        public static DeviceFormFactorType GetDeviceFormFactorType()
        {
            switch (AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                case "Windows.Mobile":
                    return DeviceFormFactorType.Mobile;
                case "Windows.Xbox":
                    return DeviceFormFactorType.Xbox;
                case "Windows.Holographic":
                    return DeviceFormFactorType.Holographic;
                default:
                    return DeviceFormFactorType.Desktop;
            }
        }
    }    
}
