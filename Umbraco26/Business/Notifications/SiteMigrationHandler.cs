using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using Umbraco26.Business.Migrations;

namespace Umbraco26.Business.Notifications
{
    /// <summary>Runs the pending steps of <see cref="SiteMigrationPlan"/> once Umbraco is installed.</summary>
    public class SiteMigrationHandler(
        IRuntimeState runtimeState,
        IMigrationPlanExecutor migrationPlanExecutor,
        ICoreScopeProvider coreScopeProvider,
        IKeyValueService keyValueService,
        AppCaches appCaches)
        : INotificationAsyncHandler<UmbracoApplicationStartingNotification>
    {
        public async Task HandleAsync(UmbracoApplicationStartingNotification notification, CancellationToken cancellationToken)
        {
            if (runtimeState.Level < RuntimeLevel.Run)
            {
                return;
            }

            var result = await new Upgrader(new SiteMigrationPlan()).ExecuteAsync(migrationPlanExecutor, coreScopeProvider, keyValueService);

            // Migrations run with notifications suppressed, so the dictionary cache isn't refreshed for
            // items they create, and the site would show the fallback texts until the next restart.
            if (result.CompletedTransitions.Count > 0)
            {
                appCaches.IsolatedCaches.ClearCache<IDictionaryItem>();
            }
        }
    }
}
