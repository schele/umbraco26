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
                Carousel = BuildCarousel(roots),
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

        /// <summary>Builds the carousel from the Carousel pages under the first Carousel Container in the content tree.</summary>
        private static CarouselViewModel? BuildCarousel(List<IPublishedContent> roots)
        {
            CarouselContainer? container = roots.DescendantsOrSelf<CarouselContainer>().FirstOrDefault();

            List<CarouselSlide> slides = container?.Children<Carousel>()?
                .Where(slide => slide.Image != null)
                .Select(slide => new CarouselSlide(slide.Image!, slide.Name))
                .ToList() ?? [];

            return slides.Count == 0 ? null : new CarouselViewModel { Id = $"carousel-{container!.Key:N}", Slides = slides };
        }
    }
}
