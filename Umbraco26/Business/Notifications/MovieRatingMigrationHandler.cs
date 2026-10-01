using Microsoft.EntityFrameworkCore;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco26.Business.Data;

namespace Umbraco26.Business.Notifications
{
    /// <summary>Applies pending <see cref="MovieRatingContext"/> migrations when Umbraco has started.</summary>
    public class MovieRatingMigrationHandler(IDbContextFactory<MovieRatingContext> contextFactory)
        : INotificationAsyncHandler<UmbracoApplicationStartedNotification>
    {
        public async Task HandleAsync(UmbracoApplicationStartedNotification notification, CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

            if ((await context.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
            {
                await context.Database.MigrateAsync(cancellationToken);
            }
        }
    }
}
