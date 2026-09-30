using Umbraco.Cms.Web.Common.PublishedModels;

namespace Umbraco26.Models.ViewModels
{
    public class StartPageViewModel : PageViewModel<Start>
    {
        public CarouselViewModel? Carousel { get; init; }
    }
}
