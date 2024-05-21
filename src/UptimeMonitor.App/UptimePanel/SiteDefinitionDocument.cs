using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace UptimeMonitor.App.UptimePanel;

public record SiteDefinitionDocument(Guid Id, Uri Uri, string Slug);

public delegate IAsyncEnumerable<SiteDefinitionDocument> ReadAllSitesDefinitions();

public static class ReadAllSitesDefinitionsExtensions
{
    private static async IAsyncEnumerable<SiteDefinitionDocument> ReadAllSitesDefinitions(Container sitesContainer)
    {
        using var iterator = sitesContainer
            .GetItemLinqQueryable<SiteDefinitionDocument>(requestOptions: new QueryRequestOptions
                { PartitionKey = new PartitionKey(nameof(SiteDefinitionDocument)) })
            .ToFeedIterator();
        
        var response = await iterator.ReadNextAsync();
        while (iterator.HasMoreResults)
        {
            foreach (var item in response)
            {
                yield return item;
            }
        }
    }

    public static IServiceCollection AddReadAllSitesDefinitions(this IServiceCollection services) => services
        .AddSingleton<ReadAllSitesDefinitions>(sp => () => 
            ReadAllSitesDefinitions(sp.GetRequiredService<Database>().GetContainer("sites")));
}

public delegate Task StoreNewSite(Uri uri, string slug);

public static class StoreNewSiteExtensions
{
    private static StoreNewSite StoreNewSite(Container sitesContainer) => (uri, slug) => sitesContainer
        .CreateItemAsync(
            new SiteDefinitionDocument(Guid.NewGuid(), uri, slug), 
            new PartitionKey(nameof(SiteDefinitionDocument)));

    public static IServiceCollection AddStoreNewSite(this IServiceCollection services) => services
        .AddSingleton<StoreNewSite>(sp => StoreNewSite(sp.GetRequiredService<Database>().GetContainer("sites")));
}