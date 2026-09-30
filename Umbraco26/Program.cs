
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
