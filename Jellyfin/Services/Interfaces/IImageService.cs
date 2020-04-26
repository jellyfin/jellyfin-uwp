using System.Threading.Tasks;

namespace Jellyfin.Services.Interfaces
{
    /// <summary>
    /// Interface for the image service.
    /// </summary>
    public interface IImageService
    {
        Task<byte[]> GetImage(string id, string imageId);
    }
}