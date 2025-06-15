using System.Text.Json;
using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class NavigationService : INavigationService
{
    private readonly ILogger<NavigationService> _logger;
    private readonly IWebHostEnvironment _env;

    public NavigationService(ILogger<NavigationService> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async Task<IEnumerable<NavigationItem>> GetMenuItemsAsync()
    {
        var file = Path.Combine(_env.ContentRootPath, "menuitems.json");
        if (!File.Exists(file))
        {
            _logger.LogWarning("Menu items file not found: {File}", file);
            return GetDefaultMenuItems();
        }

        try
        {
            var json = await File.ReadAllTextAsync(file);
            var items = JsonSerializer.Deserialize<List<NavigationItem>>(json);
            return items ?? GetDefaultMenuItems();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading menu items from {File}", file);
            return GetDefaultMenuItems();
        }
    }

    private static IEnumerable<NavigationItem> GetDefaultMenuItems() => new List<NavigationItem>
    {
        new NavigationItem { Key = "Navigation.Dashboard", Url = "", Icon = "oi oi-home" },
        new NavigationItem { Key = "Navigation.Companies", Url = "companies", Icon = "oi oi-people" },
        new NavigationItem { Key = "Navigation.Users", Url = "users", Icon = "oi oi-person" },
        new NavigationItem { Key = "Navigation.Roles", Url = "roles", Icon = "oi oi-key" },
        new NavigationItem { Key = "Navigation.Settings", Url = "settings", Icon = "oi oi-cog" },
        new NavigationItem { Key = "Navigation.Audit", Url = "audit", Icon = "oi oi-clipboard" },
        new NavigationItem { Key = "Navigation.UIAudit", Url = "uiaudit", Icon = "oi oi-eye" },
        new NavigationItem { Key = "Navigation.Notifications", Url = "notifications", Icon = "oi oi-bell" },
        new NavigationItem { Key = "Navigation.Plugins", Url = "plugins", Icon = "oi oi-puzzle-piece" }
    };
}
