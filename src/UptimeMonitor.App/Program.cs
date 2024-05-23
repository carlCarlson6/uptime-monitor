using UptimeMonitor.App.Infrastructure.CosmosDb;
using UptimeMonitor.App.Infrastructure.MassTransit;
using UptimeMonitor.App.Infrastructure.Quartz;
using UptimeMonitor.App.Presentation;
using UptimeMonitor.App.UptimePanel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddQuartzAppJobs()
    .AddMassTransitMessaging()
    .AddCosmosDb()
    .AddReadAllSitesDefinitions()
    .AddReadAllSites()
    .AddStoreNewSite()
    .AddRazorComponents()
    .AddInteractiveServerComponents();

var webApp = builder.Build();

// Configure the HTTP request pipeline.
if (!webApp.Environment.IsDevelopment())
{
    webApp.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    webApp.UseHsts();
}

webApp
    .UseHttpsRedirection()
    .UseStaticFiles()
    .UseAntiforgery();

webApp
    .MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

webApp.Run();