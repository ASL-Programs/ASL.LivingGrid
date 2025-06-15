using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface ITranslationProviderService
{
    Task<IEnumerable<TranslationProvider>> GetProvidersAsync();
    Task AddAsync(TranslationProvider provider);
    Task DeleteAsync(string id);
    Task TriggerWebhookAsync(string id, object payload);
}
