using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace Umbraco26.Controllers
{
    /// <summary>
    /// Bundles everything <see cref="BasePageController{TContent, TViewModel}"/> needs so that
    /// derived controllers take a single constructor parameter instead of repeating the
    /// <see cref="RenderController"/> constructor chain.
    /// </summary>
    public sealed class PageControllerDependencies(
        ILogger<RenderController> logger,
        ICompositeViewEngine viewEngine,
        IUmbracoContextAccessor umbracoContextAccessor,
        IVariationContextAccessor variationContextAccessor,
        IDocumentUrlService documentUrlService)
    {
        public ILogger<RenderController> Logger { get; } = logger;

        public ICompositeViewEngine ViewEngine { get; } = viewEngine;

        public IUmbracoContextAccessor UmbracoContextAccessor { get; } = umbracoContextAccessor;

        public IVariationContextAccessor VariationContextAccessor { get; } = variationContextAccessor;

        public IDocumentUrlService DocumentUrlService { get; } = documentUrlService;
    }
}
