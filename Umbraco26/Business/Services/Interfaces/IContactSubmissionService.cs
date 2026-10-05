using Umbraco.Cms.Core.Models;
using Umbraco26.Models;

namespace Umbraco26.Business.Services.Interfaces
{
    public interface IContactSubmissionService
    {
        /// <summary>Stores a submission; the email is encrypted before it is saved.</summary>
        Task<ContactSubmission> AddAsync(string name, string email, string comment, Guid pageKey, string culture);

        /// <summary>One page of submissions, newest first. Emails are returned still encrypted.</summary>
        Task<PagedModel<ContactSubmission>> GetPageAsync(int skip, int take);
    }
}
