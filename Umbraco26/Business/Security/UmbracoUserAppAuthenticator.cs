using Umbraco.Cms.Core.Services;

namespace Umbraco26.Business.Security
{
    /// <summary>Two-factor login with an authenticator app for backoffice users, turned on from their user menu.</summary>
    public class UmbracoUserAppAuthenticator(IUserService userService) : AppAuthenticatorBase
    {
        /// <summary>Saved with each user who turns it on, so it can't be renamed afterwards.</summary>
        public const string Name = "UmbracoUserAppAuthenticator";

        public override string ProviderName => Name;

        protected override async Task<string?> GetAccountNameAsync(Guid userOrMemberKey)
            => (await userService.GetAsync(userOrMemberKey))?.Username;
    }
}
