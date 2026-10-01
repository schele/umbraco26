using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;
using Umbraco26.Business.Data;
using Umbraco26.Business.Notifications;
using Umbraco26.Business.Services;
using Umbraco26.Business.Services.Interfaces;

namespace Umbraco26.Composers
{
    public class MovieRatingComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddUmbracoDbContext<MovieRatingContext>((serviceProvider, options, _, _)
                => options.UseUmbracoDatabaseProvider(serviceProvider));

            builder.Services.AddScoped<IMovieRatingService, MovieRatingService>();

            builder.AddNotificationAsyncHandler<UmbracoApplicationStartedNotification, MovieRatingMigrationHandler>();
        }
    }
}
