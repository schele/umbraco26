using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco26.Models.ViewModels;

namespace Umbraco26.Controllers
{
    public class StartController(PageControllerDependencies dependencies)
        : BasePageController<Start, StartPageViewModel>(dependencies)
    {
        protected override StartPageViewModel Build(Start page) => new()
        {
            Content = page,
            Carousel = BuildCarousel(page),
        };

        private static CarouselViewModel? BuildCarousel(Start page)
        {
            List<CarouselSlide> slides = page.Descendants<Carousel>()
                .Where(carousel => carousel.Image != null)
                .Select(carousel => new CarouselSlide(carousel.Image!, carousel.Name))
                .ToList();

            return slides.Count > 0
                ? new CarouselViewModel { Id = "start-carousel", Slides = slides }
                : null;
        }
    }
}
