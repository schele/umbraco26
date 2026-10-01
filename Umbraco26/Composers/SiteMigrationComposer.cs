using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Notifications;
using Umbraco26.Business.Notifications;

namespace Umbraco26.Composers
{
    public class SiteMigrationComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
            => builder.AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, SiteMigrationHandler>();
    }
}
