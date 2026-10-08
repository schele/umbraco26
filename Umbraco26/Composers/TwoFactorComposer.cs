using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Security;
using Umbraco.Extensions;
using Umbraco26.Business.Security;

namespace Umbraco26.Composers
{
    /// <summary>
    /// Two-factor login with an authenticator app: for backoffice users, who turn it on from their user menu, and for
    /// members, who turn it on from the My account page.
    /// </summary>
    public class TwoFactorComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            new BackOfficeIdentityBuilder(builder.Services)
                .AddTwoFactorProvider<UmbracoUserAppAuthenticator>(UmbracoUserAppAuthenticator.Name);

            new MemberIdentityBuilder(builder.Services)
                .AddTwoFactorProvider<UmbracoMemberAppAuthenticator>(UmbracoMemberAppAuthenticator.Name);
        }
    }
}
