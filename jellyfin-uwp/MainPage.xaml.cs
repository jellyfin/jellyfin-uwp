using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace jellyfin_uwp
{
    /// <summary>
    /// MainPage controls frame switching and global settings
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static Frame jfFrame;
        public static SettingsStore globalSettingsStore;

        public MainPage()
        {
            this.InitializeComponent();

            globalSettingsStore = new SettingsStore();

            jfFrame = JellyfinFrame;

            if (!globalSettingsStore.AppURLValid)
            {
                JellyfinFrame.Navigate(typeof(AppSettings));
            }
            else {
                JellyfinFrame.Navigate(typeof(WebView));
            }
        }
    }
}
