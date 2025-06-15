using System.Text.Json;
using System.Security.Claims;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class RoleBasedUiService : IRoleBasedUiService
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<RoleBasedUiService> _logger;
    private Dictionary<string, string[]> _permissions = new();
    private string? _simulationRole;

    public RoleBasedUiService(IWebHostEnvironment env, ILogger<RoleBasedUiService> logger)
    {
        _env = env;
        _logger = logger;
        LoadPermissions();
    }

    private void LoadPermissions()
    {
        var file = Path.Combine(_env.ContentRootPath, "menu_permissions.json");
        if (!File.Exists(file)) return;
        try
        {
            var json = File.ReadAllText(file);
            _permissions = JsonSerializer.Deserialize<Dictionary<string, string[]>>(json) ?? new();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load menu permissions from {File}", file);
        }
    }

    public Task<bool> HasAccessAsync(string menuKey, ClaimsPrincipal user)
    {
        if (_simulationRole != null)
        {
            return Task.FromResult(CheckRole(menuKey, _simulationRole));
        }
        foreach (var role in _permissions.GetValueOrDefault(menuKey, Array.Empty<string>()))
        {
            if (user.IsInRole(role))
                return Task.FromResult(true);
        }
        return Task.FromResult(!_permissions.ContainsKey(menuKey));
    }

    private bool CheckRole(string menuKey, string role)
    {
        var allowed = _permissions.GetValueOrDefault(menuKey, Array.Empty<string>());
        return allowed.Length == 0 || allowed.Contains(role, StringComparer.OrdinalIgnoreCase);
    }

    public void SetSimulationRole(string? role)
    {
        _simulationRole = role;
    }
}
