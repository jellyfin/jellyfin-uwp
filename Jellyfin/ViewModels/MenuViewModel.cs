using System.Diagnostics;

namespace Jellyfin.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        #region Properties

        #endregion

        #region ctor

        #endregion

        #region Additional methods

        protected override void Execute(string commandParameter)
        {
            switch (commandParameter)
            {
                case "Movies":
                    OpenMovies();
                    break;
                default:
                    base.Execute(commandParameter);
                    break;
            }
        }

        private void OpenMovies()
        {
            Debugger.Break();
        }

        #endregion
    }
}