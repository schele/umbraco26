using Microsoft.Extensions.DependencyInjection.Extensions;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;
using Umbraco26.Business.Data;
using Umbraco26.Business.Notifications;
using Umbraco26.Business.Services;
using Umbraco26.Business.Services.Interfaces;

namespace Umbraco26.Composers
{
    /// <summary>The contact form block's services.</summary>
    public class ContactFormComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            // Submissions live in the Umbraco database, so share its connection and transaction
            builder.Services.AddUmbracoDbContext<ContactFormDbContext>(
                (serviceProvider, options, _, _) => options.UseUmbracoDatabaseProvider(serviceProvider),
                shareUmbracoConnection: true);

            builder.Services.TryAddSingleton(TimeProvider.System);
            builder.Services.AddSingleton<ContactSubmissionEmailProtector>();
            builder.Services.AddScoped<IContactSubmissionService, ContactSubmissionService>();

            builder.AddNotificationAsyncHandler<UmbracoApplicationStartedNotification, ContactFormMigrationHandler>();
        }
    }
}
