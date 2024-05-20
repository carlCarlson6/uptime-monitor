using MassTransit;

namespace UptimeMonitor.App.SitesAvailability;

public record CheckSiteAvailability(Guid SiteId, Uri SiteUri);

public class CheckSiteAvailabilityConsumer : IConsumer<CheckSiteAvailability>
{
    public async Task Consume(ConsumeContext<CheckSiteAvailability> context)
    {
        var (siteId, siteUri) = context.Message;
        var siteIsUp = await ConnectToSite(siteUri);
        
        // TODO - update site status
        // TODO - update site status history
    }

    private static async Task<bool> ConnectToSite(Uri uri)
    {
        var http = new HttpClient();
        var response = await http.GetAsync(uri);
        return response.IsSuccessStatusCode;
    }
}