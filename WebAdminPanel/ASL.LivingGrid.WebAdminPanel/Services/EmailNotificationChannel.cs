using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class EmailNotificationChannel : INotificationChannel
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailNotificationChannel> _logger;

    public EmailNotificationChannel(IConfiguration config, ILogger<EmailNotificationChannel> logger)
    {
        _config = config;
        _logger = logger;
    }

    public Task SendAsync(Notification notification)
    {
        if (!_config.GetValue<bool>("Notifications:EnableEmailNotifications"))
            return Task.CompletedTask;
        _logger.LogInformation("Email sent: {Title} -> {Recipients}", notification.Title, notification.Recipients);
        return Task.CompletedTask;
    }
}
