using Windows.System;
using Jellyfin.Models;
using Jellyfin.Views;

namespace Jellyfin.ViewModels
{
    public class MovieDetailViewModel : JellyfinViewModelBase
    {
        #region Properties

        #region SelectedMovie

        private Movie _selectedMovie;

        public Movie SelectedMovie
        {
            get { return _selectedMovie; }
            set
            {
                _selectedMovie = value;
                RaisePropertyChanged(nameof(SelectedMovie));
            }
        }

        #endregion

        #endregion
        
        #region Additional methods

        protected override void Execute(string commandParameter)
        {
            switch (commandParameter)
            {
                case "Play":
                    Play();
                    break;
                default:
                    base.Execute(commandParameter);
                    break;
            }
        }

        private void Play()
        {
            NavigationService.Navigate(typeof(MediaPlaybackView), SelectedMovie);
        }

        #endregion

        public bool HandleKeyPressed(VirtualKey key)
        {
            switch (key)
            {
                case VirtualKey.Escape:
                    NavigationService.GoBack();
                    return true;
                default:
                    return false;
            }
        }
    }
}