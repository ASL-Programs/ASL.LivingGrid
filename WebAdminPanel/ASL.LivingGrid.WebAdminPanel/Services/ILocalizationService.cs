using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface ILocalizationService
{
    Task<string> GetStringAsync(string key, string culture = "az", Guid? companyId = null, Guid? tenantId = null);
    Task<IEnumerable<LocalizationResource>> GetAllAsync(string culture, Guid? companyId = null, Guid? tenantId = null);
    Task<IEnumerable<LocalizationResource>> GetByCategoryAsync(string category, string culture, Guid? companyId = null, Guid? tenantId = null);
    Task SetStringAsync(string key, string value, string culture, Guid? companyId = null, Guid? tenantId = null);
    Task DeleteAsync(string key, string culture, Guid? companyId = null, Guid? tenantId = null);
    Task<IEnumerable<string>> GetSupportedCulturesAsync();
    Task<Dictionary<string, string>> GetAllStringsAsync(string culture, Guid? companyId = null, Guid? tenantId = null);

    Task BulkSetAsync(IEnumerable<LocalizationResource> resources);
    Task<string> ExportAsync(string culture, Guid? companyId = null, Guid? tenantId = null);
    Task ImportAsync(string jsonContent, string culture, Guid? companyId = null, Guid? tenantId = null);
    Task<IEnumerable<LocalizationResourceVersion>> GetHistoryAsync(Guid resourceId);
    Task ApproveAsync(Guid resourceId, string approvedBy);
}
