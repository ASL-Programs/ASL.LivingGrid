using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using System.Text.Json;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class ReportSchedulerService : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<ReportSchedulerService> _logger;

    public ReportSchedulerService(IServiceProvider provider, ILogger<ReportSchedulerService> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _provider.CreateScope();
                var reporting = scope.ServiceProvider.GetRequiredService<IReportingService>();
                var notifications = scope.ServiceProvider.GetRequiredService<INotificationService>();

                var due = await reporting.GetDueScheduledReportsAsync();
                foreach (var item in due)
                {
                    var filter = item.FilterJson != null ?
                        JsonSerializer.Deserialize<ReportFilter>(item.FilterJson) ?? new ReportFilter() : new ReportFilter();
                    var result = await reporting.RunReportAsync(item.ReportId, filter, new ClaimsPrincipal());
                    await notifications.CreateAsync($"Report: {item.Report?.Name}",
                        $"Scheduled report executed at {DateTime.UtcNow}",
                        recipients: item.Recipients);
                    await reporting.MarkScheduleSentAsync(item.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running scheduled reports");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
