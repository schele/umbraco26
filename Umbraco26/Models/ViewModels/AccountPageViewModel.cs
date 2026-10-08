using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco26.Business.Security;

namespace Umbraco26.Models.ViewModels
{
    public class AccountPageViewModel : PageViewModel<Account>
    {
        public string MemberName { get; init; } = string.Empty;

        /// <summary>Whether the member has turned on two-factor login with the authenticator app.</summary>
        public bool TwoFactorEnabled { get; init; }

        /// <summary>The QR code and secret to turn it on with, while it is off.</summary>
        public TwoFactorAuthInfo? Setup { get; init; }
    }
}
