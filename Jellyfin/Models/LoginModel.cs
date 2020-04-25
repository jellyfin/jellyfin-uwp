namespace Jellyfin.Models
{
    /// <summary>
    /// JSON model for provinding username and password.
    /// </summary>
    public class LoginModel
    {
        #region Properties

        public string Username { get; set; }

        public string Pw { get; set; }

        #endregion
    }
}
