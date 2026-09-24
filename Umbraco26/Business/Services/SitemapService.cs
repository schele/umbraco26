using System.Text;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services.Navigation;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco26.Business.Extensions;
using Umbraco26.Business.Services.Interfaces;
using Umbraco26.Models.ViewModels;

namespace Umbraco26.Business.Services
{
    public class SitemapService(IUmbracoContextAccessor umbracoContextAccessor, IDocumentNavigationQueryService documentNavigationQueryService) : ISitemapService
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor = umbracoContextAccessor;

        private readonly IDocumentNavigationQueryService _documentNavigationQueryService = documentNavigationQueryService;

        public IEnumerable<IPublishedContent> Pages()
        {
            if (_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext))
            {
                var content = umbracoContext.Content;

                if (content != null)
                {
                    var startPage = _documentNavigationQueryService.TryGetRootKeys(out var rootKeys)
                        ? rootKeys.Select(key => content.GetById(key)).OfType<IPublishedContent>().DescendantsOrSelf<Start>().FirstOrDefault()
                        : null;                            

                    if (startPage != null)
                    {
                        return startPage.DescendantsOrSelf<IPublishedContent>()
                            .Where(page => page is IBase basePage && page.IsPublished()).ToList();
                    }
                }
            }

            return [];
        }

        public string GenerateXml(SitemapViewModel model)
        {
            if (_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext))
            {
                var currentCulture = umbracoContext.PublishedRequest?.Culture;
                var sb = new StringBuilder();

                var localizedPages = model.Pages
                    .Where(p => p.IsPublished(currentCulture))
                    .Select(p => new
                    {
                        Page = p,
                        Url = p.GetFullUrl(currentCulture),
                        LastModified = p.UpdateDate
                    })
                    .ToList();

                sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sb.AppendLine("<urlset xmlns=\"https://www.sitemaps.org/schemas/sitemap/0.9\">");

                foreach (var page in localizedPages)
                {
                    sb.AppendLine($"<url><loc>{page.Url}</loc><lastmod>{page.LastModified:yyyy-MM-dd HH:mm:ss}</lastmod></url>");
                }

                sb.AppendLine("</urlset>");

                return sb.ToString();
            }

            return string.Empty;
        }
    }
}