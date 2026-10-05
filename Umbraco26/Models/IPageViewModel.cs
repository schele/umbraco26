using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Umbraco26.Models
{
    /// <summary>
    /// The page-level surface shared by every view model, so layouts can bind to
    /// this instead of to the page's content model directly.
    /// </summary>
    public interface IPageViewModel
    {
        IPublishedContent Content { get; }

        Start? StartPage { get; }

        string UrlSegment { get; }

        string PageTitle { get; }

        string MetaDescription { get; }
    }
}
