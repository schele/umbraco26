using Umbraco.Cms.Web.Common.PublishedModels;

namespace Umbraco26.Models.ViewModels
{
    public class MoviePageViewModel : PageViewModel<Movie>
    {
        public OmdbMovieDetails? Movie { get; init; }

        public override string PageTitle => Movie?.Title ?? base.PageTitle;

        public override string MetaDescription => Movie?.Plot ?? base.MetaDescription;
    }
}
