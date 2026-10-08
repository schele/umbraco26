using Umbraco.Cms.Infrastructure.Migrations;

namespace Umbraco26.Business.Migrations
{
    /// <summary>Content and settings changes for this site, applied once per environment on startup.</summary>
    public class SiteMigrationPlan : MigrationPlan
    {
        public SiteMigrationPlan() : base("Umbraco26")
        {
            From(string.Empty)
                .To<AddFindDictionaryItems>("add-find-dictionary-items")
                .To<AddContactFormDictionaryItems>("add-contact-form-dictionary-items")
                .To<AddContactFormBlock>("add-contact-form-block")
                // Adds the contact form texts that came after the first step; existing items are kept
                .To<AddContactFormDictionaryItems>("add-contact-form-dictionary-items-2")
                // Adds the menu's search link text; existing items are kept
                .To<AddFindDictionaryItems>("add-find-dictionary-items-2")
                .To<AddSettingsPage>("add-settings-page")
                .To<AddMenuDictionaryItems>("add-menu-dictionary-items")
                // Adds the reCAPTCHA texts; existing items are kept
                .To<AddContactFormDictionaryItems>("add-contact-form-dictionary-items-3")
                // Adds the send button's sending text; existing items are kept
                .To<AddContactFormDictionaryItems>("add-contact-form-dictionary-items-4")
                .To<AddMemberPages>("add-member-pages")
                .To<AddMemberDictionaryItems>("add-member-dictionary-items")
                .To<AddMetaRobots>("add-meta-robots");
        }
    }
}
