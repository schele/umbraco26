using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Umbraco26.Business.Data
{
    /// <summary>
    /// Owns the SQLite migrations of <see cref="ContactFormDbContext"/>. EF migrations are tied to a database provider
    /// and Umbraco runs on SQLite or SQL Server, so there is one set of migrations per provider, told apart by a
    /// context type per provider. The site itself uses <see cref="ContactFormDbContext"/>; only
    /// <c>ContactFormMigrationHandler</c> uses these. A model change needs a migration for each provider:
    /// <code>
    /// dotnet ef migrations add Name --context SqliteContactFormDbContext --output-dir Business/Data/Migrations/ContactForm/Sqlite
    /// dotnet ef migrations add Name --context SqlServerContactFormDbContext --output-dir Business/Data/Migrations/ContactForm/SqlServer
    /// </code>
    /// </summary>
    public class SqliteContactFormDbContext(DbContextOptions<ContactFormDbContext> options) : ContactFormDbContext(options)
    {
    }

    /// <summary>Owns the SQL Server migrations of <see cref="ContactFormDbContext"/>, see <see cref="SqliteContactFormDbContext"/>.</summary>
    public class SqlServerContactFormDbContext(DbContextOptions<ContactFormDbContext> options) : ContactFormDbContext(options)
    {
    }

    /// <summary>
    /// Used by <c>dotnet ef</c> only, so migrations can be generated without booting Umbraco. Generating a migration
    /// needs no database, so there is no connection string.
    /// </summary>
    public class SqliteContactFormDbContextFactory : IDesignTimeDbContextFactory<SqliteContactFormDbContext>
    {
        public SqliteContactFormDbContext CreateDbContext(string[] args)
            => new(new DbContextOptionsBuilder<ContactFormDbContext>().UseSqlite().Options);
    }

    /// <inheritdoc cref="SqliteContactFormDbContextFactory"/>
    public class SqlServerContactFormDbContextFactory : IDesignTimeDbContextFactory<SqlServerContactFormDbContext>
    {
        public SqlServerContactFormDbContext CreateDbContext(string[] args)
            => new(new DbContextOptionsBuilder<ContactFormDbContext>().UseSqlServer().Options);
    }
}
