using System.Text.Json;
using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class LanguagePackMarketplaceService : ILanguagePackMarketplaceService
{
    private readonly IWebHostEnvironment _env;
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<LanguagePackMarketplaceService> _logger;
    private readonly IConfiguration _configuration;
    private List<MarketplaceLanguagePack> _packs = new();
    private readonly Dictionary<string, List<int>> _ratings = new();

    public LanguagePackMarketplaceService(IWebHostEnvironment env, IHttpClientFactory clientFactory, ILogger<LanguagePackMarketplaceService> logger, IConfiguration configuration)
    {
        _env = env;
        _clientFactory = clientFactory;
        _logger = logger;
        _configuration = configuration;
    }

    private async Task LoadAsync()
    {
        if (_packs.Count > 0) return;
        var source = _configuration["LanguagePackMarketplace:Source"];
        try
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                var file = Path.Combine(_env.ContentRootPath, "languagepacks_marketplace.json");
                if (File.Exists(file))
                {
                    var json = await File.ReadAllTextAsync(file);
                    _packs = JsonSerializer.Deserialize<List<MarketplaceLanguagePack>>(json) ?? new();
                }
            }
            else if (source.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                var client = _clientFactory.CreateClient();
                var json = await client.GetStringAsync(source);
                _packs = JsonSerializer.Deserialize<List<MarketplaceLanguagePack>>(json) ?? new();
            }
            else
            {
                var file = Path.IsPathRooted(source) ? source : Path.Combine(_env.ContentRootPath, source);
                if (File.Exists(file))
                {
                    var json = await File.ReadAllTextAsync(file);
                    _packs = JsonSerializer.Deserialize<List<MarketplaceLanguagePack>>(json) ?? new();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading language packs from marketplace source: {Source}", source);
            _packs = new();
        }
    }

    public async Task<IEnumerable<MarketplaceLanguagePack>> ListAsync()
    {
        await LoadAsync();
        foreach (var p in _packs)
        {
            if (_ratings.TryGetValue(p.Id, out var list) && list.Count > 0)
            {
                p.Rating = list.Average();
                p.RatingsCount = list.Count;
            }
        }
        return _packs;
    }

    public async Task<Dictionary<string, string>> ImportAsync(string packId)
    {
        await LoadAsync();
        var pack = _packs.FirstOrDefault(p => p.Id == packId);
        if (pack == null) return new();
        try
        {
            var client = _clientFactory.CreateClient();
            var json = await client.GetStringAsync(pack.DownloadUrl);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing language pack {PackId}", packId);
            return new();
        }
    }

    public async Task<string> ExportAsync(string culture)
    {
        var file = Path.Combine(_env.ContentRootPath, "exports", $"lang_{culture}.json");
        if (!File.Exists(file)) return string.Empty;
        return await File.ReadAllTextAsync(file);
    }

    public Task RateAsync(string packId, int rating)
    {
        if (!_ratings.ContainsKey(packId))
            _ratings[packId] = new();
        _ratings[packId].Add(rating);
        return Task.CompletedTask;
    }
}
