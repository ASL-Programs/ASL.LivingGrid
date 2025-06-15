using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface ILocalizationService
{
    Task&lt;string&gt; GetStringAsync(string key, string culture = "az", Guid? companyId = null, Guid? tenantId = null);
    Task&lt;IEnumerable&lt;LocalizationResource&gt;&gt; GetAllAsync(string culture, Guid? companyId = null, Guid? tenantId = null);
    Task&lt;IEnumerable&lt;LocalizationResource&gt;&gt; GetByCategoryAsync(string category, string culture, Guid? companyId = null, Guid? tenantId = null);
    Task SetStringAsync(string key, string value, string culture, Guid? companyId = null, Guid? tenantId = null);
    Task DeleteAsync(string key, string culture, Guid? companyId = null, Guid? tenantId = null);
    Task&lt;IEnumerable&lt;string&gt;&gt; GetSupportedCulturesAsync();
    Task&lt;Dictionary&lt;string, string&gt;&gt; GetAllStringsAsync(string culture, Guid? companyId = null, Guid? tenantId = null);
}
