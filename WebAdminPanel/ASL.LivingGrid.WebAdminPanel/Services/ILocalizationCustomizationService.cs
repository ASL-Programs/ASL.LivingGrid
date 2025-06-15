using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface ILocalizationCustomizationService
{
    Task<CultureCustomization?> GetAsync(string culture, Guid? companyId = null, Guid? tenantId = null, string? module = null);
    Task SetAsync(CultureCustomization customization);

    Task<TemplateOverride?> GetTemplateAsync(string culture, string module, Guid? companyId = null, Guid? tenantId = null);
    Task SetTemplateAsync(TemplateOverride template);

    Task<TerminologyOverride?> GetTerminologyAsync(string key, string culture, string module, Guid? companyId = null, Guid? tenantId = null);
    Task SetTerminologyAsync(TerminologyOverride term);
}
