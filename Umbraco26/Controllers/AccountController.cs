using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco26.Business.Security;
using Umbraco26.Models.ViewModels;

namespace Umbraco26.Controllers
{
    /// <summary>
    /// The member's My account page, where they turn two-factor login on and off. The forms post to Umbraco's own
    /// two-factor and logout controllers.
    /// </summary>
    public class AccountController(
        PageControllerDependencies dependencies,
        IMemberManager memberManager,
        IMemberTwoFactorLoginService twoFactorLoginService,
        UmbracoMemberAppAuthenticator authenticator)
        : BasePageController<Account, AccountPageViewModel>(dependencies)
    {
        /// <summary>The page is only shown through <see cref="AccountAsync"/>, which checks for a member first.</summary>
        public override IActionResult Index() => NotFound();

        protected override AccountPageViewModel Build(Account page) => new() { Content = page };

        /// <summary>
        /// Matches the Account template, so Umbraco routes the page here instead of to <c>Index</c>, and lets the member
        /// lookups run asynchronously. Visitors who aren't logged in are sent to the login page, which brings them back.
        /// </summary>
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)] // The QR code holds the member's secret
        public async Task<IActionResult> AccountAsync()
        {
            if (CurrentPage is not Account page)
            {
                return NotFound();
            }

            var member = await memberManager.GetCurrentMemberAsync();

            if (member == null)
            {
                var loginUrl = page.AncestorOrSelf<Start>()?.FirstChild<Login>()?.Url();

                return loginUrl == null ? NotFound() : Redirect(QueryHelpers.AddQueryString(loginUrl, "returnUrl", page.Url()));
            }

            var providers = await twoFactorLoginService.GetProviderNamesAsync(member.Key);
            var enabled = providers.Success
                && providers.Result.Any(x => x.ProviderName == UmbracoMemberAppAuthenticator.Name && x.IsEnabledOnUser);

            return PageResult(page, new AccountPageViewModel
            {
                Content = page,
                MemberName = member.Name ?? member.UserName ?? string.Empty,
                TwoFactorEnabled = enabled,
                Setup = enabled ? null : await GetSetupAsync(member.Key),
            });
        }

        private async Task<TwoFactorAuthInfo?> GetSetupAsync(Guid memberKey)
        {
            // After a wrong code Umbraco shows the page again for the same post; keep the QR code the member has scanned
            if (Request.HasFormContentType && Request.Form["secret"].ToString() is { Length: > 0 } secret)
            {
                return await authenticator.GetSetupDataAsync(memberKey, secret) as TwoFactorAuthInfo;
            }

            var attempt = await twoFactorLoginService.GetSetupInfoAsync(memberKey, UmbracoMemberAppAuthenticator.Name);

            return attempt.Success ? attempt.Result as TwoFactorAuthInfo : null;
        }
    }
}
