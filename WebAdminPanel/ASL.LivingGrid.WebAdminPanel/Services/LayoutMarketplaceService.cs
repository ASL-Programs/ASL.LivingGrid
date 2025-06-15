using System.Text.Json;
using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class LayoutMarketplaceService : ILayoutMarketplaceService
{
    private readonly IWebHostEnvironment _env;
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<LayoutMarketplaceService> _logger;
    private readonly IConfiguration _configuration;
    private List<MarketplaceLayout> _layouts = new();

    public LayoutMarketplaceService(IWebHostEnvironment env, IHttpClientFactory clientFactory,
        ILogger<LayoutMarketplaceService> logger, IConfiguration configuration)
    {
        _env = env;
        _clientFactory = clientFactory;
        _logger = logger;
        _configuration = configuration;
    }

    private async Task LoadAsync()
    {
        if (_layouts.Count > 0) return;
        var source = _configuration["LayoutMarketplace:Source"];
        try
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                var file = Path.Combine(_env.ContentRootPath, "layout_marketplace.json");
                if (File.Exists(file))
                {
                    var json = await File.ReadAllTextAsync(file);
                    _layouts = JsonSerializer.Deserialize<List<MarketplaceLayout>>(json) ?? new();
                }
            }
            else if (source.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                var client = _clientFactory.CreateClient();
                var json = await client.GetStringAsync(source);
                _layouts = JsonSerializer.Deserialize<List<MarketplaceLayout>>(json) ?? new();
            }
            else
            {
                var file = Path.IsPathRooted(source) ? source : Path.Combine(_env.ContentRootPath, source);
                if (File.Exists(file))
                {
                    var json = await File.ReadAllTextAsync(file);
                    _layouts = JsonSerializer.Deserialize<List<MarketplaceLayout>>(json) ?? new();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading layouts from marketplace source: {Source}", source);
            _layouts = new();
        }
    }

    public async Task<IEnumerable<MarketplaceLayout>> ListAvailableLayoutsAsync()
    {
        await LoadAsync();
        return _layouts;
    }

    public async Task<UILayout?> ImportLayoutAsync(string layoutId)
    {
        await LoadAsync();
        var layout = _layouts.FirstOrDefault(t => t.Id == layoutId);
        if (layout == null)
            return null;
        try
        {
            var client = _clientFactory.CreateClient();
            var json = await client.GetStringAsync(layout.DownloadUrl);
            var layoutDir = Path.Combine(_env.WebRootPath, "layouts");
            Directory.CreateDirectory(layoutDir);
            var layoutFile = Path.Combine(layoutDir, $"{layout.Id}.json");
            await File.WriteAllTextAsync(layoutFile, json);
            return new UILayout
            {
                Id = layout.Id,
                Name = layout.Name,
                Description = layout.Description,
                FilePath = layoutFile,
                IsActive = false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing layout {LayoutId}", layoutId);
            return null;
        }
    }

    public async Task<string> ExportLayoutAsync(string layoutId)
    {
        var layoutFile = Path.Combine(_env.WebRootPath, "layouts", $"{layoutId}.json");
        if (!File.Exists(layoutFile))
            return string.Empty;
        return await File.ReadAllTextAsync(layoutFile);
    }
}
