using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IConfigurationService
{
    Task<string?> GetValueAsync(string key, Guid? companyId = null, Guid? tenantId = null);
    Task<T?> GetValueAsync<T>(string key, Guid? companyId = null, Guid? tenantId = null);
    Task SetValueAsync(string key, string? value, Guid? companyId = null, Guid? tenantId = null, bool isSecret = false);
    Task<IEnumerable<Configuration>> GetAllAsync(Guid? companyId = null, Guid? tenantId = null);
    Task<IEnumerable<Configuration>> GetByCategoryAsync(string category, Guid? companyId = null, Guid? tenantId = null);
    Task DeleteAsync(string key, Guid? companyId = null, Guid? tenantId = null);
    Task<bool> ExistsAsync(string key, Guid? companyId = null, Guid? tenantId = null);
    Task<bool> RollbackValueAsync(string key, Guid? companyId = null, Guid? tenantId = null);
}
