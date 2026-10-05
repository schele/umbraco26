
using Microsoft.AspNetCore.DataProtection;
using Umbraco26.Business.Services;
using Umbraco26.Business.Services.Interfaces;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddComposers()
    .Build();

builder.Services.AddScoped<ISitemapService, SitemapService>();
builder.Services.AddScoped<IOmdbService, OmdbService>();
builder.Services.AddScoped<IMoviesJob, MoviesJob>();
builder.Services.AddScoped<IFindService, FindService>();

// The Data Protection key ring encrypts contact form emails at rest and protects the backoffice login,
// anti-forgery and TempData cookies. Keep the folder, and share it between servers: losing it, or changing the
// application name, makes stored emails unreadable and signs everyone out. It is git-ignored. On Windows the
// keys are also encrypted for this machine, so a copied folder can't decrypt emails or forge logins elsewhere.
IDataProtectionBuilder dataProtection = builder.Services.AddDataProtection()
    .SetApplicationName("Umbraco26")
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "umbraco", "Data", "DataProtection-Keys")));

if (OperatingSystem.IsWindows())
{
    dataProtection.ProtectKeysWithDpapi(protectToLocalMachine: true);
}

builder.Services.AddServerSideBlazor();

WebApplication app = builder.Build();

app.MapBlazorHub();

await app.BootUmbracoAsync();

app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

await app.RunAsync();
