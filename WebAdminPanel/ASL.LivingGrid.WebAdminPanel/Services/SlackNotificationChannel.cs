using ASL.LivingGrid.WebAdminPanel.Models;
using System.Net.Http.Json;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class SlackNotificationChannel : INotificationChannel
{
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _factory;
    private readonly ILogger<SlackNotificationChannel> _logger;

    public SlackNotificationChannel(IConfiguration config, IHttpClientFactory factory, ILogger<SlackNotificationChannel> logger)
    {
        _config = config;
        _factory = factory;
        _logger = logger;
    }

    public async Task SendAsync(Notification notification)
    {
        if (!_config.GetValue<bool>("Notifications:EnableSlackNotifications"))
            return;
        var url = _config["Notifications:SlackWebhookUrl"];
        if (string.IsNullOrEmpty(url)) return;
        try
        {
            var client = _factory.CreateClient();
            var payload = new { text = $"{notification.Title}: {notification.Message}" };
            await client.PostAsJsonAsync(url, payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Slack send failed");
        }
    }
}
