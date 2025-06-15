using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ApplicationDbContext context, ILogger<NotificationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Notification> CreateAsync(string title, string message, string type = "Info", 
        string priority = "Normal", string? recipients = null, DateTime? scheduledAt = null,
        Guid? companyId = null, Guid? tenantId = null, string? userId = null, object? data = null)
    {
        try
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,
                Type = type,
                Priority = priority,
                Recipients = recipients,
                ScheduledAt = scheduledAt,
                CompanyId = companyId,
                TenantId = tenantId,
                UserId = userId,
                Data = data != null ? JsonSerializer.Serialize(data) : null,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Notification created: {Title} for {Recipients}", title, recipients ?? userId ?? "All");

            // If not scheduled, send immediately
            if (!scheduledAt.HasValue)
            {
                await SendAsync(notification.Id);
            }

            return notification;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating notification: {Title}", title);
            throw;
        }
    }

    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId, 
        bool includeRead = true, int skip = 0, int take = 50)
    {
        try
        {
            var query = _context.Notifications
                .Where(n => n.UserId == userId || n.UserId == null); // User-specific or broadcast

            if (!includeRead)
                query = query.Where(n => !n.IsRead);

            return await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user notifications for user: {UserId}", userId);
            return Enumerable.Empty<Notification>();
        }
    }

    public async Task<IEnumerable<Notification>> GetCompanyNotificationsAsync(Guid companyId, 
        int skip = 0, int take = 50)
    {
        try
        {
            return await _context.Notifications
                .Where(n => n.CompanyId == companyId)
                .OrderByDescending(n => n.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting company notifications for company: {CompanyId}", companyId);
            return Enumerable.Empty<Notification>();
        }
    }

    public async Task<int> GetUnreadCountAsync(string userId)
    {
        try
        {
            return await _context.Notifications
                .Where(n => (n.UserId == userId || n.UserId == null) && !n.IsRead)
                .CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread count for user: {UserId}", userId);
            return 0;
        }
    }

    public async Task MarkAsReadAsync(Guid notificationId, string userId)
    {
        try
        {
            var notification = await _context.Notifications
                .Where(n => n.Id == notificationId && (n.UserId == userId || n.UserId == null))
                .FirstOrDefaultAsync();

            if (notification != null)
            {
                notification.IsRead = true;
                notification.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Notification marked as read: {NotificationId} by {UserId}", 
                    notificationId, userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification as read: {NotificationId}", notificationId);
            throw;
        }
    }

    public async Task MarkAllAsReadAsync(string userId)
    {
        try
        {
            var notifications = await _context.Notifications
                .Where(n => (n.UserId == userId || n.UserId == null) && !n.IsRead)
                .ToListAsync();

            if (notifications.Any())
            {
                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                    notification.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("All notifications marked as read for user: {UserId}", userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read for user: {UserId}", userId);
            throw;
        }
    }

    public async Task SendAsync(Guid notificationId)
    {
        try
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null && !notification.SentAt.HasValue)
            {
                notification.SentAt = DateTime.UtcNow;
                notification.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Notification sent: {NotificationId}", notificationId);

                // Here you would integrate with actual notification providers
                // (Email, SMS, Push notifications, etc.)
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification: {NotificationId}", notificationId);
            throw;
        }
    }

    public async Task DeleteAsync(Guid notificationId)
    {
        try
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Notification deleted: {NotificationId}", notificationId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting notification: {NotificationId}", notificationId);
            throw;
        }
    }

    public async Task<IEnumerable<Notification>> GetPendingNotificationsAsync()
    {
        try
        {
            return await _context.Notifications
                .Where(n => n.ScheduledAt.HasValue && n.ScheduledAt <= DateTime.UtcNow && !n.SentAt.HasValue)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending notifications");
            return Enumerable.Empty<Notification>();
        }
    }

    public async Task ProcessScheduledNotificationsAsync()
    {
        try
        {
            var pendingNotifications = await GetPendingNotificationsAsync();
            
            foreach (var notification in pendingNotifications)
            {
                await SendAsync(notification.Id);
            }

            if (pendingNotifications.Any())
            {
                _logger.LogInformation("Processed {Count} scheduled notifications", 
                    pendingNotifications.Count());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing scheduled notifications");
            throw;
        }
    }
}
