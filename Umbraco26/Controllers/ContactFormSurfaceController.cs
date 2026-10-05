using System.Globalization;
using Microsoft.AspNetCore.Antiforgery;
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
    /// <c>?contact=sent|invalid|expired#contact-form</c> (Post/Redirect/Get). On invalid and expired the
    /// visitor's input is kept in TempData, so the form comes back filled in.
    /// </summary>
    public class ContactFormSurfaceController(
        IUmbracoContextAccessor umbracoContextAccessor,
        IUmbracoDatabaseFactory databaseFactory,
        ServiceContext services,
        AppCaches appCaches,
        IProfilingLogger profilingLogger,
        IPublishedUrlProvider publishedUrlProvider,
        IAntiforgery antiforgery,
        IContactFormTokenService tokenService,
        IContactSubmissionService submissionService,
        ILogger<ContactFormSurfaceController> logger)
        : SurfaceController(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
    {
        [HttpPost]
        [IgnoreAntiforgeryToken] // Validated below, so a failure can keep the input and say "expired" instead of a bare 400
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

            if (!await antiforgery.IsRequestValidAsync(HttpContext))
            {
                logger.LogInformation("Contact form on page {PageKey} failed anti-forgery validation; nothing was stored.", page.Key);
                return KeepInputAndRedirect(page, ContactFormStatus.Expired);
            }

            switch (tokenService.Validate(model.FormToken, page.Key))
            {
                case ContactFormTokenStatus.Invalid:
                    logger.LogInformation("Contact form on page {PageKey} had a missing, changed or expired form token; nothing was stored.", page.Key);
                    return KeepInputAndRedirect(page, ContactFormStatus.Expired);

                case ContactFormTokenStatus.WrongPage:
                    logger.LogInformation("Contact form on page {PageKey} had a form token issued for another page; nothing was stored.", page.Key);
                    return KeepInputAndRedirect(page, ContactFormStatus.Expired);

                case ContactFormTokenStatus.TooFast:
                    logger.LogInformation("Contact form on page {PageKey} was sent too soon after it was shown; nothing was stored.", page.Key);
                    return RedirectToPage(page, ContactFormStatus.Sent);
            }

            // Browsers send line breaks as CRLF, which would count twice against the comment's length limit
            model.Name = model.Name?.Trim();
            model.Email = model.Email?.Trim();
            model.Comment = model.Comment?.Trim().ReplaceLineEndings("\n");

            ModelState.Clear();

            if (!TryValidateModel(model) || !ContactFormModel.IsPlainEmailAddress(model.Email))
            {
                return KeepInputAndRedirect(page, ContactFormStatus.Invalid);
            }

            await submissionService.AddAsync(model.Name!, model.Email!, model.Comment!, page.Key, GetCulture());

            return RedirectToPage(page, ContactFormStatus.Sent);
        }

        /// <summary>Keeps what the visitor typed, as they typed it, for the form to show again after the redirect.</summary>
        private IActionResult KeepInputAndRedirect(IPublishedContent page, string status)
        {
            string? Field(string name) => Request.HasFormContentType ? Request.Form[name].ToString() : null;

            ContactFormTempData.Keep(TempData, Field("name"), Field("email"), Field("comment"));

            return RedirectToPage(page, status);
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
