using MassTransit;
using Microsoft.Azure.Cosmos;
using Quartz;
using UptimeMonitor.App.UptimePanel;

namespace UptimeMonitor.App.SitesAvailability;

public class SendCheckSitesAvailability(Database db, IPublishEndpoint bus) : IJob
{
    private const string GetAllSiteDefinitionsQuery = "SELECT * FROM docs where docs.partitionKey = @docType";
    private readonly Container _sitesContainer = db.GetContainer("sites");
    
    public async Task Execute(IJobExecutionContext context)
    {
        await foreach (var site in ReadAllSites())
            await bus.Publish(new CheckSiteAvailability(site.Id, site.Uri));
    }

    private async IAsyncEnumerable<SiteDefinitionDocument> ReadAllSites()
    {
        using var feed = _sitesContainer.GetItemQueryIterator<SiteDefinitionDocument>(new QueryDefinition(
                GetAllSiteDefinitionsQuery)
            .WithParameter("@docType", nameof(SiteDefinitionDocument)));
        while (feed.HasMoreResults)
        {
            var response = await feed.ReadNextAsync();
            foreach (var item in response)
            {
                yield return item;
            }
        }
    }
}