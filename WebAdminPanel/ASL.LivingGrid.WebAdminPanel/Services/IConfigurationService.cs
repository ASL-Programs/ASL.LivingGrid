using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IConfigurationService
{
    Task&lt;string?&gt; GetValueAsync(string key, Guid? companyId = null, Guid? tenantId = null);
    Task&lt;T?&gt; GetValueAsync&lt;T&gt;(string key, Guid? companyId = null, Guid? tenantId = null);
    Task SetValueAsync(string key, string? value, Guid? companyId = null, Guid? tenantId = null);
    Task&lt;IEnumerable&lt;Configuration&gt;&gt; GetAllAsync(Guid? companyId = null, Guid? tenantId = null);
    Task&lt;IEnumerable&lt;Configuration&gt;&gt; GetByCategoryAsync(string category, Guid? companyId = null, Guid? tenantId = null);
    Task DeleteAsync(string key, Guid? companyId = null, Guid? tenantId = null);
    Task&lt;bool&gt; ExistsAsync(string key, Guid? companyId = null, Guid? tenantId = null);
}
