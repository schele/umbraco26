using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Umbraco26.Business.Migrations
{
    /// <summary>Creates the dictionary items the contact form block reads, in Swedish and English.</summary>
    public class AddContactFormDictionaryItems(IMigrationContext context, IDictionaryItemService dictionaryItemService, ILanguageService languageService)
        : DictionaryItemsMigrationBase(context, dictionaryItemService, languageService)
    {
        protected override string ParentKey => "ContactForm";

        protected override IEnumerable<(string Key, string Swedish, string English)> Items =>
        [
            ("ContactForm.Name", "Namn", "Name"),
            ("ContactForm.Email", "E-post", "Email"),
            ("ContactForm.Comment", "Kommentar", "Comment"),
            ("ContactForm.Send", "Skicka", "Send"),
            ("ContactForm.Sent", "Tack! Vi har tagit emot ditt meddelande.", "Thanks! We have received your message."),
            ("ContactForm.Invalid", "Fyll i alla fält och använd en giltig e-postadress.", "Please fill in every field and use a valid email address."),
            ("ContactForm.Expired", "Formuläret har gått ut. Försök igen.", "The form expired. Please try again."),
            ("ContactForm.Honeypot", "Lämna det här fältet tomt", "Leave this field empty"),
        ];
    }
}
