using Umbraco.Cms.Web.Common.PublishedModels;

namespace Umbraco26.Models.ViewModels
{
    public class StartPageViewModel : PageViewModel<Start>
    {
        /// <summary>The Movie page that renders a single OMDb title from <c>?id={imdbId}</c>.</summary>
        public string? MoviePageUrl { get; init; }
    }
}
