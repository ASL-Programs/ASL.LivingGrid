using System.Text.Json;
using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class TranslationProviderService : ITranslationProviderService
{
    private readonly IWebHostEnvironment _env;
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<TranslationProviderService> _logger;
    private readonly string _file;
    private List<TranslationProvider> _providers = new();

    public TranslationProviderService(IWebHostEnvironment env, IHttpClientFactory clientFactory, ILogger<TranslationProviderService> logger)
    {
        _env = env;
        _clientFactory = clientFactory;
        _logger = logger;
        _file = Path.Combine(_env.ContentRootPath, "translation_providers.json");
        Load();
    }

    private void Load()
    {
        if (File.Exists(_file))
        {
            var json = File.ReadAllText(_file);
            _providers = JsonSerializer.Deserialize<List<TranslationProvider>>(json) ?? new();
        }
    }

    private void Save()
    {
        File.WriteAllText(_file, JsonSerializer.Serialize(_providers));
    }

    public Task<IEnumerable<TranslationProvider>> GetProvidersAsync()
    {
        return Task.FromResult<IEnumerable<TranslationProvider>>(_providers);
    }

    public Task AddAsync(TranslationProvider provider)
    {
        _providers.RemoveAll(p => p.Id == provider.Id);
        _providers.Add(provider);
        Save();
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string id)
    {
        _providers.RemoveAll(p => p.Id == id);
        Save();
        return Task.CompletedTask;
    }

    public async Task TriggerWebhookAsync(string id, object payload)
    {
        var provider = _providers.FirstOrDefault(p => p.Id == id);
        if (provider == null || string.IsNullOrEmpty(provider.WebhookUrl)) return;
        try
        {
            var client = _clientFactory.CreateClient();
            await client.PostAsync(provider.WebhookUrl, new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error triggering webhook for provider {Id}", id);
        }
    }
}
