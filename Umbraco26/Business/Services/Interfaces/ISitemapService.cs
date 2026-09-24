using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco26.Models.ViewModels;

namespace Umbraco26.Business.Services.Interfaces
{
    public interface ISitemapService
    {
        IEnumerable<IPublishedContent> Pages();

        string GenerateXml(SitemapViewModel model);
    }
}