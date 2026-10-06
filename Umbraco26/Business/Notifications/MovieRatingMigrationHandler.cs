using Microsoft.EntityFrameworkCore;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco26.Business.Data;

namespace Umbraco26.Business.Notifications
{
    /// <summary>
    /// Applies pending <see cref="MovieRatingContext"/> migrations when Umbraco has started, using the set for the
    /// database provider Umbraco runs on (see <see cref="SqliteMovieRatingContext"/>).
    /// </summary>
    public class MovieRatingMigrationHandler(
        IRuntimeState runtimeState,
        IDbContextFactory<MovieRatingContext> contextFactory,
        DbContextOptions<MovieRatingContext> options,
        ILogger<MovieRatingMigrationHandler> logger)
        : INotificationAsyncHandler<UmbracoApplicationStartedNotification>
    {
        public async Task HandleAsync(UmbracoApplicationStartedNotification notification, CancellationToken cancellationToken)
        {
            // Nothing to migrate into until Umbraco is installed and upgraded
            if (runtimeState.Level < RuntimeLevel.Run)
            {
                return;
            }

            // A failure here must not stop the site, nor the other startup handlers, from starting
            try
            {
                await using var siteContext = await contextFactory.CreateDbContextAsync(cancellationToken);

                await using MovieRatingContext context = siteContext.Database.IsSqlServer()
                    ? new SqlServerMovieRatingContext(options)
                    : new SqliteMovieRatingContext(options);

                var pending = (await context.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();

                if (pending.Count > 0)
                {
                    logger.LogInformation("Applying movie rating migrations: {Migrations}", pending);
                    await context.Database.MigrateAsync(cancellationToken);
                    logger.LogInformation("Applied movie rating migrations.");
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Could not apply the movie rating migrations, so movie ratings can't be stored until this is fixed.");
            }
        }
    }
}
