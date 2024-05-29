using Jellyfin.Utils;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Gaming.Input;
using Windows.Graphics.Display.Core;
using Windows.Media;
using Windows.Storage;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.WebUI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Jellyfin
{
    public sealed partial class MainPage : Page
    {
        private Gamepad _gamepad = null;

        private readonly bool _autoResolution = true;
        private readonly bool _autoFrameRate = true;

        public MainPage()
        {
            InitializeComponent();

            Gamepad.GamepadAdded += Gamepad_GamepadAdded;
            Gamepad.GamepadRemoved += Gamepad_GamepadRemoved;
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
        }

        private async void InitializeWebViewAndNavigateTo(Uri uri)
        {
            await WebView2.EnsureCoreWebView2Async();
            if (WebView2.CoreWebView2 == null)
            {
                Debug.WriteLine("Failed to EnsureCoreWebView2");
                Exit();
            }

            // Allows using DevTools to debug on Windows.
            // Not needed for Xbox as debugging is done remotely
            // https://learn.microsoft.com/en-us/microsoft-edge/webview2/how-to/remote-debugging-xbox
            bool debug = false;

            WebView2.CoreWebView2.Settings.AreDefaultContextMenusEnabled = debug;
            WebView2.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
            WebView2.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false;
            WebView2.CoreWebView2.Settings.IsStatusBarEnabled = false;
            WebView2.CoreWebView2.Settings.HiddenPdfToolbarItems = CoreWebView2PdfToolbarItems.None;
            WebView2.CoreWebView2.Settings.AreDevToolsEnabled = debug;

            AddDeviceFormToUserAgent();
            string nativeShellJs = await PopulateNativeShellJs();
            await WebView2.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(nativeShellJs);

            WebView2.WebMessageReceived += OnWebMessageReceived;

            WebView2.Source = uri;
        }

        /**
         * Adds the device type to WebView2's UserAgent so jellyfin-web can detect Xbox
         */
        private void AddDeviceFormToUserAgent()
        {
            string userAgent = WebView2.CoreWebView2.Settings.UserAgent;
            string deviceForm = AnalyticsInfo.DeviceForm;
            // An "Unknown" device adds no value
            if (!userAgent.Contains(deviceForm) && deviceForm != "Unknown")
            {
                // "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36 Edg/123.0.0.0"
                // becomes
                // "Mozilla/5.0 (Windows NT 10.0; Win64; x64; Xbox Series X) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36 Edg/123.0.0.0"
                const string toReplace = ")";
                string userAgentWithDeviceForm = new Regex(Regex.Escape(toReplace))
                    .Replace(userAgent, "; " + deviceForm + toReplace, 1);
                WebView2.CoreWebView2.Settings.UserAgent = userAgentWithDeviceForm;
            }
        }
        private async Task<string> PopulateNativeShellJs()
        {
            Uri uri = new Uri("ms-appx:///Resources/winuwp.js");
            StorageFile storageFile = await StorageFile.GetFileFromApplicationUriAsync(uri);
            string appHostJs = await FileIO.ReadTextAsync(storageFile);

            Assembly assembly = Assembly.GetExecutingAssembly();
            appHostJs = appHostJs.Replace("APP_NAME",
                Wrap(assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title, '\''));
            appHostJs = appHostJs.Replace("APP_VERSION", Wrap(assembly.GetName().Version.ToString(), '\''));

            string deviceForm = AnalyticsInfo.DeviceForm;
            if (deviceForm == "Unknown")
            {
                deviceForm = AppUtils.GetDeviceFormFactorType().ToString();
            }

            appHostJs = appHostJs.Replace("DEVICE_NAME", Wrap(deviceForm, '\''));

            uint screenWidth = 0;
            uint screenHeight = 0;
            HdmiDisplayInformation hdmiDisplayInformation = HdmiDisplayInformation.GetForCurrentView();
            if (deviceForm.ToLower().Contains("xbox one"))
            {
                screenWidth = 1920;
                screenHeight = 1080;
            }
            else if (hdmiDisplayInformation != null)
            {
                IReadOnlyList<HdmiDisplayMode> supportedDisplayModes =
                    hdmiDisplayInformation.GetSupportedDisplayModes();
                screenWidth = supportedDisplayModes.Max(mode => mode.ResolutionWidthInRawPixels);
                screenHeight = supportedDisplayModes.Max(mode => mode.ResolutionHeightInRawPixels);
            }

            appHostJs = appHostJs.Replace("SCREEN_WIDTH", screenWidth != 0 ? screenWidth.ToString() : "undefined");
            appHostJs = appHostJs.Replace("SCREEN_HEIGHT", screenHeight != 0 ? screenHeight.ToString() : "undefined");

            return appHostJs;
        }
        private string Wrap(string text, char c)
        {
            return c + text + c;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is string url)
            {
                InitializeWebViewAndNavigateTo(new Uri(url));
            }
        }

        private void OnWebMessageReceived(WebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            try
            {
                string jsonMessage = args.TryGetWebMessageAsString();
                if (JsonObject.TryParse(jsonMessage, out JsonObject argsJson))
                {
                    HandleJsonNotification(argsJson);
                }
                else
                {
                    Debug.WriteLine($"Failed to parse args as JSON: {args}");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to process OnWebMessageReceived with args: {args}", e);
            }
        }

        private async void HandleJsonNotification(JsonObject json)
        {
            string eventType = json.GetNamedString("type");
            JsonObject args = json.GetNamedObject("args");

            if (eventType == "enableFullscreen")
            {
                if (AppUtils.IsXbox)
                {
                    if (args != null)
                    {
                        uint videoWidth = (uint)args.GetNamedNumber("videoWidth");
                        uint videoHeight = (uint)args.GetNamedNumber("videoHeight");
                        double videoFrameRate = args.GetNamedNumber("videoFrameRate");
                        string videoRangeType = args.GetNamedString("videoRangeType");
                        HdmiDisplayHdrOption hdmiDisplayHdrOption = GetHdmiDisplayHdrOption(videoRangeType);
                        await SwitchToBestDisplayMode(videoWidth, videoHeight, videoFrameRate, hdmiDisplayHdrOption);
                    }
                    else
                    {
                        Debug.WriteLine(eventType + " contains no args");
                    }
                }
                else
                {
                    ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                }
            }
            else if (eventType == "disableFullscreen")
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
            else if (eventType == "exit")
            {
                Exit();
            }
            else
            {
                Debug.WriteLine($"Unexpected JSON message: {eventType}");
            }
        }

        private void Exit()
        {
            Application.Current.Exit();
        }

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
            if (_autoResolution)
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

            if (_autoFrameRate)
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

        private void Gamepad_GamepadAdded(object sender, Gamepad e)
        {
            if (_gamepad == null)
            {
                _gamepad = e;
                Task.Run(() => GamepadInputLoop());
            }
        }

        private void Gamepad_GamepadRemoved(object sender, Gamepad e)
        {
            if (_gamepad == e)
            {
                _gamepad = null;
            }
        }

        private async void GamepadInputLoop()
        {
            while (_gamepad != null)
            {
                var reading = _gamepad.GetCurrentReading();

                if (reading.Buttons.HasFlag(GamepadButtons.A))
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await WebView2.ExecuteScriptAsync("document.activeElement.click();");
                    });
                }
                else if (reading.Buttons.HasFlag(GamepadButtons.B))
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        if (WebView2.CanGoBack) WebView2.GoBack();
                    });
                }

                if (reading.Buttons.HasFlag(GamepadButtons.DPadUp))
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await WebView2.ExecuteScriptAsync("navigateFocusableElements('prev');");
                    });
                }
                else if (reading.Buttons.HasFlag(GamepadButtons.DPadDown))
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await WebView2.ExecuteScriptAsync("navigateFocusableElements('next');");
                    });
                }

                await Task.Delay(100); // Delay to prevent rapid firing
            }
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            // Handle key inputs if needed
        }
    }
}