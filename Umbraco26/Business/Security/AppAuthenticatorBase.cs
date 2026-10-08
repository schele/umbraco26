using System.Runtime.Serialization;
using Google.Authenticator;
using Umbraco.Cms.Core.Security;

namespace Umbraco26.Business.Security
{
    /// <summary>What's needed to add the account to an authenticator app: a QR code and the secret it holds.</summary>
    [DataContract]
    public class TwoFactorAuthInfo : ISetupTwoFactorModel
    {
        [DataMember(Name = "qrCodeSetupImageUrl")]
        public string? QrCodeSetupImageUrl { get; set; }

        [DataMember(Name = "secret")]
        public string? Secret { get; set; }
    }

    /// <summary>
    /// Two-factor login with an authenticator app, such as Google or Microsoft Authenticator. Backoffice users and
    /// members each get their own provider, as Umbraco lists every provider to both; they differ only in whose name
    /// the app lists the account under.
    /// </summary>
    public abstract class AppAuthenticatorBase : ITwoFactorProvider
    {
        /// <summary>What the authenticator app lists the account under, next to the username.</summary>
        private const string Issuer = "Umbraco26";

        // The library accepts codes from up to five minutes either side by default. Allow only the neighbouring
        // 30-second codes, for clock drift and the time it takes to type one in.
        private static readonly TimeSpan ClockDriftTolerance = TimeSpan.FromSeconds(30);

        public abstract string ProviderName { get; }

        /// <summary>The username of the user or member with this key, or <c>null</c> when there is none.</summary>
        protected abstract Task<string?> GetAccountNameAsync(Guid userOrMemberKey);

        public async Task<ISetupTwoFactorModel> GetSetupDataAsync(Guid userOrMemberKey, string secret)
        {
            var accountName = await GetAccountNameAsync(userOrMemberKey)
                ?? throw new InvalidOperationException($"{ProviderName} found no account with the key {userOrMemberKey}.");

            // The QR code is drawn here as a data URL, so the secret isn't sent to a QR code service
            SetupCode setupInfo = new TwoFactorAuthenticator().GenerateSetupCode(Issuer, accountName, secret, secretIsBase32: false);

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
