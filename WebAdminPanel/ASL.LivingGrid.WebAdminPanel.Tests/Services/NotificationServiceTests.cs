using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Models;
using ASL.LivingGrid.WebAdminPanel.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ASL.LivingGrid.WebAdminPanel.Tests.Services;

public class NotificationServiceTests
{
    private static ApplicationDbContext GetContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new ApplicationDbContext(options);
    }

    private static NotificationService CreateService(ApplicationDbContext context,
        params Mock<INotificationChannel>[] channelMocks)
    {
        var logger = new Mock<ILogger<NotificationService>>();
        var channels = channelMocks.Select(m => m.Object).ToList();
        return new NotificationService(context, logger.Object, channels);
    }

    [Fact]
    public async Task CreateAsync_WithoutSchedule_SendsImmediately()
    {
        using var context = GetContext(nameof(CreateAsync_WithoutSchedule_SendsImmediately));
        var email = new Mock<INotificationChannel>();
        var slack = new Mock<SlackNotificationChannel>();
        var service = CreateService(context, email, slack);

        var notification = await service.CreateAsync("t", "m");

        var saved = await context.Notifications.FirstAsync();
        Assert.NotNull(saved.SentAt);
        email.Verify(c => c.SendAsync(It.Is<Notification>(n => n.Id == notification.Id)), Times.Once);
        slack.Verify(c => c.SendAsync(It.Is<Notification>(n => n.Id == notification.Id)), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithFutureSchedule_DoesNotSendImmediately()
    {
        using var context = GetContext(nameof(CreateAsync_WithFutureSchedule_DoesNotSendImmediately));
        var email = new Mock<INotificationChannel>();
        var slack = new Mock<SlackNotificationChannel>();
        var service = CreateService(context, email, slack);

        var future = DateTime.UtcNow.AddMinutes(5);
        await service.CreateAsync("t", "m", scheduledAt: future);

        var saved = await context.Notifications.FirstAsync();
        Assert.Null(saved.SentAt);
        email.Verify(c => c.SendAsync(It.IsAny<Notification>()), Times.Never);
        slack.Verify(c => c.SendAsync(It.IsAny<Notification>()), Times.Never);
    }

    [Fact]
    public async Task ProcessScheduledNotificationsAsync_SendsDueNotifications()
    {
        using var context = GetContext(nameof(ProcessScheduledNotificationsAsync_SendsDueNotifications));
        var email = new Mock<INotificationChannel>();
        var slack = new Mock<SlackNotificationChannel>();
        var service = CreateService(context, email, slack);

        var due = new Notification { Title = "t1", Message = "m1", ScheduledAt = DateTime.UtcNow.AddMinutes(-1) };
        var future = new Notification { Title = "t2", Message = "m2", ScheduledAt = DateTime.UtcNow.AddMinutes(10) };
        context.Notifications.AddRange(due, future);
        await context.SaveChangesAsync();

        await service.ProcessScheduledNotificationsAsync();

        var refreshed = await context.Notifications.OrderBy(n => n.Title).ToListAsync();
        Assert.NotNull(refreshed[0].SentAt); // due notification
        Assert.Null(refreshed[1].SentAt); // future notification
        email.Verify(c => c.SendAsync(It.Is<Notification>(n => n.Id == due.Id)), Times.Once);
        slack.Verify(c => c.SendAsync(It.Is<Notification>(n => n.Id == due.Id)), Times.Once);
    }
}
