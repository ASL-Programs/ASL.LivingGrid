using System.Text.Json;
using ASL.LivingGrid.WebAdminPanel.Models;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Hosting;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class WidgetService : IWidgetService
{
    private readonly IWebHostEnvironment _env;
    private readonly IJSRuntime _js;
    private readonly ILogger<WidgetService> _logger;
    private readonly List<WidgetDefinition> _installed = new();
    private bool _loaded;
    private const string FileName = "widgets.json";

    public WidgetService(IWebHostEnvironment env, IJSRuntime js, ILogger<WidgetService> logger)
    {
        _env = env;
        _js = js;
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
                var list = JsonSerializer.Deserialize<List<WidgetDefinition>>(json);
                if (list != null)
                    _installed.AddRange(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading widgets file");
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

    public async Task<IEnumerable<WidgetDefinition>> GetInstalledWidgetsAsync()
    {
        await LoadAsync();
        return _installed;
    }

    public async Task InstallWidgetAsync(WidgetDefinition widget)
    {
        await LoadAsync();
        if (_installed.Any(w => w.Id == widget.Id)) return;
        _installed.Add(widget);
        await SaveAsync();
    }

    public async Task RemoveWidgetAsync(string id)
    {
        await LoadAsync();
        var found = _installed.FirstOrDefault(w => w.Id == id);
        if (found != null)
        {
            _installed.Remove(found);
            await SaveAsync();
        }
    }

    public async Task<IList<string>> GetUserWidgetsAsync(string companyId, string userId)
    {
        var json = await _js.InvokeAsync<string?>("aslWidgets.getWidgets", companyId, userId);
        if (string.IsNullOrWhiteSpace(json)) return new List<string>();
        try
        {
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    public async Task SaveUserWidgetsAsync(string companyId, string userId, IList<string> widgets)
    {
        var json = JsonSerializer.Serialize(widgets);
        await _js.InvokeVoidAsync("aslWidgets.saveWidgets", companyId, userId, json);
    }

    public Task IncrementUsageAsync(string widgetId)
        => _js.InvokeVoidAsync("aslWidgets.incrementUsage", widgetId).AsTask();

    public Task<int> GetUsageAsync(string widgetId)
        => _js.InvokeAsync<int>("aslWidgets.getUsage", widgetId).AsTask();
}
