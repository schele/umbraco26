using System.Runtime.Serialization;
using Google.Authenticator;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;

namespace Umbraco26.Business.Security
{
    /// <summary>What the backoffice needs to add the account to an authenticator app: a QR code and the secret it holds.</summary>
    [DataContract]
    public class TwoFactorAuthInfo : ISetupTwoFactorModel
    {
        [DataMember(Name = "qrCodeSetupImageUrl")]
        public string? QrCodeSetupImageUrl { get; set; }

        [DataMember(Name = "secret")]
        public string? Secret { get; set; }
    }

    /// <summary>Two-factor login for backoffice users with an authenticator app, such as Google or Microsoft Authenticator.</summary>
    public class UmbracoUserAppAuthenticator(IUserService userService) : ITwoFactorProvider
    {
        /// <summary>Saved with each user who turns it on, so it can't be renamed afterwards.</summary>
        public const string Name = "UmbracoUserAppAuthenticator";

        /// <summary>What the authenticator app lists the account under, next to the username.</summary>
        private const string Issuer = "Umbraco26";

        // The library accepts codes from up to five minutes either side by default. Allow only the neighbouring
        // 30-second codes, for clock drift and the time it takes to type one in.
        private static readonly TimeSpan ClockDriftTolerance = TimeSpan.FromSeconds(30);

        public string ProviderName => Name;

        public async Task<ISetupTwoFactorModel> GetSetupDataAsync(Guid userOrMemberKey, string secret)
        {
            IUser? user = await userService.GetAsync(userOrMemberKey);
            ArgumentNullException.ThrowIfNull(user);

            // The QR code is drawn here as a data URL, so the secret isn't sent to a QR code service
            SetupCode setupInfo = new TwoFactorAuthenticator().GenerateSetupCode(Issuer, user.Username, secret, secretIsBase32: false);

            return new TwoFactorAuthInfo
            {
                QrCodeSetupImageUrl = setupInfo.QrCodeSetupImageUrl,
                Secret = secret
            };
        }

        public bool ValidateTwoFactorPIN(string secret, string code)
            => new TwoFactorAuthenticator().ValidateTwoFactorPIN(secret, code, ClockDriftTolerance, secretIsBase32: false);

        // Turning it on is confirmed like a login, with a code from the newly added account
        public bool ValidateTwoFactorSetup(string secret, string token) => ValidateTwoFactorPIN(secret, token);
    }
}
