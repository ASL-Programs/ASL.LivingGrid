using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IPluginMarketplaceService
{
    Task<IEnumerable<MarketplacePlugin>> ListAsync();
    Task<Plugin?> ImportAsync(string id);
    Task<string> ExportAsync(string id);
}
