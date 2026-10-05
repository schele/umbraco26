using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Umbraco26.Business.Migrations
{
    /// <summary>
    /// Creates dictionary items under a parent item, in Swedish and English. Existing items are left
    /// untouched, so texts edited in the backoffice are never overwritten.
    /// </summary>
    public abstract class DictionaryItemsMigrationBase(IMigrationContext context, IDictionaryItemService dictionaryItemService, ILanguageService languageService)
        : AsyncMigrationBase(context)
    {
        protected abstract string ParentKey { get; }

        protected abstract IEnumerable<(string Key, string Swedish, string English)> Items { get; }

        protected override async Task MigrateAsync()
        {
            var languages = (await languageService.GetAllAsync()).ToList();
            var swedish = languages.FirstOrDefault(x => x.IsoCode.StartsWith("sv", StringComparison.OrdinalIgnoreCase));
            var english = languages.FirstOrDefault(x => x.IsoCode.StartsWith("en", StringComparison.OrdinalIgnoreCase));

            var parent = await dictionaryItemService.GetAsync(ParentKey)
                ?? await CreateAsync(new DictionaryItem(ParentKey));

            foreach (var (key, swedishValue, englishValue) in Items)
            {
                if (await dictionaryItemService.ExistsAsync(key))
                {
                    continue;
                }

                var translations = new List<IDictionaryTranslation>();

                if (swedish != null)
                {
                    translations.Add(new DictionaryTranslation(swedish, swedishValue));
                }

                if (english != null)
                {
                    translations.Add(new DictionaryTranslation(english, englishValue));
                }

                await CreateAsync(new DictionaryItem(parent.Key, key) { Translations = translations });
            }
        }

        private async Task<IDictionaryItem> CreateAsync(IDictionaryItem item)
        {
            var attempt = await dictionaryItemService.CreateAsync(item, Constants.Security.SuperUserKey);

            if (!attempt.Success)
            {
                throw new InvalidOperationException($"Could not create dictionary item '{item.ItemKey}': {attempt.Status}");
            }

            return attempt.Result;
        }
    }
}
