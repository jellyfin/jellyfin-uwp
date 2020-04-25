using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Jellyfin.Services.Interfaces;

namespace Jellyfin.Services
{
    public sealed class JellyfinNavigationService : IJellyfinNavigationService
    {
        public void Navigate(Type sourcePage)
        {
            Frame frame = (Frame)Window.Current.Content;
            frame.Navigate(sourcePage);
        }

        public void Navigate(Type sourcePage, object parameter)
        {
            Frame frame = (Frame)Window.Current.Content;
            frame.Navigate(sourcePage, parameter);
        }

        public void Navigate(string sourcePage)
        {
            Navigate(Type.GetType(sourcePage));
        }
        public void Navigate(string sourcePage, object parameter)
        {
            Navigate(Type.GetType(sourcePage), parameter);
        }

        /// <summary>
        /// Navigates to the most recent item in forward navigation history, if a Frame manages its own navigation history.
        /// </summary>
        public void GoForward()
        {
            // Frame.CanGoForward()?
            Go(true);
        }
        /// <summary>
        /// Navigates to the most recent item in back navigation history, if a Frame manages its own navigation history.
        /// </summary>
        public void GoBack()
        {
            // Frame.CanGoBack()?
            Go(false);
        }

        private static void Go(bool isForward)
        {
            Frame frame = (Frame)Window.Current.Content;
            if (isForward)
            {
                frame.GoForward();
            }
            else
            {
                frame.GoBack();
            }
        }
    }
}
