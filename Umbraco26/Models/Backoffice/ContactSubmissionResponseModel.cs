namespace Umbraco26.Models.Backoffice
{
    /// <summary>A contact form submission as the backoffice dashboard shows it, with the email decrypted.</summary>
    public class ContactSubmissionResponseModel
    {
        public int Id { get; init; }

        public DateTime CreatedUtc { get; init; }

        public string Name { get; init; } = string.Empty;

        /// <summary>Null if the address can't be decrypted, e.g. when the Data Protection keys were lost.</summary>
        public string? Email { get; init; }

        public string Comment { get; init; } = string.Empty;

        public Guid PageKey { get; init; }

        /// <summary>Null if the page has been deleted.</summary>
        public string? PageName { get; init; }

        public string Culture { get; init; } = string.Empty;
    }
}
