using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IThemeMarketplaceService
{
    Task<IEnumerable<MarketplaceTheme>> ListAvailableThemesAsync();
    Task<UITheme?> ImportThemeAsync(string themeId);
    Task<string> ExportThemeAsync(string themeId);
}
