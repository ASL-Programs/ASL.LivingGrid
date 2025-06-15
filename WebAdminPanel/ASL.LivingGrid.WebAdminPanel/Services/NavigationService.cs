using System.Text.Json;
using ASL.LivingGrid.WebAdminPanel.Models;
using System.Security.Claims;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class NavigationService : INavigationService
{
    private readonly ILogger<NavigationService> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly IRoleBasedUiService _roleUi;
    public NavigationService(ILogger<NavigationService> logger, IWebHostEnvironment env, IRoleBasedUiService roleUi)
    {
        _logger = logger;
        _env = env;
        _roleUi = roleUi;
    }
    public async Task<IEnumerable<NavigationItem>> GetMenuItemsAsync(ClaimsPrincipal? user = null, string? tenantId = null)
    {
        var fileName = "menuitems.json";
        if (!string.IsNullOrEmpty(tenantId))
        {
            var candidate = Path.Combine(_env.ContentRootPath, $"menuitems.{tenantId}.json");
            if (File.Exists(candidate))
                fileName = $"menuitems.{tenantId}.json";
        }

        var file = Path.Combine(_env.ContentRootPath, fileName);
        IEnumerable<NavigationItem> items;
        if (!File.Exists(file))
        {
            _logger.LogWarning("Menu items file not found: {File}", file);
            items = GetDefaultMenuItems();
        }
        else
        {
            try
            {
                var json = await File.ReadAllTextAsync(file);
                var list = JsonSerializer.Deserialize<List<NavigationItem>>(json);
                items = list ?? GetDefaultMenuItems();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading menu items from {File}", file);
                items = GetDefaultMenuItems();
            }
        }

        if (user == null)
            return items;

        var filtered = new List<NavigationItem>();
        foreach (var item in items)
        {
            if (await _roleUi.HasAccessAsync(item.Key, user))
                filtered.Add(item);
        }

        return filtered;
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
