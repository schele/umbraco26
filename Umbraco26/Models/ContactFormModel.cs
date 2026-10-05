using System.ComponentModel.DataAnnotations;

namespace Umbraco26.Models
{
    /// <summary>What the contact form block posts to <c>ContactFormSurfaceController</c>.</summary>
    public class ContactFormModel
    {
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
    }
}
