using System.Text.Json;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class ModuleCustomizationService : IModuleCustomizationService
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ModuleCustomizationService> _logger;
    private Dictionary<string, Dictionary<string, string>> _overrides = new(); // tenant -> module -> theme

    public ModuleCustomizationService(IWebHostEnvironment env, ILogger<ModuleCustomizationService> logger)
    {
        _env = env;
        _logger = logger;
        Load();
    }

    private void Load()
    {
        var file = Path.Combine(_env.ContentRootPath, "module_theme_overrides.json");
        if (File.Exists(file))
        {
            try
            {
                var json = File.ReadAllText(file);
                _overrides = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load module theme overrides");
            }
        }
    }

    private void Save()
    {
        var file = Path.Combine(_env.ContentRootPath, "module_theme_overrides.json");
        var json = JsonSerializer.Serialize(_overrides, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(file, json);
    }

    public Task SetModuleThemeAsync(string module, string themeId, string? tenantId = null)
    {
        tenantId ??= "default";
        if (!_overrides.ContainsKey(tenantId))
            _overrides[tenantId] = new();
        _overrides[tenantId][module] = themeId;
        Save();
        return Task.CompletedTask;
    }

    public Task<string?> GetModuleThemeAsync(string module, string? tenantId = null)
    {
        tenantId ??= "default";
        if (_overrides.TryGetValue(tenantId, out var map) && map.TryGetValue(module, out var theme))
            return Task.FromResult<string?>(theme);
        return Task.FromResult<string?>(null);
    }
}
