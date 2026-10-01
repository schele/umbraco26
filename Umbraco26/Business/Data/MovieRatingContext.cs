using Microsoft.EntityFrameworkCore;
using Umbraco26.Models;

namespace Umbraco26.Business.Data
{
    public class MovieRatingContext(DbContextOptions<MovieRatingContext> options) : DbContext(options)
    {
        public DbSet<MovieRating> MovieRatings => Set<MovieRating>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MovieRating>(rating =>
            {
                rating.ToTable("movieFinderRating");
                rating.HasKey(x => x.Id);
                rating.Property(x => x.ImdbId).HasMaxLength(20).IsRequired();
                rating.Property(x => x.Name).HasMaxLength(MovieRating.NameMaxLength);
                rating.Property(x => x.Comment).HasMaxLength(MovieRating.CommentMaxLength);
                rating.HasIndex(x => x.ImdbId);
            });
        }
    }
}
