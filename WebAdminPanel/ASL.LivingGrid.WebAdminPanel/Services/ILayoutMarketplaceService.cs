using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface ILayoutMarketplaceService
{
    Task<IEnumerable<MarketplaceLayout>> ListAvailableLayoutsAsync();
    Task<UILayout?> ImportLayoutAsync(string layoutId);
    Task<string> ExportLayoutAsync(string layoutId);
}
