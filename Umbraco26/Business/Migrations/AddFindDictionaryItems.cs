using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Umbraco26.Business.Migrations
{
    /// <summary>Creates the dictionary items the find page reads, in Swedish and English.</summary>
    public class AddFindDictionaryItems(IMigrationContext context, IDictionaryItemService dictionaryItemService, ILanguageService languageService)
        : DictionaryItemsMigrationBase(context, dictionaryItemService, languageService)
    {
        protected override string ParentKey => "Find";

        protected override IEnumerable<(string Key, string Swedish, string English)> Items =>
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
    }
}
