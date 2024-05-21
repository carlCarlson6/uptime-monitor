using MassTransit;
using Quartz;
using UptimeMonitor.App.UptimePanel;

namespace UptimeMonitor.App.SitesAvailability;

public class SendCheckSitesAvailability(ReadAllSitesDefinitions readAllSitesDefinitions, IPublishEndpoint bus) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await foreach (var site in readAllSitesDefinitions())
            await bus.Publish(new CheckSiteAvailability(site.Id, site.Uri));
    }
}