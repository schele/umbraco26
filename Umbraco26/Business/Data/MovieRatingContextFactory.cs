using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Umbraco26.Business.Data
{
    /// <summary>
    /// Used by <c>dotnet ef</c> only, so migrations can be generated without booting Umbraco.
    /// At runtime the context uses the Umbraco connection string (see <c>MovieRatingComposer</c>).
    /// </summary>
    public class MovieRatingContextFactory : IDesignTimeDbContextFactory<MovieRatingContext>
    {
        public MovieRatingContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var options = new DbContextOptionsBuilder<MovieRatingContext>()
                .UseSqlServer(configuration.GetConnectionString("umbracoDbDSN"))
                .Options;

            return new MovieRatingContext(options);
        }
    }
}
