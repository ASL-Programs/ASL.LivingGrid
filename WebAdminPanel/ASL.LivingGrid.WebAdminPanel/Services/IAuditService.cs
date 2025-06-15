using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IAuditService
{
    Task LogAsync(string action, string entityType, string? entityId = null, 
        string? userId = null, string? userName = null, object? oldValues = null, 
        object? newValues = null, Guid? companyId = null, Guid? tenantId = null,
        string? ipAddress = null, string? userAgent = null);
    
    Task&lt;IEnumerable&lt;AuditLog&gt;&gt; GetLogsAsync(int skip = 0, int take = 50, 
        string? userId = null, string? entityType = null, DateTime? fromDate = null, 
        DateTime? toDate = null, Guid? companyId = null, Guid? tenantId = null);
    
    Task&lt;int&gt; GetLogsCountAsync(string? userId = null, string? entityType = null, 
        DateTime? fromDate = null, DateTime? toDate = null, Guid? companyId = null, 
        Guid? tenantId = null);
    
    Task&lt;AuditLog?&gt; GetLogAsync(Guid id);
    Task DeleteOldLogsAsync(DateTime cutoffDate);
    Task&lt;IEnumerable&lt;AuditLog&gt;&gt; GetUserActivityAsync(string userId, int days = 30);
}
