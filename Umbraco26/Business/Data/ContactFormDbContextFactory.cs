using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Umbraco26.Business.Data
{
    /// <summary>
    /// Used by <c>dotnet ef</c> only, so migrations can be generated without booting Umbraco. The site runs on
    /// SQLite, so the migrations are generated for SQLite. At runtime the context uses the Umbraco connection
    /// string and provider (see <c>ContactFormComposer</c>).
    /// </summary>
    public class ContactFormDbContextFactory : IDesignTimeDbContextFactory<ContactFormDbContext>
    {
        public ContactFormDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("umbracoDbDSN")?
                .Replace("|DataDirectory|", Path.Combine(Directory.GetCurrentDirectory(), "umbraco", "Data"));

            var options = new DbContextOptionsBuilder<ContactFormDbContext>()
                .UseSqlite(connectionString)
                .Options;

            return new ContactFormDbContext(options);
        }
    }
}
