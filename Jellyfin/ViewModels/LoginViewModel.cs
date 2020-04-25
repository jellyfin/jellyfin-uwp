using System;
using System.Threading.Tasks;
using Jellyfin.Core;
using Jellyfin.Models;
using Jellyfin.Services.Interfaces;
using Jellyfin.Views;

namespace Jellyfin.ViewModels
{
    public class LoginViewModel : JellyfinViewModelBase
    {
        #region Properties

        #region Host

        private string _host;

        /// <summary>
        /// The Jellyfin server address.
        /// </summary>
        public string Host
        {
            get { return _host; }
            set
            {
                _host = value;
                RaisePropertyChanged(nameof(Host));
            }
        }

        #endregion
        
        #region Username

        private string _username;

        /// <summary>
        /// The user name used for connecting to Jellyfin.
        /// </summary>
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                RaisePropertyChanged(nameof(Username));
            }
        }

        #endregion

        #region Password

        private string _password;

        /// <summary>
        /// The password for the entered username.
        /// </summary>
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                RaisePropertyChanged(nameof(Password));
            }
        }

        #endregion

        #region IsLoginFailed

        private bool _isLoginFailed;

        /// <summary>
        /// Indicates whether the login is succeed or not.
        /// </summary>
        public bool IsLoginFailed
        {
            get { return _isLoginFailed; }
            set
            {
                _isLoginFailed = value;
                RaisePropertyChanged(nameof(IsLoginFailed));
            }
        }

        #endregion

        #region LoginFailureReason

        private string _loginFailureReason;

        /// <summary>
        /// Shows the failure reason of connecting.
        /// </summary>
        public string LoginFailureReason
        {
            get { return _loginFailureReason; }
            set
            {
                _loginFailureReason = value;
                RaisePropertyChanged(nameof(LoginFailureReason));
            }
        }

        #endregion

        /// <summary>
        /// Reference for the login service.
        /// </summary>
        private ILoginService _loginService { get; set; }

        /// <summary>
        /// Reference for the settings service.
        /// </summary>
        private ISettingsService _settingsService { get; set; }

        #endregion

        #region ctor

        public LoginViewModel(ILoginService loginService, ISettingsService settingsService)
        {
            _loginService = loginService ??
                throw new ArgumentNullException(nameof(loginService));

            _settingsService = settingsService ??
                throw new ArgumentNullException(nameof(settingsService));
        }
        
        #endregion

        #region Additional methods

        protected override void Execute(string commandParameter)
        {
            switch (commandParameter)
            {
                case "Login":
                    Login();
                    break;
                default:
                    base.Execute(commandParameter);
                    break;
            }
        }

        public void InitializeSessionSettings()
        {
            Globals.Instance.LoadSettings(_settingsService);
            if (string.IsNullOrEmpty(Globals.Instance.AccessToken))
            {
                return;
            }

            NavigationService.Navigate(typeof(MainWindowView));
        }

        public async Task Login()
        {
            IsLoginFailed = false;
            if (string.IsNullOrEmpty(Host))
            {
                IsLoginFailed = true;
                LoginFailureReason = "Error: the host should not be empty.";
                return;
            }

            if (string.IsNullOrEmpty(Username))
            {
                IsLoginFailed = true;
                LoginFailureReason = "Error: the username should not be empty.";
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                IsLoginFailed = true;
                LoginFailureReason = "Error: the password should not be empty.";
                return;
            }
            
            if (!await CheckUrlValid())
            {
                IsLoginFailed = true;
                LoginFailureReason = "Error: the provided host is invalid.";
                return;
            }

            bool loginResult = await _loginService.Login(Host, new LoginModel {Username = Username, Pw = Password});

            if (!loginResult)
            {
                IsLoginFailed = true;
                LoginFailureReason = "Error: Invalid username or password.";
                return;
            }

            if (!string.IsNullOrEmpty(Globals.Instance.AccessToken))
            {
                Globals.Instance.SaveSettings(_settingsService);
                NavigationService.Navigate(typeof(MainWindowView));
            }
        }

        public async Task<bool> CheckUrlValid()
        {
            Host = Host.ToLower()
                .Replace("https://", string.Empty).Replace("http://", string.Empty);
            
            foreach (string urlPrefix in new [] {"https://", "http://", string.Empty})
            {
                if (!await _loginService.CheckUrl(urlPrefix + Host))
                {
                    continue;
                }

                Host = urlPrefix + Host;
                return true;
            }
            
            return false;
        }

        #endregion
    }
}