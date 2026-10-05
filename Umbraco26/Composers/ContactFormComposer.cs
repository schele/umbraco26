using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenIddict.Validation.AspNetCore;
using Umbraco.Cms.Api.Common.OpenApi;
using Umbraco.Cms.Api.Management.OpenApi;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;
using Umbraco26.Business.Data;
using Umbraco26.Business.Notifications;
using Umbraco26.Business.Security;
using Umbraco26.Business.Services;
using Umbraco26.Business.Services.Interfaces;

namespace Umbraco26.Composers
{
    /// <summary>The contact form block: its storage, form security and the backoffice API behind the dashboard.</summary>
    public class ContactFormComposer : IComposer
    {
        /// <summary>The backoffice API's name, route prefix and OpenAPI document.</summary>
        public const string ApiName = "contact-submissions";

        public void Compose(IUmbracoBuilder builder)
        {
            // Submissions live in the Umbraco database, so share its connection and transaction
            builder.Services.AddUmbracoDbContext<ContactFormDbContext>(
                (serviceProvider, options, _, _) => options.UseUmbracoDatabaseProvider(serviceProvider),
                shareUmbracoConnection: true);

            builder.Services.TryAddSingleton(TimeProvider.System);
            builder.Services.AddSingleton<IContactFormTokenService, ContactFormTokenService>();
            builder.Services.AddSingleton<ContactSubmissionEmailProtector>();
            builder.Services.AddScoped<IContactSubmissionService, ContactSubmissionService>();

            builder.AddNotificationAsyncHandler<UmbracoApplicationStartedNotification, ContactFormMigrationHandler>();

            builder.Services.AddAuthorization(options => options.AddPolicy(ContactSubmissionsAccessRequirement.PolicyName, policy =>
            {
                policy.AuthenticationSchemes.Add(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
                policy.Requirements.Add(new ContactSubmissionsAccessRequirement());
            }));
            // Singleton like Umbraco's own handlers, which singletons consume through IAuthorizationService
            builder.Services.AddSingleton<IAuthorizationHandler, ContactSubmissionsAccessHandler>();

            // Served at /umbraco/openapi/contact-submissions.json, for generating a typed client if needed
            builder.AddBackOfficeOpenApiDocument(
                ApiName,
                document => document
                    .WithTitle("Contact submissions API")
                    .WithBackOfficeAuthentication()
                    .WithJsonOptions(Umbraco.Cms.Core.Constants.JsonOptionsNames.BackOffice));
        }
    }
}
