using Examine;
using Examine.Search;
using System.Globalization;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco26.Business.Services.Interfaces;
using Umbraco26.Models;

namespace Umbraco26.Business.Services
{
	public class FindService : IFindService
	{
		/// <summary>Pages that exist in the tree but aren't useful as search results.</summary>
		private static readonly string[] ExcludedContentTypes = [Error.ModelTypeAlias, Sitemap.ModelTypeAlias, Settings.ModelTypeAlias];

		private readonly IExamineManager _examineManager;
		private readonly IUmbracoContextFactory _umbracoContextFactory;
		private readonly IVariationContextAccessor _variationContextAccessor;
		private readonly IPublishedValueFallback _publishedValueFallback;

		public FindService(IExamineManager examineManager, IUmbracoContextFactory umbracoContextFactory, IVariationContextAccessor variationContextAccessor, IPublishedValueFallback publishedValueFallback)
		{
			_examineManager = examineManager;
			_umbracoContextFactory = umbracoContextFactory;
			_variationContextAccessor = variationContextAccessor;
			_publishedValueFallback = publishedValueFallback;
		}

		public FindResult FindContent(string query, CultureInfo cultureInfo, int page, int pageSize)
		{
			if (string.IsNullOrWhiteSpace(query) || !_examineManager.TryGetIndex(Constants.UmbracoIndexes.ExternalIndexName, out var index))
			{
				return FindResult.Empty;
			}

			// Pages whose type varies by culture are indexed per culture (nodeName_sv, metaDescription_en-us, ...),
			// invariant pages (e.g. articles) only under the plain field names. Variant pages also get the plain
			// nodeName (in the default language), so the plain fields only count for invariant pages.
			var culture = cultureInfo.Name.ToLowerInvariant();

			var results = index.Searcher
				.CreateQuery(IndexTypes.Content)
				.Group(text => text
					.ManagedQuery(query, [$"{UmbracoExamineFieldNames.NodeNameFieldName}_{culture}", $"metaDescription_{culture}"])
					.Or(invariant => invariant
						.ManagedQuery(query, [UmbracoExamineFieldNames.NodeNameFieldName, "metaDescription"])
						.And().Field(UmbracoExamineFieldNames.VariesByCultureFieldName, "n")))
				// Variant pages must be published in this language; invariant pages are in every language
				.And().Group(published => published
					.Field($"{UmbracoExamineFieldNames.PublishedFieldName}_{culture}", "y")
					.Or().Field(UmbracoExamineFieldNames.VariesByCultureFieldName, "n"))
				.And().Field(UmbracoExamineFieldNames.PublishedFieldName, "y")
				.Not().GroupedOr([ExamineFieldNames.ItemTypeFieldName], ExcludedContentTypes)
				.Execute(QueryOptions.SkipTake((Math.Max(page, 1) - 1) * pageSize, pageSize));

			// Blazor event handlers run outside an HTTP request, so there may be no Umbraco context yet.
			using var umbracoContextReference = _umbracoContextFactory.EnsureUmbracoContext();
			var content = umbracoContextReference.UmbracoContext.Content;
			var hits = new List<Hit>();

			foreach (var item in results)
			{
				var result = content?.GetById(int.Parse(item.Id));

				if (result == null)
				{
					continue;
				}

				hits.Add(new Hit
				{
					Name = result.Name(_variationContextAccessor, cultureInfo.Name),
					Description = result.Value<string>(_publishedValueFallback, "metaDescription", cultureInfo.Name) ?? string.Empty,
					Url = result.Url(cultureInfo.Name),
					UpdateDate = result.UpdateDate
				});
			}

			return new FindResult(hits, results.TotalItemCount);
		}
	}
}
