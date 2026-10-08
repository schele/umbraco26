using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco26.Models;

namespace Umbraco26.Business.Migrations
{
    /// <summary>
    /// Sets up Meta Robots: the "Meta Robots" data type, which uses the property editor UI in
    /// wwwroot/App_Plugins/MetaRobots, and a <c>metaRobots</c> property on the SEO tab of the <c>base</c> composition.
    /// Every step checks what is already there, so nothing editors created or changed is overwritten.
    /// </summary>
    /// <remarks>
    /// Runs unscoped for the same reasons as <see cref="AddContactFormBlock"/>: the content type change needs
    /// notifications to reach the published cache, and they make ModelsBuilder regenerate models. The property is
    /// invariant, as whether search engines may index a page is the same in every language. Existing pages get no
    /// value, which counts as <see cref="MetaRobotsValues.All"/>.
    /// </remarks>
    public class AddMetaRobots(
        IMigrationContext context,
        ICoreScopeProvider scopeProvider,
        IContentTypeService contentTypeService,
        IDataTypeService dataTypeService,
        PropertyEditorCollection propertyEditors,
        IConfigurationEditorJsonSerializer configurationEditorJsonSerializer,
        IShortStringHelper shortStringHelper)
        : UnscopedAsyncMigrationBase(context)
    {
        public const string BaseTypeAlias = "base";
        public const string MetaRobotsPropertyAlias = "metaRobots";
        public const string DataTypeName = "Meta Robots";
        public const string EditorUiAlias = "Umbraco26.PropertyEditorUi.MetaRobots";

        private const string SeoTabAlias = "seo";
        private const string SeoTabName = "SEO";

        private static readonly Guid UserKey = Constants.Security.SuperUserKey;

        protected override async Task MigrateAsync()
        {
            using (var scope = scopeProvider.CreateCoreScope())
            {
                var dataType = await EnsureDataTypeAsync();
                await EnsureBasePropertyAsync(dataType);

                scope.Complete();
            }

            // Unscoped migrations have to say they are done, which records the plan's new state
            Context.Complete();
        }

        private async Task<IDataType> EnsureDataTypeAsync()
        {
            var existing = await dataTypeService.GetAsync(DataTypeName);

            if (existing != null)
            {
                return existing;
            }

            if (!propertyEditors.TryGet(Constants.PropertyEditors.Aliases.PlainString, out var plainString))
            {
                throw new InvalidOperationException("The Plain String property editor is not available.");
            }

            // The values and the default are in the editor UI, so there is nothing to configure
            var dataType = new DataType(plainString, configurationEditorJsonSerializer)
            {
                Name = DataTypeName,
                EditorUiAlias = EditorUiAlias,
                DatabaseType = ValueStorageType.Ntext,
            };

            var attempt = await dataTypeService.CreateAsync(dataType, UserKey);

            if (!attempt.Success)
            {
                throw new InvalidOperationException($"Could not create the '{DataTypeName}' data type: {attempt.Status}");
            }

            return attempt.Result;
        }

        private async Task EnsureBasePropertyAsync(IDataType metaRobotsDataType)
        {
            var baseType = contentTypeService.Get(BaseTypeAlias);

            if (baseType == null)
            {
                Logger.LogWarning("No {Alias} composition found, so the {Property} property was not added.", BaseTypeAlias, MetaRobotsPropertyAlias);
                return;
            }

            if (baseType.PropertyTypeExists(MetaRobotsPropertyAlias))
            {
                return;
            }

            // The tab the backoffice made is aliased "sEO", so it's found by name
            var seoTab = baseType.PropertyGroups.FirstOrDefault(
                x => x.Type == PropertyGroupType.Tab && string.Equals(x.Name, SeoTabName, StringComparison.OrdinalIgnoreCase));

            if (seoTab == null)
            {
                baseType.AddPropertyGroup(SeoTabAlias, SeoTabName);
                seoTab = baseType.PropertyGroups[SeoTabAlias];
                seoTab.Type = PropertyGroupType.Tab;
            }

            var metaRobots = new PropertyType(shortStringHelper, metaRobotsDataType, MetaRobotsPropertyAlias)
            {
                Name = "Meta Robots",
                Description = "NONE asks search engines not to index the page and leaves it out of the sitemap.",
                Variations = ContentVariation.Nothing,
                // Below the tab's other properties, like Meta Description
                SortOrder = seoTab.PropertyTypes?.Select(x => x.SortOrder + 1).DefaultIfEmpty(0).Max() ?? 0,
            };

            baseType.AddPropertyType(metaRobots, seoTab.Alias);

            var attempt = await contentTypeService.UpdateAsync(baseType, UserKey);

            if (!attempt.Success)
            {
                throw new InvalidOperationException($"Could not add '{MetaRobotsPropertyAlias}' to '{BaseTypeAlias}': {attempt.Result}");
            }
        }
    }
}
