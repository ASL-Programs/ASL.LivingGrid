using System.Text.Json;
using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class ThemeMarketplaceService : IThemeMarketplaceService
{
    private readonly IWebHostEnvironment _env;
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<ThemeMarketplaceService> _logger;
    private readonly IConfiguration _configuration;
    private List<MarketplaceTheme> _themes = new();

    public ThemeMarketplaceService(IWebHostEnvironment env, IHttpClientFactory clientFactory, ILogger<ThemeMarketplaceService> logger, IConfiguration configuration)
    {
        _env = env;
        _clientFactory = clientFactory;
        _logger = logger;
        _configuration = configuration;
    }

    private async Task LoadAsync()
    {
        if (_themes.Count > 0) return;
        var source = _configuration["ThemeMarketplace:Source"];
        try
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                var file = Path.Combine(_env.ContentRootPath, "theme_marketplace.json");
                if (File.Exists(file))
                {
                    var json = await File.ReadAllTextAsync(file);
                    _themes = JsonSerializer.Deserialize<List<MarketplaceTheme>>(json) ?? new();
                }
            }
            else if (source.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                var client = _clientFactory.CreateClient();
                var json = await client.GetStringAsync(source);
                _themes = JsonSerializer.Deserialize<List<MarketplaceTheme>>(json) ?? new();
            }
            else
            {
                var file = Path.IsPathRooted(source) ? source : Path.Combine(_env.ContentRootPath, source);
                if (File.Exists(file))
                {
                    var json = await File.ReadAllTextAsync(file);
                    _themes = JsonSerializer.Deserialize<List<MarketplaceTheme>>(json) ?? new();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading themes from marketplace source: {Source}", source);
            _themes = new();
        }
    }

    public async Task<IEnumerable<MarketplaceTheme>> ListAvailableThemesAsync()
    {
        await LoadAsync();
        return _themes;
    }

    public async Task<UITheme?> ImportThemeAsync(string themeId)
    {
        await LoadAsync();
        var theme = _themes.FirstOrDefault(t => t.Id == themeId);
        if (theme == null)
            return null;
        try
        {
            var client = _clientFactory.CreateClient();
            var css = await client.GetStringAsync(theme.DownloadUrl);
            var themeFile = Path.Combine(_env.WebRootPath, "css", "themes", $"{theme.Id}.css");
            await File.WriteAllTextAsync(themeFile, css);
            return new UITheme
            {
                Id = theme.Id,
                Name = theme.Name,
                Description = theme.Description,
                Metadata = new ThemeMetadata { PreviewImage = theme.PreviewImage }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing theme {ThemeId}", themeId);
            return null;
        }
    }

    public async Task<string> ExportThemeAsync(string themeId)
    {
        var themeFile = Path.Combine(_env.WebRootPath, "css", "themes", $"{themeId}.css");
        if (!File.Exists(themeFile))
            return string.Empty;
        return await File.ReadAllTextAsync(themeFile);
    }
}
