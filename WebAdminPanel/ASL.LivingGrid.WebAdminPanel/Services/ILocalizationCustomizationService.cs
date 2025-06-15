using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface ILocalizationCustomizationService
{
    Task<CultureCustomization?> GetAsync(string culture);
    Task SetAsync(CultureCustomization customization);
}
