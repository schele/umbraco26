using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services.Navigation;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco26.Models.ViewModels;

namespace Umbraco26.Controllers
{
    public class StartController(PageControllerDependencies dependencies, IDocumentNavigationQueryService documentNavigationQueryService)
        : BasePageController<Start, StartPageViewModel>(dependencies)
    {
        protected override StartPageViewModel Build(Start page)
        {
            List<IPublishedContent> roots = GetRoots();

            return new()
            {
                Content = page,
                MoviePageUrl = roots.DescendantsOrSelf<Movie>().FirstOrDefault()?.Url(),
            };
        }

        private List<IPublishedContent> GetRoots()
        {
            if (!Dependencies.UmbracoContextAccessor.TryGetUmbracoContext(out IUmbracoContext? umbracoContext)
                || umbracoContext.Content == null
                || !documentNavigationQueryService.TryGetRootKeys(out IEnumerable<Guid> rootKeys))
            {
                return [];
            }

            return rootKeys
                .Select(key => umbracoContext.Content.GetById(key))
                .OfType<IPublishedContent>()
                .ToList();
        }
    }
}
