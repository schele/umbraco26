using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Umbraco26.Business.Migrations
{
    /// <summary>
    /// Creates the dictionary items the contact form block reads, in Swedish and English. Only missing items are
    /// created, so the plan runs it again (as a new step) when items are added to the list.
    /// </summary>
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
            ("ContactForm.Sending", "Skickar…", "Sending…"),
            ("ContactForm.Sent", "Tack! Vi har tagit emot ditt meddelande.", "Thanks! We have received your message."),
            ("ContactForm.Invalid", "Fyll i alla fält och använd en giltig e-postadress.", "Please fill in every field and use a valid email address."),
            ("ContactForm.Expired", "Formuläret har gått ut. Försök igen.", "The form expired. Please try again."),
            ("ContactForm.Honeypot", "Lämna det här fältet tomt", "Leave this field empty"),
            ("ContactForm.Required", "Alla fält måste fyllas i.", "All fields are required."),
            ("ContactForm.SendAnother", "Skicka ett till meddelande", "Send another message"),
            ("ContactForm.Unverified", "Vi kunde inte bekräfta att meddelandet skickades av en människa. Kontrollera att inget i webbläsaren blockerar Googles reCAPTCHA och försök igen.", "We couldn't confirm that a person sent the message. Check that nothing in your browser blocks Google's reCAPTCHA and try again."),
            // Google's required notice when its badge is hidden; {0} and {1} become links to its privacy policy and terms
            ("ContactForm.ReCaptchaNotice", "Formuläret skyddas av reCAPTCHA, och Googles {0} och {1} gäller.", "This form is protected by reCAPTCHA, and the Google {0} and {1} apply."),
            ("ContactForm.ReCaptchaPrivacy", "integritetspolicy", "Privacy Policy"),
            ("ContactForm.ReCaptchaTerms", "användarvillkor", "Terms of Service"),
        ];
    }
}
