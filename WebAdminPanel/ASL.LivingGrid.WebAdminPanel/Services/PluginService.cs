using System.Text.Json;
using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class PluginService : IPluginService
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<PluginService> _logger;
    private readonly List<Plugin> _installed = new();
    private bool _loaded;
    private const string FileName = "plugins.json";

    public PluginService(IWebHostEnvironment env, ILogger<PluginService> logger)
    {
        _env = env;
        _logger = logger;
    }

    private async Task LoadAsync()
    {
        if (_loaded) return;
        var file = Path.Combine(_env.ContentRootPath, FileName);
        if (File.Exists(file))
        {
            try
            {
                var json = await File.ReadAllTextAsync(file);
                var list = JsonSerializer.Deserialize<List<Plugin>>(json);
                if (list != null)
                    _installed.AddRange(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading plugins file");
            }
        }
        _loaded = true;
    }

    private async Task SaveAsync()
    {
        var file = Path.Combine(_env.ContentRootPath, FileName);
        var json = JsonSerializer.Serialize(_installed);
        await File.WriteAllTextAsync(file, json);
    }

    public async Task<IEnumerable<Plugin>> GetInstalledPluginsAsync()
    {
        await LoadAsync();
        return _installed;
    }

    public async Task InstallPluginAsync(Plugin plugin)
    {
        await LoadAsync();
        if (_installed.Any(p => p.Name == plugin.Name && p.Version == plugin.Version))
            return;
        _installed.Add(plugin);
        await SaveAsync();
    }

    public async Task RemovePluginAsync(string id)
    {
        await LoadAsync();
        var p = _installed.FirstOrDefault(x => x.Id == Guid.Parse(id));
        if (p != null)
        {
            _installed.Remove(p);
            await SaveAsync();
        }
    }
}
