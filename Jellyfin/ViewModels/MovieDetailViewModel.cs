using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using Jellyfin.Models;
using Newtonsoft.Json;

namespace Jellyfin.ViewModels
{
    public class MovieDetailViewModel : BaseViewModel
    {
        #region Properties
        
        #endregion

        #region ctor

        public MovieDetailViewModel()
        {
        }

        #endregion

        #region Additional methods

        protected override void Execute(string commandParameter)
        {
            switch (commandParameter)
            {
                case "":
                    break;
                default:
                    base.Execute(commandParameter);
                    break;
            }
        }
        
        #endregion
    }
}