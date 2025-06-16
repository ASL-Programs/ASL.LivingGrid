using ASL.LivingGrid.WebAdminPanel.Models;
using System.Security.Claims;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IReportingService
{
    Task<IEnumerable<Report>> GetAccessibleReportsAsync(ClaimsPrincipal user);
    Task<IEnumerable<Dictionary<string, object>>> RunReportAsync(Guid reportId, ReportFilter filter, ClaimsPrincipal user);
    Task<byte[]> ExportReportAsync(Guid reportId, ReportFilter filter, string format, ClaimsPrincipal user);
    Task<IEnumerable<AuditLog>> GetAuditTimelineAsync(Guid reportId, ClaimsPrincipal user, int skip = 0, int take = 100);
    Task<Report> SaveReportAsync(Report report, ClaimsPrincipal user);
    Task ScheduleReportAsync(Guid reportId, ReportFilter filter, DateTime scheduledAt, string? recipients, ClaimsPrincipal user);
    Task<IEnumerable<ScheduledReport>> GetDueScheduledReportsAsync();
    Task MarkScheduleSentAsync(Guid scheduleId);
}
