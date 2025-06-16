using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class ReportingService : IReportingService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuditService _audit;
    private readonly ILogger<ReportingService> _logger;

    public ReportingService(ApplicationDbContext context, IAuditService audit, ILogger<ReportingService> logger)
    {
        _context = context;
        _audit = audit;
        _logger = logger;
    }

    public async Task<IEnumerable<Report>> GetAccessibleReportsAsync(ClaimsPrincipal user)
    {
        var roles = string.Join(',', user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value));
        return await _context.Reports
            .Where(r => r.AllowedRoles == null || roles.Split(',').Any(role => r.AllowedRoles!.Contains(role)))
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Dictionary<string, object>>> RunReportAsync(Guid reportId, ReportFilter filter, ClaimsPrincipal user)
    {
        var report = await _context.Reports.FindAsync(reportId);
        if (report == null) return Enumerable.Empty<Dictionary<string, object>>();

        await _audit.LogAsync("Run", "Report", reportId.ToString(), user.Identity?.Name, user.Identity?.Name, filter, null);
        // TODO: execute report.Query with parameters from filter
        return Enumerable.Empty<Dictionary<string, object>>();
    }

    public Task<byte[]> ExportReportAsync(Guid reportId, ReportFilter filter, string format, ClaimsPrincipal user)
    {
        // TODO: generate export in requested format
        return Task.FromResult(Array.Empty<byte>());
    }

    public async Task<IEnumerable<AuditLog>> GetAuditTimelineAsync(Guid reportId, ClaimsPrincipal user, int skip = 0, int take = 100)
    {
        return await _context.AuditLogs
            .Where(a => a.EntityType == "Report" && a.EntityId == reportId.ToString())
            .OrderByDescending(a => a.Timestamp)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }
}
