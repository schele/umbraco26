using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco26.Models.ViewModels;

namespace Umbraco26.Controllers
{
    public class ErrorController(PageControllerDependencies dependencies)
        : BasePageController<Error, ErrorPageViewModel>(dependencies)
    {
        protected override ErrorPageViewModel Build(Error page) => new() { Content = page };
    }
}