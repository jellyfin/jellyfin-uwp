using System.Threading.Tasks;
using Jellyfin.Models;

namespace Jellyfin.Services.Interfaces
{
    /// <summary>
    /// Interface for the authentication service.
    /// </summary>
    public interface ILoginService
    {
        Task<bool> CheckUrl(string host);

        Task<bool> Login(string host, LoginModel loginModel);
    }
}