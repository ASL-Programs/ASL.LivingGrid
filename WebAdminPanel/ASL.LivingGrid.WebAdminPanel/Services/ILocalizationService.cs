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

    /// <summary>
    /// Validate placeholder usage against the default culture and return keys with issues.
    /// </summary>
    Task<IEnumerable<string>> ValidatePlaceholdersAsync(string culture);

    /// <summary>
    /// Returns keys whose translated values exceed the specified maximum length.
    /// </summary>
    Task<Dictionary<string, int>> GetOverflowStringsAsync(string culture, int maxLength = 60);

    /// <summary>
    /// Event raised when a translation is missing for the requested culture.
    /// </summary>
    event Func<string, string, Task>? MissingTranslation;

    /// <summary>
    /// Returns coverage percentage per category/module for the specified culture.
    /// </summary>
    Task<Dictionary<string, double>> GetCoverageByCategoryAsync(string culture);

    /// <summary>
    /// Returns keys that are not translated in the specified culture compared to the default culture.
    /// </summary>
    Task<IEnumerable<string>> GetMissingKeysAsync(string culture);
}
