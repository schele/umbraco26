namespace Umbraco26.Business.Services.Interfaces
{
    public enum ContactFormTokenStatus
    {
        Valid,

        /// <summary>Tampered with, malformed or older than the token lifetime.</summary>
        Invalid,

        /// <summary>Issued for another page than the one it was posted to.</summary>
        WrongPage,

        /// <summary>Posted sooner after the form was rendered than a person can fill it in.</summary>
        TooFast,
    }

    /// <summary>
    /// Signs the contact form with the page it belongs to and the time it was rendered, so a post can be
    /// checked for tampering, expiry and bot-like speed without any server-side state.
    /// </summary>
    public interface IContactFormTokenService
    {
        string CreateToken(Guid pageKey);

        ContactFormTokenStatus Validate(string? token, Guid pageKey);
    }
}
