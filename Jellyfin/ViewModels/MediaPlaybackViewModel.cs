using Windows.System;

namespace Jellyfin.ViewModels
{
    public class MediaPlaybackViewModel : JellyfinViewModelBase
    {
        #region Properties

        #region Source

        private string _source;

        public string Source
        {
            get { return _source; }
            set
            {
                _source = value;
                RaisePropertyChanged(nameof(Source));
            }
        }

        #region Chunk

        private int _chunk;

        public int Chunk
        {
            get { return _chunk; }
            set
            {
                _chunk = value;
                RaisePropertyChanged(nameof(Chunk));
            }
        }

        #endregion

        #endregion
        
        #endregion

        #region ctor

        #endregion

        #region Additional methods

        protected override void Execute(string commandParameter)
        {
            switch (commandParameter)
            {
                case "Play":
                    break;
                default:
                    base.Execute(commandParameter);
                    break;
            }
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