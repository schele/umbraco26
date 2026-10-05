namespace Umbraco26.Models
{
    /// <summary>A message sent with the contact form block, stored in the Umbraco database.</summary>
    public class ContactSubmission
    {
        public const int NameMaxLength = 100;
        public const int EmailMaxLength = 254;
        public const int CommentMaxLength = 2000;
        public const int CultureMaxLength = 16;

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        /// <summary>The sender's email, encrypted with ASP.NET Core Data Protection. Never stored in plain text.</summary>
        public string EmailProtected { get; set; } = string.Empty;

        public string Comment { get; set; } = string.Empty;

        /// <summary>The page the form was sent from.</summary>
        public Guid PageKey { get; set; }

        /// <summary>The culture the page was shown in, e.g. <c>sv</c> or <c>en-US</c>.</summary>
        public string Culture { get; set; } = string.Empty;

        public DateTime CreatedUtc { get; set; }
    }
}
