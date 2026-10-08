using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Umbraco26.Models
{
    public class PageViewModel<T> : IPageViewModel where T : class, IPublishedContent
    {
        public required T Content { get; init; }

        IPublishedContent IPageViewModel.Content => Content;

        public Start? StartPage => Content.AncestorOrSelf<Start>();

        /// <summary>The first find page under the start page that is published in the current culture, so visitors are never sent to a 404.</summary>
        public string? FindPageUrl => StartPage?.FirstChild<Find>()?.Url();

        /// <summary>The first login page under the start page that is published in the current culture.</summary>
        public string? LoginPageUrl => StartPage?.FirstChild<Login>()?.Url();

        /// <summary>The first My account page under the start page that is published in the current culture.</summary>
        public string? AccountPageUrl => StartPage?.FirstChild<Account>()?.Url();

        /// <summary>The pages picked on the start page's settings page, without those not published in the current culture.</summary>
        public IReadOnlyList<IPublishedContent> MenuItems
            => StartPage?.FirstChild<Settings>()?.MenuItems?.Where(page => page.IsPublished()).ToList() ?? [];

        public virtual string PageTitle => Content.Name;

        /// <summary>Pages without the <c>base</c> composition (such as Article) have no meta description.</summary>
        public virtual string MetaDescription => (Content as IBase)?.MetaDescription ?? string.Empty;

        /// <summary>Populated by <c>BasePageController</c> when the model is built.</summary>
        public string UrlSegment { get; set; } = string.Empty;
    }
}
