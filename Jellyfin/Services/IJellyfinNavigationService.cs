using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Services
{
    public interface IJellyfinNavigationService
    {
        void Navigate(Type sourcePage);
        void Navigate2(Type sourcePage, object parameter);
        void Navigate(Type sourcePage, object parameter);
        void Navigate(string sourcePage);
        void Navigate(string sourcePage, object parameter);
        void GoBack();
        void GoForward();
    }
}
