using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Umbraco26.Business.Migrations
{
    /// <summary>
    /// Sets up the member pages: the <c>login</c> and <c>account</c> document types with their templates, and a
    /// published Log in page and My account page under the start page, named in Swedish and English.
    /// Every step checks what is already there, so nothing editors created or changed is overwritten.
    /// </summary>
    /// <remarks>
    /// Runs unscoped for the same reasons as <see cref="AddContactFormBlock"/>: publishing needs notifications, and they
    /// make ModelsBuilder regenerate models. Neither type has the <c>base</c> composition, which keeps the pages out of
    /// the sitemap; there is nothing for search engines on them. Members are created by editors in the backoffice.
    /// </remarks>
    public class AddMemberPages(
        IMigrationContext context,
        ICoreScopeProvider scopeProvider,
        IContentTypeService contentTypeService,
        ITemplateService templateService,
        IContentService contentService,
        ILanguageService languageService,
        IShortStringHelper shortStringHelper,
        IHostEnvironment hostEnvironment)
        : UnscopedAsyncMigrationBase(context)
    {
        public const string LoginTypeAlias = "login";
        public const string AccountTypeAlias = "account";

        private const string StartTypeAlias = "start";

        private static readonly Guid UserKey = Constants.Security.SuperUserKey;

        private static readonly PageSpec LoginPage = new(LoginTypeAlias, "Login", "Login", "icon-lock", "Logga in", "Log in");
        private static readonly PageSpec AccountPage = new(AccountTypeAlias, "Account", "Account", "icon-user", "Mitt konto", "My account");

        protected override async Task MigrateAsync()
        {
            using (var scope = scopeProvider.CreateCoreScope())
            {
                foreach (var page in new[] { LoginPage, AccountPage })
                {
                    var template = await EnsureTemplateAsync(page.Template);
                    var contentType = await EnsureContentTypeAsync(page, template);
                    await EnsureAllowedUnderStartAsync(contentType);
                    await EnsurePageAsync(page, contentType);
                }

                scope.Complete();
            }

            // Unscoped migrations have to say they are done, which records the plan's new state
            Context.Complete();
        }

        private async Task<ITemplate> EnsureTemplateAsync(string alias)
        {
            var existing = await templateService.GetAsync(alias);

            if (existing != null)
            {
                return existing;
            }

            // Creating a template (re)writes its view file, so hand it the view that ships with the site
            var viewPath = Path.Combine(hostEnvironment.ContentRootPath, "Views", $"{alias}.cshtml");
            var content = System.IO.File.Exists(viewPath) ? await System.IO.File.ReadAllTextAsync(viewPath) : null;

            var attempt = await templateService.CreateAsync(alias, alias, content, UserKey);

            if (!attempt.Success)
            {
                throw new InvalidOperationException($"Could not create the {alias} template: {attempt.Status}");
            }

            return attempt.Result;
        }

        private async Task<IContentType> EnsureContentTypeAsync(PageSpec page, ITemplate template)
        {
            var existing = contentTypeService.Get(page.Alias);

            if (existing != null)
            {
                return existing;
            }

            // Varies by culture like the other pages, so each language has its own name and URL
            var contentType = new ContentType(shortStringHelper, Constants.System.Root)
            {
                Alias = page.Alias,
                Name = page.TypeName,
                Icon = page.Icon,
                Variations = ContentVariation.Culture,
                AllowedTemplates = [template],
            };
            contentType.SetDefaultTemplate(template);

            var created = await contentTypeService.CreateAsync(contentType, UserKey);

            if (!created.Success)
            {
                throw new InvalidOperationException($"Could not create the '{page.Alias}' content type: {created.Result}");
            }

            return contentType;
        }

        private async Task EnsureAllowedUnderStartAsync(IContentType contentType)
        {
            var startType = contentTypeService.Get(StartTypeAlias);
            var allowed = startType?.AllowedContentTypes?.ToList() ?? [];

            if (startType == null || allowed.Any(x => x.Key == contentType.Key))
            {
                return;
            }

            allowed.Add(new ContentTypeSort(contentType.Key, allowed.Count, contentType.Alias));
            startType.AllowedContentTypes = allowed;

            var attempt = await contentTypeService.UpdateAsync(startType, UserKey);

            if (!attempt.Success)
            {
                throw new InvalidOperationException($"Could not allow '{contentType.Alias}' under '{StartTypeAlias}': {attempt.Result}");
            }
        }

        private async Task EnsurePageAsync(PageSpec page, IContentType contentType)
        {
            var start = contentService.GetRootContent().FirstOrDefault(x => x.ContentType.Alias == StartTypeAlias);

            if (start == null)
            {
                Logger.LogWarning("No start page found, so the {Page} page was not created.", page.English);
                return;
            }

            var exists = contentService
                .GetPagedChildren(start.Id, 0, int.MaxValue, out _, propertyAliases: null, filter: null, ordering: null)
                .Any(x => x.ContentType.Alias == page.Alias);

            if (exists)
            {
                return;
            }

            var content = contentService.Create(page.English, start.Key, page.Alias);

            // A culture-variant page can't be saved without a name in each language
            foreach (var language in await languageService.GetAllAsync())
            {
                var isSwedish = language.IsoCode.StartsWith("sv", StringComparison.OrdinalIgnoreCase);
                content.SetCultureName(isSwedish ? page.Swedish : page.English, language.IsoCode);
            }

            var saved = contentService.Save(content);

            if (!saved.Success)
            {
                throw new InvalidOperationException($"Could not save the {page.English} page: {saved.Result}");
            }

            var published = contentService.Publish(content, ["*"]);

            if (!published.Success)
            {
                throw new InvalidOperationException($"Could not publish the {page.English} page: {published.Result}");
            }
        }

        /// <summary>A member page: its document type, template and the page's name in Swedish and English.</summary>
        private sealed record PageSpec(string Alias, string Template, string TypeName, string Icon, string Swedish, string English);
    }
}
