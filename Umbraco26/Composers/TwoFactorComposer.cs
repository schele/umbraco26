using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Security;
using Umbraco.Extensions;
using Umbraco26.Business.Security;

namespace Umbraco26.Composers
{
    /// <summary>Two-factor login for backoffice users, which each user turns on from their user menu.</summary>
    public class TwoFactorComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            new BackOfficeIdentityBuilder(builder.Services)
                .AddTwoFactorProvider<UmbracoUserAppAuthenticator>(UmbracoUserAppAuthenticator.Name);
        }
    }
}
