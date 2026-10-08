using Umbraco.Cms.Core.Services;

namespace Umbraco26.Business.Security
{
    /// <summary>
    /// Two-factor login with an authenticator app for members, turned on from the My account page. It has no
    /// backoffice manifest, so the backoffice doesn't offer it to users.
    /// </summary>
    public class UmbracoMemberAppAuthenticator(IMemberService memberService) : AppAuthenticatorBase
    {
        /// <summary>Saved with each member who turns it on, so it can't be renamed afterwards.</summary>
        public const string Name = "UmbracoMemberAppAuthenticator";

        public override string ProviderName => Name;

        protected override Task<string?> GetAccountNameAsync(Guid userOrMemberKey)
            => Task.FromResult(memberService.GetById(userOrMemberKey)?.Username);
    }
}
