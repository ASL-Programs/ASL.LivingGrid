namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IModuleCustomizationService
{
    Task SetModuleThemeAsync(string module, string themeId, string? tenantId = null);
    Task<string?> GetModuleThemeAsync(string module, string? tenantId = null);
}
