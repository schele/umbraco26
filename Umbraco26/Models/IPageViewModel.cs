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

        /// <summary>The find page the menu's search link goes to, or <c>null</c> when there is none to link to.</summary>
        string? FindPageUrl { get; }

        /// <summary>The login page the menu's log in link goes to, or <c>null</c> when there is none to link to.</summary>
        string? LoginPageUrl { get; }

        /// <summary>The My account page the menu links to once a member is logged in, or <c>null</c> when there is none.</summary>
        string? AccountPageUrl { get; }

        /// <summary>The pages in the top menu, in the order editors picked them on the settings page.</summary>
        IReadOnlyList<IPublishedContent> MenuItems { get; }

        string UrlSegment { get; }

        string PageTitle { get; }

        string MetaDescription { get; }
    }
}
