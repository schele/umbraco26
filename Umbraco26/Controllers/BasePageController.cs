using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco26.Models;

namespace Umbraco26.Controllers
{
    /// <summary>
    /// Absorbs the <see cref="RenderController"/> plumbing and the "is the current page the
    /// type I expect" check, so a page controller only has to build its view model.
    /// </summary>
    public abstract class BasePageController<TContent, TViewModel>(PageControllerDependencies dependencies)
        : RenderController(dependencies.Logger, dependencies.ViewEngine, dependencies.UmbracoContextAccessor)
        where TContent : class, IBase
        where TViewModel : PageViewModel<TContent>
    {
        protected PageControllerDependencies Dependencies { get; } = dependencies;

        public override IActionResult Index()
        {
            if (CurrentPage is not TContent page)
            {
                return NotFound();
            }

            TViewModel model = Build(page);
            model.UrlSegment = GetUrlSegment(page);

            return CurrentTemplate(model);
        }

        protected abstract TViewModel Build(TContent page);

        private string GetUrlSegment(IBase content)
        {
            string culture = Dependencies.VariationContextAccessor.VariationContext?.Culture ?? string.Empty;
            bool isDraft = Dependencies.UmbracoContextAccessor.TryGetUmbracoContext(out IUmbracoContext? umbracoContext)
                && umbracoContext.InPreviewMode;

            return Dependencies.DocumentUrlService.GetUrlSegment(content.Key, culture, isDraft) ?? string.Empty;
        }
    }
}
