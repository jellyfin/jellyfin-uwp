using System;

namespace Jellyfin.Services.Interfaces
{
    public interface IJellyfinNavigationService
    {
        void Navigate(Type sourcePage);

        void Navigate(Type sourcePage, object parameter);

        void Navigate(string sourcePage);

        void Navigate(string sourcePage, object parameter);

        void GoBack();

        void GoForward();
    }
}
