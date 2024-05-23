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