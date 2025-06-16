using System.Text.Json;
using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class WebhookNotificationChannel : INotificationChannel
{
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _factory;
    private readonly ILogger<WebhookNotificationChannel> _logger;

    public WebhookNotificationChannel(IConfiguration config, IHttpClientFactory factory, ILogger<WebhookNotificationChannel> logger)
    {
        _config = config;
        _factory = factory;
        _logger = logger;
    }

    public async Task SendAsync(Notification notification)
    {
        if (!_config.GetValue<bool>("Notifications:EnableWebhookNotifications"))
            return;
        var url = _config["Notifications:WebhookUrl"];
        if (string.IsNullOrEmpty(url)) return;
        try
        {
            var client = _factory.CreateClient();
            await client.PostAsync(url, new StringContent(JsonSerializer.Serialize(notification), System.Text.Encoding.UTF8, "application/json"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Webhook send failed");
        }
    }
}
