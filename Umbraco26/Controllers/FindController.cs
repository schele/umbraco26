using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco26.Models.ViewModels;

namespace Umbraco26.Controllers
{
    public class FindController(PageControllerDependencies dependencies)
        : BasePageController<Find, FindPageViewModel>(dependencies)
    {
        protected override FindPageViewModel Build(Find page) => new() { Content = page };
    }
}