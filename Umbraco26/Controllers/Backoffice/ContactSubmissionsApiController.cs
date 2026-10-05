using System.Security.Cryptography;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Common.ViewModels.Pagination;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Routing;
using Umbraco26.Business.Security;
using Umbraco26.Business.Services;
using Umbraco26.Business.Services.Interfaces;
using Umbraco26.Composers;
using Umbraco26.Models;
using Umbraco26.Models.Backoffice;

namespace Umbraco26.Controllers.Backoffice
{
    /// <summary>
    /// Lists contact form submissions for the "Contact submissions" dashboard, newest first. Served at
    /// <c>/umbraco/contact-submissions/api/v1/submissions</c> and only to backoffice users who may see them.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [BackOfficeRoute($"{ContactFormComposer.ApiName}/api/v{{version:apiVersion}}")]
    [Authorize(Policy = ContactSubmissionsAccessRequirement.PolicyName)]
    [MapToApi(ContactFormComposer.ApiName)]
    [ApiExplorerSettings(GroupName = "Contact submissions")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)] // Personal data: keep it out of every cache
    public class ContactSubmissionsApiController(
        IContactSubmissionService submissionService,
        ContactSubmissionEmailProtector emailProtector,
        IEntityService entityService,
        ILogger<ContactSubmissionsApiController> logger)
        : ControllerBase
    {
        public const int MaxTake = 100;

        [HttpGet("submissions")]
        [ProducesResponseType<PagedViewModel<ContactSubmissionResponseModel>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSubmissions(int skip = 0, int take = 20)
        {
            skip = Math.Max(skip, 0);
            take = Math.Clamp(take, 1, MaxTake);

            var page = await submissionService.GetPageAsync(skip, take);
            var submissions = page.Items.ToList();
            var pageNames = GetPageNames(submissions.Select(x => x.PageKey));

            return Ok(new PagedViewModel<ContactSubmissionResponseModel>
            {
                Total = page.Total,
                Items = submissions.Select(submission => new ContactSubmissionResponseModel
                {
                    Id = submission.Id,
                    CreatedUtc = submission.CreatedUtc,
                    Name = submission.Name,
                    Email = Decrypt(submission),
                    Comment = submission.Comment,
                    PageKey = submission.PageKey,
                    PageName = pageNames.GetValueOrDefault(submission.PageKey),
                    Culture = submission.Culture,
                }).ToList(),
            });
        }

        private Dictionary<Guid, string?> GetPageNames(IEnumerable<Guid> pageKeys)
        {
            var keys = pageKeys.Distinct().ToArray();

            return keys.Length == 0
                ? []
                : entityService.GetAll(UmbracoObjectTypes.Document, keys).ToDictionary(x => x.Key, x => x.Name);
        }

        private string? Decrypt(ContactSubmission submission)
        {
            try
            {
                return emailProtector.Unprotect(submission.EmailProtected);
            }
            catch (CryptographicException ex)
            {
                logger.LogWarning(ex, "Could not decrypt the email of contact submission {Id}.", submission.Id);
                return null;
            }
        }
    }
}
