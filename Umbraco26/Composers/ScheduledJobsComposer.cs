using Hangfire;
using Umbraco.Cms.Core.Composing;
using Umbraco26.Business.Services.Interfaces;

namespace Umbraco26.Composers
{
    public class ScheduledJobsComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            RecurringJob.AddOrUpdate<IMoviesJob>("Add movies", x => x.AddMovies(null), Cron.Never);
        }
    }
}