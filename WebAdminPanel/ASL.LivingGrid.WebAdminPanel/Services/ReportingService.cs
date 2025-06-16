using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

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

        if (string.IsNullOrWhiteSpace(report.Query))
            return Enumerable.Empty<Dictionary<string, object>>();

        var result = new List<Dictionary<string, object>>();
        await using var conn = _context.Database.GetDbConnection();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = report.Query;

        var keywordParam = cmd.CreateParameter();
        keywordParam.ParameterName = "@keyword";
        keywordParam.Value = (object?)filter.Keyword ?? DBNull.Value;
        cmd.Parameters.Add(keywordParam);

        var fromDateParam = cmd.CreateParameter();
        fromDateParam.ParameterName = "@fromDate";
        fromDateParam.Value = (object?)filter.FromDate ?? DBNull.Value;
        cmd.Parameters.Add(fromDateParam);

        var toDateParam = cmd.CreateParameter();
        toDateParam.ParameterName = "@toDate";
        toDateParam.Value = (object?)filter.ToDate ?? DBNull.Value;
        cmd.Parameters.Add(toDateParam);

        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync();

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var val = reader.IsDBNull(i) ? null! : reader.GetValue(i);
                row[reader.GetName(i)] = val;
            }
            result.Add(row);
        }
        return result;
    }

    public async Task<byte[]> ExportReportAsync(Guid reportId, ReportFilter filter, string format, ClaimsPrincipal user)
    {
        var data = (await RunReportAsync(reportId, filter, user)).ToList();
        format = format.ToLowerInvariant();

        return format switch
        {
            "json" => JsonSerializer.SerializeToUtf8Bytes(data),
            "csv" => Encoding.UTF8.GetBytes(ToCsv(data)),
            "excel" => Encoding.UTF8.GetBytes(ToCsv(data)),
            "pdf" => GenerateSimplePdf(data),
            _ => Array.Empty<byte>()
        };
    }

    private static string ToCsv(IReadOnlyList<Dictionary<string, object>> data)
    {
        if (data.Count == 0) return string.Empty;
        var sb = new StringBuilder();
        var headers = data[0].Keys.ToList();
        sb.AppendLine(string.Join(',', headers));
        foreach (var row in data)
        {
            var values = headers.Select(h => EscapeCsv(row.TryGetValue(h, out var v) ? v : null));
            sb.AppendLine(string.Join(',', values));
        }
        return sb.ToString();
    }

    private static string EscapeCsv(object? value)
    {
        if (value == null) return string.Empty;
        var s = value.ToString()?.Replace("\"", "\"\"") ?? string.Empty;
        if (s.Contains(',') || s.Contains('\n') || s.Contains('\r'))
        {
            s = $"\"{s}\"";
        }
        return s;
    }

    private static byte[] GenerateSimplePdf(IReadOnlyList<Dictionary<string, object>> data)
    {
        var text = ToCsv(data);
        // very basic PDF with single text object
        var content = $"BT /F1 12 Tf 50 750 Td ({text.Replace("(", "\\(").Replace(")", "\\)")}) Tj ET";
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var objects = new List<string>
        {
            "1 0 obj<< /Type /Catalog /Pages 2 0 R >>endobj",
            "2 0 obj<< /Type /Pages /Kids [3 0 R] /Count 1 >>endobj",
            $"3 0 obj<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R /Resources<< /Font<< /F1 5 0 R >> >> >>endobj",
            $"4 0 obj<< /Length {contentBytes.Length} >>stream\n{content}\nendstream endobj",
            "5 0 obj<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>endobj"
        };
        var sb = new StringBuilder();
        sb.AppendLine("%PDF-1.4");
        foreach (var o in objects) sb.AppendLine(o);
        sb.AppendLine($"xref 0 {objects.Count + 1}");
        sb.AppendLine("0000000000 65535 f ");
        var offset = 9; // header length
        foreach (var o in objects)
        {
            sb.AppendLine(offset.ToString("0000000000") + " 00000 n ");
            offset += o.Length + 1; // newline
        }
        sb.AppendLine("trailer<< /Size " + (objects.Count + 1) + " /Root 1 0 R>>");
        sb.AppendLine("startxref");
        sb.AppendLine(offset.ToString());
        sb.AppendLine("%%EOF");
        return Encoding.ASCII.GetBytes(sb.ToString());
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

    public async Task<Report> SaveReportAsync(Report report, ClaimsPrincipal user)
    {
        if (_context.Reports.Any(r => r.Id == report.Id))
        {
            _context.Reports.Update(report);
        }
        else
        {
            _context.Reports.Add(report);
        }
        await _context.SaveChangesAsync();
        await _audit.LogAsync("Save", "Report", report.Id.ToString(), user.Identity?.Name, user.Identity?.Name, null, null);
        return report;
    }

    public async Task ScheduleReportAsync(Guid reportId, ReportFilter filter, DateTime scheduledAt, string? recipients, ClaimsPrincipal user)
    {
        var sched = new ScheduledReport
        {
            ReportId = reportId,
            FilterJson = JsonSerializer.Serialize(filter),
            ScheduledAt = scheduledAt,
            Recipients = recipients,
            CreatedBy = user.Identity?.Name
        };
        _context.ScheduledReports.Add(sched);
        await _context.SaveChangesAsync();
        await _audit.LogAsync("Schedule", "Report", reportId.ToString(), user.Identity?.Name, user.Identity?.Name, filter, null);
    }

    public async Task<IEnumerable<ScheduledReport>> GetDueScheduledReportsAsync()
    {
        return await _context.ScheduledReports
            .Include(s => s.Report)
            .Where(s => s.ScheduledAt <= DateTime.UtcNow && s.SentAt == null)
            .ToListAsync();
    }

    public async Task MarkScheduleSentAsync(Guid scheduleId)
    {
        var sched = await _context.ScheduledReports.FindAsync(scheduleId);
        if (sched != null)
        {
            sched.SentAt = DateTime.UtcNow;
            sched.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
