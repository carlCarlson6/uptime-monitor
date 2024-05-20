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