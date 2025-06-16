using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IWidgetMarketplaceService
{
    Task<IEnumerable<MarketplaceWidget>> ListAsync();
    Task<WidgetDefinition?> ImportAsync(string id);
    Task<string> ExportAsync(string id);
}
