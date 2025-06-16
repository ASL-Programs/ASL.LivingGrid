using ASL.LivingGrid.WebAdminPanel.Models;
using System.Net;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class TelegramNotificationChannel : INotificationChannel
{
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _factory;
    private readonly ILogger<TelegramNotificationChannel> _logger;

    public TelegramNotificationChannel(IConfiguration config, IHttpClientFactory factory, ILogger<TelegramNotificationChannel> logger)
    {
        _config = config;
        _factory = factory;
        _logger = logger;
    }

    public async Task SendAsync(Notification notification)
    {
        if (!_config.GetValue<bool>("Notifications:EnableTelegramNotifications"))
            return;
        var token = _config["Notifications:TelegramBotToken"];
        var chat = _config["Notifications:TelegramChatId"];
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(chat)) return;
        try
        {
            var client = _factory.CreateClient();
            var text = WebUtility.UrlEncode(notification.Message);
            await client.GetAsync($"https://api.telegram.org/bot{token}/sendMessage?chat_id={chat}&text={text}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Telegram send failed");
        }
    }
}
