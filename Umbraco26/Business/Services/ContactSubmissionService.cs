using Microsoft.EntityFrameworkCore;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Persistence.EFCore.Scoping;
using Umbraco26.Business.Data;
using Umbraco26.Business.Services.Interfaces;
using Umbraco26.Models;

namespace Umbraco26.Business.Services
{
    public class ContactSubmissionService(
        IEFCoreScopeProvider<ContactFormDbContext> scopeProvider,
        ContactSubmissionEmailProtector emailProtector,
        TimeProvider timeProvider)
        : IContactSubmissionService
    {
        public async Task<ContactSubmission> AddAsync(string name, string email, string comment, Guid pageKey, string culture)
        {
            var submission = new ContactSubmission
            {
                Name = Truncate(name, ContactSubmission.NameMaxLength),
                EmailProtected = emailProtector.Protect(email.Trim()),
                Comment = Truncate(comment, ContactSubmission.CommentMaxLength),
                PageKey = pageKey,
                Culture = Truncate(culture, ContactSubmission.CultureMaxLength),
                CreatedUtc = timeProvider.GetUtcNow().UtcDateTime,
            };

            using var scope = scopeProvider.CreateScope();

            await scope.ExecuteWithContextAsync<Task>(async db =>
            {
                db.ContactSubmissions.Add(submission);
                await db.SaveChangesAsync();
            });

            scope.Complete();

            return submission;
        }

        public async Task<PagedModel<ContactSubmission>> GetPageAsync(int skip, int take)
        {
            using var scope = scopeProvider.CreateScope();

            var page = await scope.ExecuteWithContextAsync(async db =>
            {
                var submissions = db.ContactSubmissions.AsNoTracking();

                return new PagedModel<ContactSubmission>
                {
                    Total = await submissions.CountAsync(),
                    Items = await submissions
                        .OrderByDescending(x => x.CreatedUtc)
                        .ThenByDescending(x => x.Id)
                        .Skip(skip)
                        .Take(take)
                        .ToListAsync(),
                };
            });

            scope.Complete();

            return page;
        }

        private static string Truncate(string value, int maxLength)
        {
            value = value.Trim();

            return value.Length > maxLength ? value[..maxLength] : value;
        }
    }
}
