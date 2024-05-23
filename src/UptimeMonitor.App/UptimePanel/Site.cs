using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using UptimeMonitor.App.SitesAvailability;

namespace UptimeMonitor.App.UptimePanel;

public record Site(string Name, Uri Uri, bool IsUp)
{
    public string DisplayShortUri()
    {
        var uriStr = Uri.ToString(); 
        return uriStr.Length <= 40
            ? Uri.ToString()
            : $"{Uri.ToString().Remove(41)}...";
    }
};

public delegate Task<List<Site>> ReadAllSites();

public static class ReadAllSitesExtensions
{
    private static ReadAllSites ReadAllSites(Container sitesContainer, ReadAllSitesDefinitions readAllSitesDefinitions) => async () =>
    {
        var sites = new List<Site>();
        await foreach (var siteDef in readAllSitesDefinitions())
        {
            var status = await GetCurrentSiteStatus(sitesContainer, siteDef.Id);
            sites.Add(new Site(siteDef.Slug, siteDef.Uri, status));
        }
        return sites;
    };

    private static async Task<bool> GetCurrentSiteStatus(Container sitesContainer, Guid siteId)
    {
        var response = await sitesContainer
            .GetItemLinqQueryable<SiteStatus>(requestOptions: new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(siteId.ToString()),
                MaxItemCount = 1
            })
            .OrderByDescending(x => x.CheckedAt)
            .ToFeedIterator()
            .ReadNextAsync();
        return response?.FirstOrDefault()?.IsUp ?? false;
    }

    public static IServiceCollection AddReadAllSites(this IServiceCollection services) => services
        .AddSingleton<ReadAllSites>(sp => ReadAllSites(
            sp.GetRequiredService<Database>().GetContainer("sites"),
            sp.GetRequiredService<ReadAllSitesDefinitions>()));
}

public delegate Task<Site> StoreNewSite(Uri uri, string slug);

public static class StoreNewSiteExtensions
{
    private static StoreNewSite StoreNewSite(Container sitesContainer) => async (uri, slug) =>
    {
        var siteId = Guid.NewGuid(); 
        await sitesContainer.CreateItemAsync(
            new SiteDefinitionDocument(siteId, uri, slug),
            new PartitionKey(nameof(SiteDefinitionDocument)));

        var response = await new HttpClient().GetAsync(uri);
        var isUp = response.IsSuccessStatusCode;
        await sitesContainer.CreateItemAsync(
            new SiteStatus(Guid.NewGuid(), isUp, DateTimeOffset.UtcNow), 
            new PartitionKey(siteId.ToString()));

        return new Site(slug, uri, isUp);
    };

    public static IServiceCollection AddStoreNewSite(this IServiceCollection services) => services
        .AddSingleton<StoreNewSite>(sp => StoreNewSite(sp.GetRequiredService<Database>().GetContainer("sites")));
}

public static class MockSites
{
    public static readonly List<Site> Sites = [
        new Site(
            "google",
            new Uri("https://www.google.com/search?q=google&sourceid=chrome&ie=UTF-8"),
            true),
        new Site(
            "facebook",
            new Uri("https://www.facebook.com"),
            true),
        new Site(
            "my-site",
            new Uri("http://localhost:666"),
            false)
    ];
}