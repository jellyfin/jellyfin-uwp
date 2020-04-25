using System;
using Jellyfin.Services.Interfaces;
using Jellyfin.Views;

namespace Jellyfin.ViewModels
{
    public class MenuViewModel : JellyfinViewModelBase
    {
        #region Properties

        /// <summary>
        /// Reference for the settings service.
        /// </summary>
        private readonly ISettingsService _settingsService;

        /// <summary>
        /// Reference for the navigation service.
        /// </summary>
        private readonly IJellyfinNavigationService _navigationService;

        #endregion

        #region ctor

        public MenuViewModel(ISettingsService settingsService, IJellyfinNavigationService jellyfinNavigationService)
        {
            _settingsService = settingsService ??
                throw new ArgumentNullException(nameof(settingsService));

            _navigationService = jellyfinNavigationService ??
                throw new ArgumentNullException(nameof(jellyfinNavigationService));
        }

        #endregion

        #region Additional methods

        protected override void Execute(string commandParameter)
        {
            switch (commandParameter)
            {
                case "Logout":
                    Logout();
                    break;
                default:
                    base.Execute(commandParameter);
                    break;
            }
        }

        private void Logout()
        {
            _settingsService.Clear();
            _navigationService.Navigate(typeof(LoginView));
        }

        #endregion
    }
}