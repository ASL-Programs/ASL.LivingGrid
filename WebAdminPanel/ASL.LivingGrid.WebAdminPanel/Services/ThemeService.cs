using Microsoft.JSInterop;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class ThemeService : IThemeService
{
    private readonly IJSRuntime _js;
    private const string StorageKey = "asl-theme";

    public ThemeService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task SetThemeAsync(string theme)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, theme);
        await _js.InvokeVoidAsync("document.body.setAttribute", "data-theme", theme);
    }

    public async Task<string> GetCurrentThemeAsync()
    {
        var theme = await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        return theme ?? "light";
    }
}
