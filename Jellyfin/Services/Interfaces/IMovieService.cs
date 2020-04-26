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
        /// <summary>
        /// Retrieves generic movie result set.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Movie>> GetMovies();

        /// <summary>
        /// Retrieves detailed movie object.
        /// </summary>
        /// <param name="movieId">the ID of the movie to be retrieved.</param>
        /// <returns></returns>
        Task<MovieDetail> GetMovieDetails(string movieId);

        /// <summary>
        /// Retrieves movies which are related to the provided movie.
        /// </summary>
        /// <param name="movieId">the ID of the movie for finding similars.</param>
        /// <returns></returns>
        Task<IEnumerable<Movie>> GetRelatedMovies(string movieId);
    }
}