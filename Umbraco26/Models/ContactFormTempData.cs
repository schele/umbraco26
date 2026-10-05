using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Umbraco26.Models
{
    /// <summary>What a visitor typed, kept across the redirect so the form can be filled in again.</summary>
    public record ContactFormInput(string? Name, string? Email, string? Comment);

    /// <summary>
    /// Keeps the contact form's input in TempData when a post is sent back as invalid or expired. The default
    /// TempData provider stores it in a data-protected cookie, so the values are capped at the form's limits.
    /// </summary>
    public static class ContactFormTempData
    {
        private const string NameKey = "ContactForm.Name";
        private const string EmailKey = "ContactForm.Email";
        private const string CommentKey = "ContactForm.Comment";

        public static void Keep(ITempDataDictionary tempData, string? name, string? email, string? comment)
        {
            tempData[NameKey] = Limit(name, ContactSubmission.NameMaxLength);
            tempData[EmailKey] = Limit(email, ContactSubmission.EmailMaxLength);
            tempData[CommentKey] = Limit(comment?.ReplaceLineEndings("\n"), ContactSubmission.CommentMaxLength);
        }

        /// <summary>The kept input, or null if there is none. Reading it removes it, so it is shown once.</summary>
        public static ContactFormInput? Take(ITempDataDictionary tempData)
        {
            if (!tempData.ContainsKey(NameKey) && !tempData.ContainsKey(EmailKey) && !tempData.ContainsKey(CommentKey))
            {
                return null;
            }

            return new ContactFormInput(
                tempData[NameKey] as string,
                tempData[EmailKey] as string,
                tempData[CommentKey] as string);
        }

        private static string? Limit(string? value, int maxLength)
            => value is { Length: > 0 } && value.Length > maxLength ? value[..maxLength] : value;
    }
}
