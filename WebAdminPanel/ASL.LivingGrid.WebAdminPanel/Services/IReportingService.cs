using ASL.LivingGrid.WebAdminPanel.Models;
using System.Security.Claims;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IReportingService
{
    Task<IEnumerable<Report>> GetAccessibleReportsAsync(ClaimsPrincipal user);
    Task<IEnumerable<Dictionary<string, object>>> RunReportAsync(Guid reportId, ReportFilter filter, ClaimsPrincipal user);
    Task<byte[]> ExportReportAsync(Guid reportId, ReportFilter filter, string format, ClaimsPrincipal user);
    Task<IEnumerable<AuditLog>> GetAuditTimelineAsync(Guid reportId, ClaimsPrincipal user, int skip = 0, int take = 100);
}
