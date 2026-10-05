using Microsoft.AspNetCore.Authorization;
using Umbraco.Cms.Api.Management.Security.Authorization;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Security.Authorization;

namespace Umbraco26.Business.Security
{
    /// <summary>Backoffice users with access to the Content section, or administrators.</summary>
    public class ContactSubmissionsAccessRequirement : IAuthorizationRequirement
    {
        public const string PolicyName = "Umbraco26.ContactSubmissionsAccess";
    }

    public class ContactSubmissionsAccessHandler(IAuthorizationHelper authorizationHelper)
        : MustSatisfyRequirementAuthorizationHandler<ContactSubmissionsAccessRequirement>
    {
        protected override Task<bool> IsAuthorized(AuthorizationHandlerContext context, ContactSubmissionsAccessRequirement requirement)
            => Task.FromResult(
                authorizationHelper.TryGetUmbracoUser(context.User, out var user)
                && (user.IsAdmin() || user.AllowedSections.Contains(Constants.Applications.Content)));
    }
}
