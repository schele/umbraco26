using Umbraco.Cms.Web.Common.PublishedModels;

namespace Umbraco26.Models
{
    /// <summary>
    /// The page-level surface shared by every view model, so layouts can bind to
    /// this instead of to <see cref="IBase"/> directly.
    /// </summary>
    public interface IPageViewModel
    {
        IBase Content { get; }

        Start? StartPage { get; }

        string UrlSegment { get; }
    }
}
