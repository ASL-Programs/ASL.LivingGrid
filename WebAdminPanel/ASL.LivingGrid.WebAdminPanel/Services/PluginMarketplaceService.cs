using System.Text.Json;
using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class PluginMarketplaceService : IPluginMarketplaceService
{
    private readonly IWebHostEnvironment _env;
    private readonly IHttpClientFactory _factory;
    private readonly ILogger<PluginMarketplaceService> _logger;
    private readonly IConfiguration _config;
    private List<MarketplacePlugin> _plugins = new();

    public PluginMarketplaceService(IWebHostEnvironment env, IHttpClientFactory factory,
        ILogger<PluginMarketplaceService> logger, IConfiguration config)
    {
        _env = env;
        _factory = factory;
        _logger = logger;
        _config = config;
    }

    private async Task LoadAsync()
    {
        if (_plugins.Count > 0) return;
        var source = _config["PluginMarketplace:Source"];
        try
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                var file = Path.Combine(_env.ContentRootPath, "plugin_marketplace.json");
                if (File.Exists(file))
                {
                    var json = await File.ReadAllTextAsync(file);
                    _plugins = JsonSerializer.Deserialize<List<MarketplacePlugin>>(json) ?? new();
                }
            }
            else if (source.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                var client = _factory.CreateClient();
                var json = await client.GetStringAsync(source);
                _plugins = JsonSerializer.Deserialize<List<MarketplacePlugin>>(json) ?? new();
            }
            else
            {
                var file = Path.IsPathRooted(source) ? source : Path.Combine(_env.ContentRootPath, source);
                if (File.Exists(file))
                {
                    var json = await File.ReadAllTextAsync(file);
                    _plugins = JsonSerializer.Deserialize<List<MarketplacePlugin>>(json) ?? new();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading plugins from marketplace {Source}", source);
            _plugins = new();
        }
    }

    public async Task<IEnumerable<MarketplacePlugin>> ListAsync()
    {
        await LoadAsync();
        return _plugins;
    }

    public async Task<Plugin?> ImportAsync(string id)
    {
        await LoadAsync();
        var plugin = _plugins.FirstOrDefault(p => p.Id == id);
        if (plugin == null) return null;
        try
        {
            var client = _factory.CreateClient();
            var json = await client.GetStringAsync(plugin.DownloadUrl);
            var path = Path.Combine(_env.ContentRootPath, "plugins");
            Directory.CreateDirectory(path);
            var file = Path.Combine(path, $"{plugin.Id}.json");
            await File.WriteAllTextAsync(file, json);
            return JsonSerializer.Deserialize<Plugin>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing plugin {Id}", id);
            return null;
        }
    }

    public async Task<string> ExportAsync(string id)
    {
        var file = Path.Combine(_env.ContentRootPath, "plugins", $"{id}.json");
        if (!File.Exists(file)) return string.Empty;
        return await File.ReadAllTextAsync(file);
    }
}
