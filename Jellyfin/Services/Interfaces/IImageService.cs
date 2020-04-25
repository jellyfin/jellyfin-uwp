using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Models;

namespace Jellyfin.Services.Interfaces
{
    /// <summary>
    /// Interface for the image service.
    /// </summary>
    public interface IImageService
    {
        Task GetImage(Movie movie);
    }
}