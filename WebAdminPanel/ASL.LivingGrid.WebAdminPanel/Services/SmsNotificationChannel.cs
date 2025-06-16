using ASL.LivingGrid.WebAdminPanel.Models;
using System.Net.Http.Json;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class SmsNotificationChannel : INotificationChannel
{
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _factory;
    private readonly ILogger<SmsNotificationChannel> _logger;

    public SmsNotificationChannel(IConfiguration config, IHttpClientFactory factory, ILogger<SmsNotificationChannel> logger)
    {
        _config = config;
        _factory = factory;
        _logger = logger;
    }

    public async Task SendAsync(Notification notification)
    {
        if (!_config.GetValue<bool>("Notifications:EnableSmsNotifications"))
            return;
        var url = _config["Notifications:SmsApiUrl"];
        var key = _config["Notifications:SmsApiKey"];
        if (string.IsNullOrEmpty(url)) return;
        try
        {
            var client = _factory.CreateClient();
            var payload = new { to = notification.Recipients, text = notification.Message, key };
            await client.PostAsJsonAsync(url, payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SMS send failed");
        }
    }
}
