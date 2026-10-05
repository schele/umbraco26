namespace Umbraco26.Models
{
    /// <summary>
    /// The outcome of a contact form post, passed back to the page as <c>?contact={status}#contact-form</c>
    /// (Post/Redirect/Get) so the block can show the matching message.
    /// </summary>
    public static class ContactFormStatus
    {
        public const string QueryKey = "contact";
        public const string Anchor = "contact-form";

        public const string Sent = "sent";
        public const string Invalid = "invalid";
        public const string Expired = "expired";
    }
}
