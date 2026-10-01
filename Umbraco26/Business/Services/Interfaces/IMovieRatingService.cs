using Umbraco26.Models;

namespace Umbraco26.Business.Services.Interfaces
{
    public interface IMovieRatingService
    {
        Task<MovieRatingSummary> GetSummaryAsync(string imdbId, int page, int pageSize);

        Task<MovieRating> AddAsync(string imdbId, double score, string? name, string? comment);
    }
}
