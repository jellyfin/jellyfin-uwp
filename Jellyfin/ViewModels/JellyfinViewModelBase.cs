using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Jellyfin.Core;
using Jellyfin.Services.Interfaces;
using Unity;

namespace Jellyfin.ViewModels
{
    public abstract class JellyfinViewModelBase : ViewModelBase
    {
        #region Properties

        public string HashCode
        {
            get { return GetHashCode().ToString(); }
        }

        /// <summary>
        /// For accessing global params from UI
        /// </summary>
        public Globals AppGlobals
        {
            get { return Globals.Instance; }
        }

        #region ToolCommand

        private RelayCommand<string> _toolCommand;

        public RelayCommand<string> ToolCommand
        {
            get { return _toolCommand; }
            set { _toolCommand = value; }
        }

        #endregion

        protected IJellyfinNavigationService NavigationService { get; set; }   

        #endregion

        #region ctor

        protected JellyfinViewModelBase()
        {
            IUnityContainer container = Globals.Instance.Container;
            NavigationService = container.Resolve<IJellyfinNavigationService>();

            ToolCommand = new RelayCommand<string>(Execute, CanExecute);
        }

        #endregion

        #region Additional methods

        protected virtual void Execute(string commandParameter)
        {
            throw new NotImplementedException();
        }

        protected bool CanExecute(string commandParameter)
        {
            return true;
        }

        #endregion
    }
}
