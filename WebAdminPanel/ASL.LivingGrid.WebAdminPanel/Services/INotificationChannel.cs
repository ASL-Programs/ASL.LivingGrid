using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface INotificationChannel
{
    Task SendAsync(Notification notification);
}
