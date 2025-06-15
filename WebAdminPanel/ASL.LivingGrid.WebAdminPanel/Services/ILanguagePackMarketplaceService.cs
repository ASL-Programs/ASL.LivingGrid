using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface ILanguagePackMarketplaceService
{
    Task<IEnumerable<MarketplaceLanguagePack>> ListAsync();
    Task<Dictionary<string, string>> ImportAsync(string packId);
    Task<string> ExportAsync(string culture);
    Task RateAsync(string packId, int rating);
}
