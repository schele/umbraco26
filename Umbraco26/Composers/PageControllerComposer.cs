using Umbraco.Cms.Core.Composing;
using Umbraco26.Controllers;

namespace Umbraco26.Composers
{
    public class PageControllerComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
            => builder.Services.AddScoped<PageControllerDependencies>();
    }
}
