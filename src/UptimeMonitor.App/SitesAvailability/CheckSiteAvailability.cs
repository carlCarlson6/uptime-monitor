using MassTransit;
using Microsoft.Azure.Cosmos;

namespace UptimeMonitor.App.SitesAvailability;

public record CheckSiteAvailability(Guid SiteId, Uri SiteUri);

public class CheckSiteAvailabilityConsumer(Database db) : IConsumer<CheckSiteAvailability>
{
    private readonly HttpClient _http = new();
    private readonly Container _sitesContainer = db.GetContainer("sites");
    
    public async Task Consume(ConsumeContext<CheckSiteAvailability> context)
    {
        var (siteId, siteUri) = context.Message;
        var siteIsUp = await ConnectToSite(siteUri);
        await _sitesContainer.CreateItemAsync(
            new SiteStatus(Guid.NewGuid(), siteIsUp, DateTimeOffset.UtcNow), 
            new PartitionKey(siteId.ToString()));
    }

    private async Task<bool> ConnectToSite(Uri uri)
    {
        var response = await _http.GetAsync(uri);
        return response.IsSuccessStatusCode;
    }
}