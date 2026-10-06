using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Umbraco26.Business.Migrations
{
    /// <summary>Creates the dictionary items the top menu reads, in Swedish and English.</summary>
    public class AddMenuDictionaryItems(IMigrationContext context, IDictionaryItemService dictionaryItemService, ILanguageService languageService)
        : DictionaryItemsMigrationBase(context, dictionaryItemService, languageService)
    {
        protected override string ParentKey => "Menu";

        protected override IEnumerable<(string Key, string Swedish, string English)> Items =>
        [
            ("Menu.Label", "Huvudmeny", "Main menu"),
            ("Menu.Toggle", "Visa eller dölj menyn", "Show or hide the menu"),
        ];
    }
}
