using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Models;

namespace Jellyfin.Services.Interfaces
{
    /// <summary>
    /// Interface for the movie service.
    /// </summary>
    public interface IMovieService
    {
        Task<IEnumerable<Movie>> GetMovies();
    }
}