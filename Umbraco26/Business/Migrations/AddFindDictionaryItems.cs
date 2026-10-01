using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Umbraco26.Business.Migrations
{
    /// <summary>
    /// Creates the dictionary items the find page reads, in Swedish and English. Existing items are left
    /// untouched, so texts edited in the backoffice are never overwritten.
    /// </summary>
    public class AddFindDictionaryItems(IMigrationContext context, IDictionaryItemService dictionaryItemService, ILanguageService languageService)
        : AsyncMigrationBase(context)
    {
        private const string ParentKey = "Find";

        private static readonly (string Key, string Swedish, string English)[] Items =
        [
            ("Find.Heading", "Sök på webbplatsen", "Search the site"),
            ("Find.Lead", "Hitta sidor utifrån titel eller beskrivning.", "Find pages by their title or description."),
            ("Find.Placeholder", "Vad letar du efter?", "What are you looking for?"),
            ("Find.Submit", "Sök", "Search"),
            ("Find.Searching", "Söker…", "Searching…"),
            ("Find.ResultsFor", "Resultat för", "Results for"),
            ("Find.Hit", "träff", "hit"),
            ("Find.Hits", "träffar", "hits"),
            ("Find.ReadMore", "Läs mer", "Read more"),
            ("Find.Pagination", "Sökresultatsidor", "Search result pages"),
            ("Find.Previous", "Föregående", "Previous"),
            ("Find.Next", "Nästa", "Next"),
            ("Find.StartTyping", "Skriv vad du letar efter ovan för att söka.", "Type what you're looking for above to search."),
        ];

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
