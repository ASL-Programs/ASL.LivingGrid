using System.Text.Json;
using System.Security.Claims;
using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class WidgetPermissionService : IWidgetPermissionService
{
    private readonly ILogger<WidgetPermissionService> _logger;
    private readonly IWebHostEnvironment _env;
    private Dictionary<string, WidgetPermissionEntry> _perms = new();

    public WidgetPermissionService(IWebHostEnvironment env, ILogger<WidgetPermissionService> logger)
    {
        _env = env;
        _logger = logger;
        Load();
    }

    private void Load()
    {
        var file = Path.Combine(_env.ContentRootPath, "widget_permissions.json");
        if (!File.Exists(file)) return;
        try
        {
            var json = File.ReadAllText(file);
            var dict = JsonSerializer.Deserialize<Dictionary<string, WidgetPermissionEntry>>(json);
            if (dict != null)
                _perms = dict;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading widget permissions");
        }
    }

    public bool HasAccess(string widgetId, ClaimsPrincipal user, string? tenantId = null, string? module = null)
    {
        if (!_perms.TryGetValue(widgetId, out var entry))
            return true;
        if (entry.Modules is { Count: > 0 } && (module == null || !entry.Modules.Contains(module)))
            return false;
        if (entry.Tenants is { Count: > 0 } && (tenantId == null || !entry.Tenants.Contains(tenantId)))
            return false;
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        if (entry.Users is { Count: > 0 } && !entry.Users.Contains(userId))
            return false;
        return true;
    }
}
