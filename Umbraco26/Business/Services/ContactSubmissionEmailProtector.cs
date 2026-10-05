using Microsoft.AspNetCore.DataProtection;

namespace Umbraco26.Business.Services
{
    /// <summary>
    /// Encrypts contact form email addresses at rest. Only the backoffice API decrypts them.
    /// The Data Protection key ring must be kept (and shared between servers) for stored addresses to stay readable.
    /// </summary>
    public class ContactSubmissionEmailProtector(IDataProtectionProvider dataProtectionProvider)
    {
        public const string Purpose = "Umbraco26.ContactSubmission.Email";

        private readonly IDataProtector _protector = dataProtectionProvider.CreateProtector(Purpose);

        public string Protect(string email) => _protector.Protect(email);

        public string Unprotect(string protectedEmail) => _protector.Unprotect(protectedEmail);
    }
}
