using System.Globalization;
using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;
using Umbraco26.Business.Services.Interfaces;

namespace Umbraco26.Business.Services
{
    /// <summary>
    /// Protects <c>{pageKey}|{issuedUtcTicks}</c> with a time-limited Data Protection protector. The protector
    /// rejects tokens that were changed or have outlived <see cref="Lifetime"/>.
    /// </summary>
    public class ContactFormTokenService(IDataProtectionProvider dataProtectionProvider, TimeProvider timeProvider)
        : IContactFormTokenService
    {
        public const string Purpose = "Umbraco26.ContactForm.Token";

        public static readonly TimeSpan Lifetime = TimeSpan.FromHours(24);

        /// <summary>People need longer than this to fill in a name, an email and a comment.</summary>
        public static readonly TimeSpan MinimumFillTime = TimeSpan.FromSeconds(3);

        private readonly ITimeLimitedDataProtector _protector = dataProtectionProvider
            .CreateProtector(Purpose)
            .ToTimeLimitedDataProtector();

        public string CreateToken(Guid pageKey)
        {
            var issuedUtcTicks = timeProvider.GetUtcNow().UtcTicks;

            return _protector.Protect(
                string.Create(CultureInfo.InvariantCulture, $"{pageKey}|{issuedUtcTicks}"),
                Lifetime);
        }

        public ContactFormTokenStatus Validate(string? token, Guid pageKey)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return ContactFormTokenStatus.Invalid;
            }

            string payload;

            try
            {
                payload = _protector.Unprotect(token);
            }
            catch (CryptographicException)
            {
                return ContactFormTokenStatus.Invalid;
            }

            var parts = payload.Split('|');

            if (parts.Length != 2
                || !Guid.TryParse(parts[0], out var tokenPageKey)
                || !long.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var issuedUtcTicks))
            {
                return ContactFormTokenStatus.Invalid;
            }

            if (tokenPageKey != pageKey)
            {
                return ContactFormTokenStatus.WrongPage;
            }

            var age = timeProvider.GetUtcNow().UtcTicks - issuedUtcTicks;

            return age < MinimumFillTime.Ticks ? ContactFormTokenStatus.TooFast : ContactFormTokenStatus.Valid;
        }
    }
}
