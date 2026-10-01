using Microsoft.EntityFrameworkCore;
using Umbraco.Cms.Persistence.EFCore.Scoping;
using Umbraco26.Business.Data;
using Umbraco26.Business.Services.Interfaces;
using Umbraco26.Models;

namespace Umbraco26.Business.Services
{
    public class MovieRatingService(IEFCoreScopeProvider<MovieRatingContext> scopeProvider) : IMovieRatingService
    {
        public async Task<MovieRatingSummary> GetSummaryAsync(string imdbId, int page, int pageSize)
        {
            using var scope = scopeProvider.CreateScope();

            var summary = await scope.ExecuteWithContextAsync(async db =>
            {
                var ratings = db.MovieRatings.AsNoTracking().Where(x => x.ImdbId == imdbId);

                // Averaged client side so the query works the same on SQL Server and SQLite
                var scores = await ratings.Select(x => x.Score).ToListAsync();
                var pageRatings = await ratings
                    .OrderByDescending(x => x.CreatedUtc)
                    .Skip((Math.Max(page, 1) - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new MovieRatingSummary
                {
                    Average = scores.Count > 0 ? Math.Round(scores.Average(), 1) : null,
                    Count = scores.Count,
                    Ratings = pageRatings,
                };
            });

            scope.Complete();

            return summary;
        }

        public async Task<MovieRating> AddAsync(string imdbId, double score, string? name, string? comment)
        {
            if (string.IsNullOrWhiteSpace(imdbId))
            {
                throw new ArgumentException("An IMDb id is required.", nameof(imdbId));
            }

            // Snap to half stars within range, whatever the client sent
            score = Math.Clamp(Math.Round(score * 2, MidpointRounding.AwayFromZero) / 2, MovieRating.MinScore, MovieRating.MaxScore);

            var rating = new MovieRating
            {
                Id = Guid.NewGuid(),
                ImdbId = imdbId.Trim(),
                Score = score,
                Name = Truncate(name, MovieRating.NameMaxLength),
                Comment = Truncate(comment, MovieRating.CommentMaxLength),
                CreatedUtc = DateTime.UtcNow,
            };

            using var scope = scopeProvider.CreateScope();

            await scope.ExecuteWithContextAsync<Task>(async db =>
            {
                db.MovieRatings.Add(rating);
                await db.SaveChangesAsync();
            });

            scope.Complete();

            return rating;
        }

        private static string? Truncate(string? value, int maxLength)
        {
            value = value?.Trim();

            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            return value.Length > maxLength ? value[..maxLength] : value;
        }
    }
}
