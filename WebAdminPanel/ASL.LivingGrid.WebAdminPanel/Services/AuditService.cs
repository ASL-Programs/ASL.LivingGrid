using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger&lt;AuditService&gt; _logger;

    public AuditService(ApplicationDbContext context, ILogger&lt;AuditService&gt; logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task LogAsync(string action, string entityType, string? entityId = null, 
        string? userId = null, string? userName = null, object? oldValues = null, 
        object? newValues = null, Guid? companyId = null, Guid? tenantId = null,
        string? ipAddress = null, string? userAgent = null)
    {
        try
        {
            var auditLog = new AuditLog
            {
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                UserId = userId,
                UserName = userName,
                IPAddress = ipAddress,
                UserAgent = userAgent,
                CompanyId = companyId,
                TenantId = tenantId,
                Timestamp = DateTime.UtcNow,
                OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
                NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Audit log created: {Action} on {EntityType} by {UserName}", 
                action, entityType, userName ?? "System");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating audit log for action: {Action} on {EntityType}", 
                action, entityType);
            // Don't throw - audit logging should not break the main operation
        }
    }

    public async Task&lt;IEnumerable&lt;AuditLog&gt;&gt; GetLogsAsync(int skip = 0, int take = 50, 
        string? userId = null, string? entityType = null, DateTime? fromDate = null, 
        DateTime? toDate = null, Guid? companyId = null, Guid? tenantId = null)
    {
        try
        {
            var query = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(a =&gt; a.UserId == userId);

            if (!string.IsNullOrEmpty(entityType))
                query = query.Where(a =&gt; a.EntityType == entityType);

            if (fromDate.HasValue)
                query = query.Where(a =&gt; a.Timestamp &gt;= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a =&gt; a.Timestamp &lt;= toDate.Value);

            if (companyId.HasValue)
                query = query.Where(a =&gt; a.CompanyId == companyId);

            if (tenantId.HasValue)
                query = query.Where(a =&gt; a.TenantId == tenantId);

            return await query
                .OrderByDescending(a =&gt; a.Timestamp)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting audit logs");
            return Enumerable.Empty&lt;AuditLog&gt;();
        }
    }

    public async Task&lt;int&gt; GetLogsCountAsync(string? userId = null, string? entityType = null, 
        DateTime? fromDate = null, DateTime? toDate = null, Guid? companyId = null, 
        Guid? tenantId = null)
    {
        try
        {
            var query = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(a =&gt; a.UserId == userId);

            if (!string.IsNullOrEmpty(entityType))
                query = query.Where(a =&gt; a.EntityType == entityType);

            if (fromDate.HasValue)
                query = query.Where(a =&gt; a.Timestamp &gt;= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a =&gt; a.Timestamp &lt;= toDate.Value);

            if (companyId.HasValue)
                query = query.Where(a =&gt; a.CompanyId == companyId);

            if (tenantId.HasValue)
                query = query.Where(a =&gt; a.TenantId == tenantId);

            return await query.CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting audit logs count");
            return 0;
        }
    }

    public async Task&lt;AuditLog?&gt; GetLogAsync(Guid id)
    {
        try
        {
            return await _context.AuditLogs.FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting audit log: {Id}", id);
            return null;
        }
    }

    public async Task DeleteOldLogsAsync(DateTime cutoffDate)
    {
        try
        {
            var oldLogs = await _context.AuditLogs
                .Where(a =&gt; a.Timestamp &lt; cutoffDate)
                .ToListAsync();

            if (oldLogs.Any())
            {
                _context.AuditLogs.RemoveRange(oldLogs);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Deleted {Count} old audit logs before {CutoffDate}", 
                    oldLogs.Count, cutoffDate);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting old audit logs");
            throw;
        }
    }

    public async Task&lt;IEnumerable&lt;AuditLog&gt;&gt; GetUserActivityAsync(string userId, int days = 30)
    {
        try
        {
            var fromDate = DateTime.UtcNow.AddDays(-days);
            
            return await _context.AuditLogs
                .Where(a =&gt; a.UserId == userId &amp;&amp; a.Timestamp &gt;= fromDate)
                .OrderByDescending(a =&gt; a.Timestamp)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user activity for user: {UserId}", userId);
            return Enumerable.Empty&lt;AuditLog&gt;();
        }
    }
}
