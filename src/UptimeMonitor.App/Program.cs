using UptimeMonitor.App.Infrastructure.CosmosDb;
using UptimeMonitor.App.Infrastructure.MassTransit;
using UptimeMonitor.App.Infrastructure.Quartz;
using UptimeMonitor.App.Presentation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddQuartzAppJobs()
    .AddMassTransitMessaging()
    .AddCosmosDb()
    .AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();