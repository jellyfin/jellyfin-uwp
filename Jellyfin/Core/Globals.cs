using Unity;

namespace Jellyfin.Core
{
    public class Globals
    {
        #region Singleton

        private static Globals _instance;

        private Globals()
        {
            Container = new UnityContainer();
        }

        public static Globals Instance => _instance ?? (_instance = new Globals());

        #endregion

        #region Properties

        public IUnityContainer Container { get; set; }

        #endregion
        
    }
}
