using Microsoft.JSInterop;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class ThemeService : IThemeService
{
    private readonly IJSRuntime _js;
    private readonly IWebHostEnvironment _env;
    private readonly string _themesDir;
    private const string StorageKey = "asl-theme";

    public ThemeService(IJSRuntime js, IWebHostEnvironment env)
    {
        _js = js;
        _env = env;
        _themesDir = Path.Combine(_env.WebRootPath, "themes");
        Directory.CreateDirectory(_themesDir);
    }

    public async Task SetThemeAsync(string theme)
    {
        await _js.InvokeVoidAsync("setAslTheme", theme);
    }

    public async Task<string> GetCurrentThemeAsync()
    {
        var theme = await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        return theme ?? "light";
    }

    public Task<IEnumerable<string>> GetAvailableThemesAsync()
    {
        var themes = Directory.Exists(_themesDir)
            ? Directory.GetDirectories(_themesDir).Select(Path.GetFileName)
            : Enumerable.Empty<string>();
        return Task.FromResult(themes);
    }

    public async Task ImportThemeAsync(string name, Stream content)
    {
        var path = Path.Combine(_themesDir, name);
        Directory.CreateDirectory(path);
        var filePath = Path.Combine(path, "theme.css");
        using var fs = File.Create(filePath);
        await content.CopyToAsync(fs);
    }

    public Task<Stream> ExportThemeAsync(string name)
    {
        var filePath = Path.Combine(_themesDir, name, "theme.css");
        Stream stream = File.OpenRead(filePath);
        return Task.FromResult(stream);
    }
}
