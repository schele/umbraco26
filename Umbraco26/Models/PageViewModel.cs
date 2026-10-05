using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Umbraco26.Models
{
    public class PageViewModel<T> : IPageViewModel where T : class, IPublishedContent
    {
        public required T Content { get; init; }

        IPublishedContent IPageViewModel.Content => Content;

        public Start? StartPage => Content.AncestorOrSelf<Start>();

        public virtual string PageTitle => Content.Name;

        /// <summary>Pages without the <c>base</c> composition (such as Article) have no meta description.</summary>
        public virtual string MetaDescription => (Content as IBase)?.MetaDescription ?? string.Empty;

        /// <summary>Populated by <c>BasePageController</c> when the model is built.</summary>
        public string UrlSegment { get; set; } = string.Empty;
    }
}
