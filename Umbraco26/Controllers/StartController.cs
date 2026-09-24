using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco26.Models.ViewModels;

namespace Umbraco26.Controllers
{
    public class StartController(PageControllerDependencies dependencies)
        : BasePageController<Start, StartPageViewModel>(dependencies)
    {
        protected override StartPageViewModel Build(Start page) => new() { Content = page };
    }
}
