using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;

namespace Jellyfin.ViewModels
{
    public abstract class BaseViewModel
    {
        #region Properties

        #region ToolCommand

        private RelayCommand<string> _toolCommand;

        public RelayCommand<string> ToolCommand
        {
            get { return _toolCommand; }
            set { _toolCommand = value; }
        }

        #endregion

        #endregion

        #region ctor

        protected BaseViewModel()
        {
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
