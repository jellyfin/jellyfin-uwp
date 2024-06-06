using Jellyfin.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Graphics.Display.Core;
using Windows.UI.ViewManagement;
namespace Jellyfin.Utils
{
    public sealed class FullScreenManager
    {
        private async Task SwitchToBestDisplayMode(uint videoWidth, uint videoHeight, double videoFrameRate,
           HdmiDisplayHdrOption hdmiDisplayHdrOption)
        {
            HdmiDisplayMode bestDisplayMode =
                GetBestDisplayMode(videoWidth, videoHeight, videoFrameRate, hdmiDisplayHdrOption);
            if (bestDisplayMode != null)
            {
                await HdmiDisplayInformation.GetForCurrentView()
                    ?.RequestSetCurrentDisplayModeAsync(bestDisplayMode, hdmiDisplayHdrOption);
            }
        }

        private HdmiDisplayHdrOption GetHdmiDisplayHdrOption(string videoRangeType)
        {
            HdmiDisplayInformation hdmiDisplayInformation = HdmiDisplayInformation.GetForCurrentView();
            if (hdmiDisplayInformation == null)
            {
                return HdmiDisplayHdrOption.None;
            }

            IReadOnlyList<HdmiDisplayMode> supportedDisplayModes = hdmiDisplayInformation.GetSupportedDisplayModes();
            bool displaySupportsDoVi = supportedDisplayModes.Any(mode => mode.IsDolbyVisionLowLatencySupported);
            bool displaySupportsHdr = supportedDisplayModes.Any(mode => mode.IsSmpte2084Supported);

            HdmiDisplayHdrOption hdrOtherwiseSdr =
                displaySupportsHdr ? HdmiDisplayHdrOption.Eotf2084 : HdmiDisplayHdrOption.None;
            HdmiDisplayHdrOption doViOtherwiseHdrOtherwiseSdr =
                displaySupportsDoVi ? HdmiDisplayHdrOption.DolbyVisionLowLatency : hdrOtherwiseSdr;

            switch (videoRangeType)
            {
                // Xbox only supports DOVI profile 5
                case "DOVI":
                    return doViOtherwiseHdrOtherwiseSdr;
                case "DOVIWithHDR10":
                case "DOVIWithHLG":
                case "HDR":
                case "HDR10":
                case "HDR10Plus":
                case "HLG":
                    return hdrOtherwiseSdr;
                case "DOVIWithSDR":
                case "SDR":
                case "Unknown":
                default:
                    return HdmiDisplayHdrOption.None;
            }
        }

        private Predicate<HdmiDisplayMode> ToRefreshRateMatchesPredicate(double refreshRate)
        {
            bool RefreshRateMatches(HdmiDisplayMode mode) => Math.Abs(refreshRate - mode.RefreshRate) <= 0.5;
            return RefreshRateMatches;
        }

        private Predicate<HdmiDisplayMode> ToResolutionMatchesPredicate(uint width, uint height)
        {
            bool ResolutionMatches(HdmiDisplayMode mode) => mode.ResolutionWidthInRawPixels == width ||
                                          mode.ResolutionHeightInRawPixels == height;
            return ResolutionMatches;
        }

        private Predicate<HdmiDisplayMode> ToHdmiDisplayHdrOptionMatchesPredicate(
            HdmiDisplayHdrOption hdmiDisplayHdrOption)
        {
            bool HdrMatches(HdmiDisplayMode mode) => hdmiDisplayHdrOption == HdmiDisplayHdrOption.None ||
                                                             (hdmiDisplayHdrOption ==
                                                              HdmiDisplayHdrOption.DolbyVisionLowLatency &&
                                                              mode.IsDolbyVisionLowLatencySupported) ||
                                                             (hdmiDisplayHdrOption == HdmiDisplayHdrOption.Eotf2084 &&
                                                              mode.IsSmpte2084Supported);
            return HdrMatches;
        }

        private HdmiDisplayMode GetBestDisplayMode(uint videoWidth, uint videoHeight, double videoFrameRate,
            HdmiDisplayHdrOption hdmiDisplayHdrOption)
        {
            HdmiDisplayInformation hdmiDisplayInformation = HdmiDisplayInformation.GetForCurrentView();
            IEnumerable<HdmiDisplayMode> supportedHdmiDisplayModes = hdmiDisplayInformation.GetSupportedDisplayModes();
            // `GetHdmiDisplayHdrOption(...)` ensures the HdmiDisplayHdrOption is always a mode the display supports
            Predicate<HdmiDisplayMode> hdrMatchesPredicate =
                ToHdmiDisplayHdrOptionMatchesPredicate(hdmiDisplayHdrOption);
            IEnumerable<HdmiDisplayMode> hdmiDisplayModes =
                supportedHdmiDisplayModes.Where(mode => hdrMatchesPredicate.Invoke(mode));

            bool filteredToVideoResolution = false;
            if (Central.Settings.AutoResolution)
            {
                Predicate<HdmiDisplayMode> resolutionMatchesVideoPredicate =
                    ToResolutionMatchesPredicate(videoWidth, videoHeight);
                IEnumerable<HdmiDisplayMode> matchingResolution =
                    hdmiDisplayModes.Where(mode => resolutionMatchesVideoPredicate.Invoke(mode));
                if (matchingResolution.Any())
                {
                    hdmiDisplayModes = matchingResolution;
                    filteredToVideoResolution = true;
                }
            }
            HdmiDisplayMode currentHdmiDisplayMode = hdmiDisplayInformation.GetCurrentDisplayMode();
            Predicate<HdmiDisplayMode> resolutionMatchesCurrentDisplayPredicate = ToResolutionMatchesPredicate(
                currentHdmiDisplayMode.ResolutionWidthInRawPixels,
                currentHdmiDisplayMode.ResolutionHeightInRawPixels);
            if (!filteredToVideoResolution)
            {
                hdmiDisplayModes =
                    hdmiDisplayModes.Where(mode => resolutionMatchesCurrentDisplayPredicate.Invoke(mode));
            }

            if (Central.Settings.AutoRefreshRate)
            {
                Predicate<HdmiDisplayMode> refreshRateMatchesVideoPredicate =
                    ToRefreshRateMatchesPredicate(videoFrameRate);
                IEnumerable<HdmiDisplayMode> matchingRefreshRates =
                    hdmiDisplayModes.Where(mode => refreshRateMatchesVideoPredicate.Invoke(mode));
                if (matchingRefreshRates.Any())
                {
                    return matchingRefreshRates.First();
                }
            }

            Predicate<HdmiDisplayMode> refreshRateMatchesCurrentDisplayPredicate =
                ToRefreshRateMatchesPredicate(currentHdmiDisplayMode.RefreshRate);
            // fall back to current resolution/refreshRate as a mode that supports the hdmiDisplayHdrOption is required. 
            return hdmiDisplayModes.Where(mode => resolutionMatchesCurrentDisplayPredicate.Invoke(mode))
                .Where(mode => refreshRateMatchesCurrentDisplayPredicate.Invoke(mode)).FirstOrDefault();
        }

        private async Task SetDefaultDisplayModeAsync()
        {
            await HdmiDisplayInformation.GetForCurrentView()?.SetDefaultDisplayModeAsync();
        }

        public async Task EnableFullscreenAsync(JsonObject args)
        {
            if (AppUtils.IsXbox)
            {
                if (args != null)
                {
                    try
                    {
                        uint videoWidth = (uint)args.GetNamedNumber("videoWidth");
                        uint videoHeight = (uint)args.GetNamedNumber("videoHeight");
                        double videoFrameRate = args.GetNamedNumber("videoFrameRate");
                        string videoRangeType = args.GetNamedString("videoRangeType");
                        HdmiDisplayHdrOption hdmiDisplayHdrOption = GetHdmiDisplayHdrOption(videoRangeType);
                        await SwitchToBestDisplayMode(videoWidth, videoHeight, videoFrameRate, hdmiDisplayHdrOption);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error during SwitchToBestDisplayMode", ex);
                    }
                }
                else
                {
                    Debug.WriteLine("enableFullscreenAsync called with no args");
                }
            }
            else
            {
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            }
        }

        public async void DisableFullScreen()
        {
            if (AppUtils.IsXbox)
            {
                await SetDefaultDisplayModeAsync();
            }
            else
            {
                ApplicationView.GetForCurrentView().ExitFullScreenMode();
            }
        }
    }
}