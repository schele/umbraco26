using Umbraco.Cms.Web.Common.PublishedModels;

namespace Umbraco26.Models
{
    public class PageViewModel<T> : IPageViewModel where T : class, IBase
    {
        public required T Content { get; init; }

        IBase IPageViewModel.Content => Content;

        public Start? StartPage => Content.AncestorOrSelf<Start>();

        /// <summary>Populated by <c>BasePageController</c> when the model is built.</summary>
        public string UrlSegment { get; set; } = string.Empty;
    }
}
