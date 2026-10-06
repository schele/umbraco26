using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace Umbraco26.Models
{
    /// <summary>What the contact form block posts to <c>ContactFormSurfaceController</c>.</summary>
    public class ContactFormModel
    {
        /// <summary>Characters that would let an address add headers or parameters to a <c>mailto:</c> link.</summary>
        private static readonly char[] MailtoUnsafeCharacters = ['?', '&', '%', '/', ':', '='];

        [Required]
        [StringLength(ContactSubmission.NameMaxLength)]
        public string? Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(ContactSubmission.EmailMaxLength)]
        public string? Email { get; set; }

        [Required]
        [StringLength(ContactSubmission.CommentMaxLength)]
        public string? Comment { get; set; }

        /// <summary>Honeypot: hidden from people, so anything in it was filled in by a bot.</summary>
        public string? Website { get; set; }

        /// <summary>Signed page key and render time, see <c>IContactFormTokenService</c>.</summary>
        public string? FormToken { get; set; }

        /// <summary>The reCAPTCHA v3 token the page fetches as the form is sent, see <c>IReCaptchaService</c>.</summary>
        public string? ReCaptchaToken { get; set; }

        /// <summary>
        /// Stricter than <see cref="EmailAddressAttribute"/>: a plain address (no display name or other extras)
        /// that is safe to put in a <c>mailto:</c> link, so <c>boss%40firm.se?cc=someone@example.com</c> is refused.
        /// </summary>
        public static bool IsPlainEmailAddress(string? email)
            => !string.IsNullOrEmpty(email)
                && MailAddress.TryCreate(email, out var address)
                && address.Address == email
                && email.IndexOfAny(MailtoUnsafeCharacters) < 0;
    }
}
