using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco26.Models.ViewModels;

namespace Umbraco26.Controllers
{
    public class ArticleController(PageControllerDependencies dependencies)
        : BasePageController<Article, ArticlePageViewModel>(dependencies)
    {
        protected override ArticlePageViewModel Build(Article page) => new() { Content = page };
    }
}
