using Umbraco.Cms.Infrastructure.Migrations;

namespace Umbraco26.Business.Migrations
{
    /// <summary>Content and settings changes for this site, applied once per environment on startup.</summary>
    public class SiteMigrationPlan : MigrationPlan
    {
        public SiteMigrationPlan() : base("Umbraco26")
        {
            From(string.Empty)
                .To<AddFindDictionaryItems>("add-find-dictionary-items");
        }
    }
}
