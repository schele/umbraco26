using Microsoft.EntityFrameworkCore;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco26.Business.Data;

namespace Umbraco26.Business.Notifications
{
    /// <summary>Applies pending <see cref="ContactFormDbContext"/> migrations when Umbraco has started.</summary>
    public class ContactFormMigrationHandler(
        IRuntimeState runtimeState,
        IDbContextFactory<ContactFormDbContext> contextFactory,
        ILogger<ContactFormMigrationHandler> logger)
        : INotificationAsyncHandler<UmbracoApplicationStartedNotification>
    {
        public async Task HandleAsync(UmbracoApplicationStartedNotification notification, CancellationToken cancellationToken)
        {
            // Nothing to migrate into until Umbraco is installed and upgraded
            if (runtimeState.Level < RuntimeLevel.Run)
            {
                return;
            }

            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

            var pending = (await context.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();

            if (pending.Count > 0)
            {
                logger.LogInformation("Applying contact form migrations: {Migrations}", pending);
                await context.Database.MigrateAsync(cancellationToken);
            }
        }
    }
}
