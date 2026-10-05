using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.Routing;
using Umbraco.Cms.Web.Website.Controllers;
using Umbraco26.Business.Services.Interfaces;
using Umbraco26.Models;

namespace Umbraco26.Controllers
{
    /// <summary>
    /// Receives the contact form block's classic POST and redirects back to the page with
    /// <c>?contact=sent|invalid|expired#contact-form</c> (Post/Redirect/Get).
    /// </summary>
    public class ContactFormSurfaceController(
        IUmbracoContextAccessor umbracoContextAccessor,
        IUmbracoDatabaseFactory databaseFactory,
        ServiceContext services,
        AppCaches appCaches,
        IProfilingLogger profilingLogger,
        IPublishedUrlProvider publishedUrlProvider,
        IContactFormTokenService tokenService,
        IContactSubmissionService submissionService,
        ILogger<ContactFormSurfaceController> logger)
        : SurfaceController(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(ContactFormModel model)
        {
            if (CurrentPage is not IPublishedContent page)
            {
                return NotFound();
            }

            // Bots get the same answer as people, so they can't tell they were caught
            if (!string.IsNullOrEmpty(model.Website))
            {
                logger.LogInformation("Contact form on page {PageKey} had the honeypot filled in; nothing was stored.", page.Key);
                return RedirectToPage(page, ContactFormStatus.Sent);
            }

            switch (tokenService.Validate(model.FormToken, page.Key))
            {
                case ContactFormTokenStatus.Invalid:
                case ContactFormTokenStatus.WrongPage:
                    return RedirectToPage(page, ContactFormStatus.Expired);

                case ContactFormTokenStatus.TooFast:
                    logger.LogInformation("Contact form on page {PageKey} was sent too soon after it was shown; nothing was stored.", page.Key);
                    return RedirectToPage(page, ContactFormStatus.Sent);
            }

            if (!ModelState.IsValid)
            {
                return RedirectToPage(page, ContactFormStatus.Invalid);
            }

            await submissionService.AddAsync(model.Name!, model.Email!, model.Comment!, page.Key, GetCulture());

            return RedirectToPage(page, ContactFormStatus.Sent);
        }

        /// <summary>
        /// Redirects to the page's own URL, worked out on the server. Nothing from the request decides
        /// where the visitor ends up, so the form can't be used as an open redirect.
        /// </summary>
        private IActionResult RedirectToPage(IPublishedContent page, string status)
        {
            var url = page.Url(PublishedUrlProvider, GetCulture(), UrlMode.Relative);

            if (!Url.IsLocalUrl(url))
            {
                url = "/";
            }

            return LocalRedirect($"{url}?{ContactFormStatus.QueryKey}={status}#{ContactFormStatus.Anchor}");
        }

        private string GetCulture()
            => HttpContext.Features.Get<UmbracoRouteValues>()?.PublishedRequest.Culture ?? CultureInfo.CurrentCulture.Name;
    }
}
