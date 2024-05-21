namespace UptimeMonitor.App.SitesAvailability;

public record SiteStatus(Guid Id, bool IsUp, DateTimeOffset CheckedAt);