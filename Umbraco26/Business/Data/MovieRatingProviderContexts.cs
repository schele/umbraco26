using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Umbraco26.Business.Data
{
    /// <summary>
    /// Owns the SQLite migrations of <see cref="MovieRatingContext"/>. EF migrations are tied to a database provider
    /// and Umbraco runs on SQLite or SQL Server, so there is one set of migrations per provider, told apart by a
    /// context type per provider. The site itself uses <see cref="MovieRatingContext"/>; only
    /// <c>MovieRatingMigrationHandler</c> uses these. A model change needs a migration for each provider:
    /// <code>
    /// dotnet ef migrations add Name --context SqliteMovieRatingContext --output-dir Business/Data/Migrations/MovieRatings/Sqlite
    /// dotnet ef migrations add Name --context SqlServerMovieRatingContext --output-dir Business/Data/Migrations/MovieRatings/SqlServer
    /// </code>
    /// </summary>
    public class SqliteMovieRatingContext(DbContextOptions<MovieRatingContext> options) : MovieRatingContext(options)
    {
    }

    /// <summary>Owns the SQL Server migrations of <see cref="MovieRatingContext"/>, see <see cref="SqliteMovieRatingContext"/>.</summary>
    public class SqlServerMovieRatingContext(DbContextOptions<MovieRatingContext> options) : MovieRatingContext(options)
    {
    }

    /// <summary>
    /// Used by <c>dotnet ef</c> only, so migrations can be generated without booting Umbraco. Generating a migration
    /// needs no database, so there is no connection string.
    /// </summary>
    public class SqliteMovieRatingContextFactory : IDesignTimeDbContextFactory<SqliteMovieRatingContext>
    {
        public SqliteMovieRatingContext CreateDbContext(string[] args)
            => new(new DbContextOptionsBuilder<MovieRatingContext>().UseSqlite().Options);
    }

    /// <inheritdoc cref="SqliteMovieRatingContextFactory"/>
    public class SqlServerMovieRatingContextFactory : IDesignTimeDbContextFactory<SqlServerMovieRatingContext>
    {
        public SqlServerMovieRatingContext CreateDbContext(string[] args)
            => new(new DbContextOptionsBuilder<MovieRatingContext>().UseSqlServer().Options);
    }
}
