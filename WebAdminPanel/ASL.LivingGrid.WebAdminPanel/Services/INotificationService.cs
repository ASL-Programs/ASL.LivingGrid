using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface INotificationService
{
    Task&lt;Notification&gt; CreateAsync(string title, string message, string type = "Info", 
        string priority = "Normal", string? recipients = null, DateTime? scheduledAt = null,
        Guid? companyId = null, Guid? tenantId = null, string? userId = null, object? data = null);
    
    Task&lt;IEnumerable&lt;Notification&gt;&gt; GetUserNotificationsAsync(string userId, 
        bool includeRead = true, int skip = 0, int take = 50);
    
    Task&lt;IEnumerable&lt;Notification&gt;&gt; GetCompanyNotificationsAsync(Guid companyId, 
        int skip = 0, int take = 50);
    
    Task&lt;int&gt; GetUnreadCountAsync(string userId);
    
    Task MarkAsReadAsync(Guid notificationId, string userId);
    
    Task MarkAllAsReadAsync(string userId);
    
    Task SendAsync(Guid notificationId);
    
    Task DeleteAsync(Guid notificationId);
    
    Task&lt;IEnumerable&lt;Notification&gt;&gt; GetPendingNotificationsAsync();
    
    Task ProcessScheduledNotificationsAsync();
}
