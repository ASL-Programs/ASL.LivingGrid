using System.Text.Json;
using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class WidgetMarketplaceService : IWidgetMarketplaceService
{
    private readonly IWebHostEnvironment _env;
    private readonly IHttpClientFactory _factory;
    private readonly ILogger<WidgetMarketplaceService> _logger;
    private readonly IConfiguration _config;
    private List<MarketplaceWidget> _widgets = new();

    public WidgetMarketplaceService(IWebHostEnvironment env, IHttpClientFactory factory,
        ILogger<WidgetMarketplaceService> logger, IConfiguration config)
    {
        _env = env;
        _factory = factory;
        _logger = logger;
        _config = config;
    }

    private async Task LoadAsync()
    {
        if (_widgets.Count > 0) return;
        var source = _config["WidgetMarketplace:Source"];
        try
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                var file = Path.Combine(_env.ContentRootPath, "widget_marketplace.json");
                if (File.Exists(file))
                {
                    var json = await File.ReadAllTextAsync(file);
                    _widgets = JsonSerializer.Deserialize<List<MarketplaceWidget>>(json) ?? new();
                }
            }
            else if (source.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                var client = _factory.CreateClient();
                var json = await client.GetStringAsync(source);
                _widgets = JsonSerializer.Deserialize<List<MarketplaceWidget>>(json) ?? new();
            }
            else
            {
                var file = Path.IsPathRooted(source) ? source : Path.Combine(_env.ContentRootPath, source);
                if (File.Exists(file))
                {
                    var json = await File.ReadAllTextAsync(file);
                    _widgets = JsonSerializer.Deserialize<List<MarketplaceWidget>>(json) ?? new();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading widgets from marketplace {Source}", source);
            _widgets = new();
        }
    }

    public async Task<IEnumerable<MarketplaceWidget>> ListAsync()
    {
        await LoadAsync();
        return _widgets;
    }

    public async Task<WidgetDefinition?> ImportAsync(string id)
    {
        await LoadAsync();
        var widget = _widgets.FirstOrDefault(w => w.Id == id);
        if (widget == null) return null;
        try
        {
            var client = _factory.CreateClient();
            var json = await client.GetStringAsync(widget.DownloadUrl);
            var path = Path.Combine(_env.WebRootPath, "widgets");
            Directory.CreateDirectory(path);
            var file = Path.Combine(path, $"{widget.Id}.json");
            await File.WriteAllTextAsync(file, json);
            return JsonSerializer.Deserialize<WidgetDefinition>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing widget {Id}", id);
            return null;
        }
    }

    public async Task<string> ExportAsync(string id)
    {
        var file = Path.Combine(_env.WebRootPath, "widgets", $"{id}.json");
        if (!File.Exists(file)) return string.Empty;
        return await File.ReadAllTextAsync(file);
    }
}
