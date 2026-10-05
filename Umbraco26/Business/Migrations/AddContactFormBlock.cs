using System.Text.Json;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Umbraco26.Business.Migrations
{
    /// <summary>
    /// Sets up the contact form block: the <c>contactFormBlock</c> element type, the "Article blocks" Block List
    /// data type, a <c>blocks</c> property on Article and a published "Contact" article under the start page.
    /// Every step checks what is already there, so nothing editors created or changed is overwritten.
    /// </summary>
    /// <remarks>
    /// Runs unscoped, i.e. with notifications, unlike scoped migrations: creating and publishing content needs
    /// them to update URLs, navigation and the published cache, and they make ModelsBuilder regenerate models.
    /// The work still happens in a single scope, so a failure rolls everything back and the step runs again.
    /// The element type is invariant because Article is: block level variance needs a culture-variant document.
    /// </remarks>
    public class AddContactFormBlock(
        IMigrationContext context,
        ICoreScopeProvider scopeProvider,
        IContentTypeService contentTypeService,
        IDataTypeService dataTypeService,
        ITemplateService templateService,
        IContentService contentService,
        PropertyEditorCollection propertyEditors,
        IConfigurationEditorJsonSerializer configurationEditorJsonSerializer,
        IShortStringHelper shortStringHelper,
        IHostEnvironment hostEnvironment)
        : UnscopedAsyncMigrationBase(context)
    {
        public const string ElementTypeAlias = "contactFormBlock";
        public const string DataTypeName = "Article blocks";
        public const string ArticleTypeAlias = "article";
        public const string BlocksPropertyAlias = "blocks";
        public const string ContactPageName = "Contact";

        private const string StartTypeAlias = "start";
        private const string BlocksContainerName = "Blocks";
        private const string ContentGroupAlias = "content";
        private const string ContentGroupName = "Content";

        private static readonly Guid UserKey = Constants.Security.SuperUserKey;

        /// <summary>camelCase property names, like the backoffice; dictionary keys (the layout's editor alias) are kept as they are.</summary>
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        protected override async Task MigrateAsync()
        {
            using (var scope = scopeProvider.CreateCoreScope())
            {
                var elementType = await EnsureElementTypeAsync();
                var dataType = await EnsureDataTypeAsync(elementType);
                var articleType = await EnsureArticleTypeAsync(dataType);
                await EnsureArticleAllowedUnderStartAsync(articleType);
                EnsureContactPage(articleType, elementType);

                scope.Complete();
            }

            // Unscoped migrations have to say they are done, which records the plan's new state
            Context.Complete();
        }

        private async Task<IContentType> EnsureElementTypeAsync()
        {
            var existing = contentTypeService.Get(ElementTypeAlias);

            if (existing != null)
            {
                return existing;
            }

            var textstring = await GetDataTypeAsync(Constants.DataTypes.Guids.TextstringGuid);
            var textarea = await GetDataTypeAsync(Constants.DataTypes.Guids.TextareaGuid);

            // Next to the other block element types, if there is such a folder
            var container = contentTypeService.GetContainers(BlocksContainerName, 1).FirstOrDefault();

            var elementType = new ContentType(shortStringHelper, container?.Id ?? Constants.System.Root)
            {
                Alias = ElementTypeAlias,
                Name = "Contact form block",
                Icon = "icon-message",
                IsElement = true,
                Variations = ContentVariation.Nothing,
            };

            elementType.AddPropertyType(
                new PropertyType(shortStringHelper, textstring, "heading") { Name = "Heading", SortOrder = 0 },
                ContentGroupAlias,
                ContentGroupName);
            elementType.AddPropertyType(
                new PropertyType(shortStringHelper, textarea, "intro") { Name = "Intro", SortOrder = 1 },
                ContentGroupAlias,
                ContentGroupName);

            await CreateAsync(elementType);

            return elementType;
        }

        private async Task<IDataType> EnsureDataTypeAsync(IContentType elementType)
        {
            var existing = await dataTypeService.GetAsync(DataTypeName);

            if (existing != null)
            {
                return existing;
            }

            if (!propertyEditors.TryGet(Constants.PropertyEditors.Aliases.BlockList, out var blockList))
            {
                throw new InvalidOperationException("The Block List property editor is not available.");
            }

            var dataType = new DataType(blockList, configurationEditorJsonSerializer)
            {
                Name = DataTypeName,
                EditorUiAlias = "Umb.PropertyEditorUi.BlockList",
                DatabaseType = ValueStorageType.Ntext,
                ConfigurationData = new Dictionary<string, object>
                {
                    ["blocks"] = new[]
                    {
                        new Dictionary<string, object> { ["contentElementTypeKey"] = elementType.Key },
                    },
                },
            };

            var attempt = await dataTypeService.CreateAsync(dataType, UserKey);

            if (!attempt.Success)
            {
                throw new InvalidOperationException($"Could not create the '{DataTypeName}' data type: {attempt.Status}");
            }

            return attempt.Result;
        }

        private async Task<IContentType> EnsureArticleTypeAsync(IDataType blocksDataType)
        {
            var articleType = contentTypeService.Get(ArticleTypeAlias);

            if (articleType == null)
            {
                var template = await EnsureArticleTemplateAsync();

                articleType = new ContentType(shortStringHelper, Constants.System.Root)
                {
                    Alias = ArticleTypeAlias,
                    Name = "Article",
                    Icon = "icon-article",
                    Variations = ContentVariation.Nothing,
                    AllowedTemplates = [template],
                };
                articleType.SetDefaultTemplate(template);
                articleType.AddPropertyType(NewBlocksProperty(blocksDataType), ContentGroupAlias, ContentGroupName);

                await CreateAsync(articleType);

                return articleType;
            }

            if (!articleType.PropertyTypeExists(BlocksPropertyAlias))
            {
                articleType.AddPropertyType(NewBlocksProperty(blocksDataType), ContentGroupAlias, ContentGroupName);

                var attempt = await contentTypeService.UpdateAsync(articleType, UserKey);

                if (!attempt.Success)
                {
                    throw new InvalidOperationException($"Could not add '{BlocksPropertyAlias}' to '{ArticleTypeAlias}': {attempt.Result}");
                }
            }

            return articleType;
        }

        private PropertyType NewBlocksProperty(IDataType blocksDataType)
            => new(shortStringHelper, blocksDataType, BlocksPropertyAlias) { Name = "Blocks", Variations = ContentVariation.Nothing };

        private async Task<ITemplate> EnsureArticleTemplateAsync()
        {
            var existing = await templateService.GetAsync("Article");

            if (existing != null)
            {
                return existing;
            }

            // Creating a template (re)writes its view file, so hand it the view that ships with the site
            var viewPath = Path.Combine(hostEnvironment.ContentRootPath, "Views", "Article.cshtml");
            var content = System.IO.File.Exists(viewPath) ? await System.IO.File.ReadAllTextAsync(viewPath) : null;

            var attempt = await templateService.CreateAsync("Article", "Article", content, UserKey);

            if (!attempt.Success)
            {
                throw new InvalidOperationException($"Could not create the Article template: {attempt.Status}");
            }

            return attempt.Result;
        }

        private async Task EnsureArticleAllowedUnderStartAsync(IContentType articleType)
        {
            var startType = contentTypeService.Get(StartTypeAlias);
            var allowed = startType?.AllowedContentTypes?.ToList() ?? [];

            if (startType == null || allowed.Any(x => x.Key == articleType.Key))
            {
                return;
            }

            allowed.Add(new ContentTypeSort(articleType.Key, allowed.Count, articleType.Alias));
            startType.AllowedContentTypes = allowed;

            var attempt = await contentTypeService.UpdateAsync(startType, UserKey);

            if (!attempt.Success)
            {
                throw new InvalidOperationException($"Could not allow '{ArticleTypeAlias}' under '{StartTypeAlias}': {attempt.Result}");
            }
        }

        private void EnsureContactPage(IContentType articleType, IContentType elementType)
        {
            var start = contentService.GetRootContent().FirstOrDefault(x => x.ContentType.Alias == StartTypeAlias);

            if (start == null)
            {
                Logger.LogWarning("No start page found, so the {Page} page was not created.", ContactPageName);
                return;
            }

            var contact = contentService
                .GetPagedChildren(start.Id, 0, int.MaxValue, out _, propertyAliases: null, filter: null, ordering: null)
                .FirstOrDefault(x => x.ContentType.Alias == ArticleTypeAlias
                    && string.Equals(x.Name, ContactPageName, StringComparison.OrdinalIgnoreCase));

            var blocks = contact?.GetValue<string>(BlocksPropertyAlias);

            if (contact != null && !string.IsNullOrWhiteSpace(blocks))
            {
                // Either it already has the form or an editor has put other blocks there; leave it alone
                Logger.LogInformation(
                    "The {Page} page already has blocks (contact form block: {HasForm}), so it was left as it is.",
                    ContactPageName,
                    HasBlockOfType(blocks, elementType.Key));
                return;
            }

            contact ??= contentService.Create(ContactPageName, start.Key, articleType.Alias);
            contact.SetValue(BlocksPropertyAlias, CreateBlocksValue(elementType.Key));

            var saved = contentService.Save(contact);

            if (!saved.Success)
            {
                throw new InvalidOperationException($"Could not save the {ContactPageName} page: {saved.Result}");
            }

            var published = contentService.Publish(contact, ["*"]);

            if (!published.Success)
            {
                throw new InvalidOperationException($"Could not publish the {ContactPageName} page: {published.Result}");
            }
        }

        /// <summary>A Block List value with one invariant contact form block, in the format the backoffice stores.</summary>
        private static string CreateBlocksValue(Guid elementTypeKey)
        {
            var contentKey = Guid.NewGuid();

            var value = new
            {
                contentData = new[]
                {
                    new
                    {
                        contentTypeKey = elementTypeKey,
                        key = contentKey,
                        values = new[]
                        {
                            new BlockValue(Constants.PropertyEditors.Aliases.TextBox, "heading", "Contact us"),
                            new BlockValue(Constants.PropertyEditors.Aliases.TextArea, "intro", "Questions about a movie or the site? Send us a message."),
                        },
                    },
                },
                settingsData = Array.Empty<object>(),
                expose = new[] { new { contentKey, culture = (string?)null, segment = (string?)null } },
                layout = new Dictionary<string, object>
                {
                    [Constants.PropertyEditors.Aliases.BlockList] = new[] { new { contentKey, settingsKey = (Guid?)null } },
                },
            };

            return JsonSerializer.Serialize(value, JsonOptions);
        }

        private static bool HasBlockOfType(string blocksValue, Guid elementTypeKey)
        {
            try
            {
                using var json = JsonDocument.Parse(blocksValue);

                return json.RootElement.TryGetProperty("contentData", out var contentData)
                    && contentData.EnumerateArray().Any(block =>
                        block.TryGetProperty("contentTypeKey", out var key) && key.GetGuid() == elementTypeKey);
            }
            catch (Exception ex) when (ex is JsonException or InvalidOperationException or FormatException)
            {
                return false;
            }
        }

        private async Task<IDataType> GetDataTypeAsync(Guid key)
            => await dataTypeService.GetAsync(key)
                ?? throw new InvalidOperationException($"The data type {key} does not exist.");

        private async Task CreateAsync(IContentType contentType)
        {
            var attempt = await contentTypeService.CreateAsync(contentType, UserKey);

            if (!attempt.Success)
            {
                throw new InvalidOperationException($"Could not create the '{contentType.Alias}' content type: {attempt.Result}");
            }
        }

        private sealed record BlockValue(string EditorAlias, string Alias, string Value)
        {
            public string? Culture => null;

            public string? Segment => null;
        }
    }
}
