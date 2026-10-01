using Umbraco26.Models;

namespace Umbraco26.Business.Services.Interfaces
{
    public interface IOmdbService
    {
        Task<List<OmdbMovie>> SearchAsync(OmdbSearchModel search);

        Task<OmdbMovieDetails?> GetByIdAsync(string imdbId);

        //Task<string?> AddMovieAsync(string id);

        //string? MoviePageUrl(string imdbId);
    }
}
