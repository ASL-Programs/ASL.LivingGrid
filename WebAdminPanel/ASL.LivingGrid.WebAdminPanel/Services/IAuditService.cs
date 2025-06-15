using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IAuditService
{
    Task LogAsync(string action, string entityType, string? entityId = null, 
        string? userId = null, string? userName = null, object? oldValues = null, 
        object? newValues = null, Guid? companyId = null, Guid? tenantId = null,
        string? ipAddress = null, string? userAgent = null);
    
    Task<IEnumerable<AuditLog>> GetLogsAsync(int skip = 0, int take = 50, 
        string? userId = null, string? entityType = null, DateTime? fromDate = null, 
        DateTime? toDate = null, Guid? companyId = null, Guid? tenantId = null);
    
    Task<int> GetLogsCountAsync(string? userId = null, string? entityType = null, 
        DateTime? fromDate = null, DateTime? toDate = null, Guid? companyId = null, 
        Guid? tenantId = null);
    
    Task<AuditLog?> GetLogAsync(Guid id);
    Task DeleteOldLogsAsync(DateTime cutoffDate);
    Task<IEnumerable<AuditLog>> GetUserActivityAsync(string userId, int days = 30);
}
