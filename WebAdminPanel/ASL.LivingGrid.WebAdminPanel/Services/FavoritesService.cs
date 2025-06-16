using Microsoft.JSInterop;
using System.Text.Json;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class FavoritesService : IFavoritesService
{
    private readonly IJSRuntime _js;
    private const string StorageKey = "asl-favorites";

    public FavoritesService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<IList<string>> GetFavoritesAsync()
    {
        var json = await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        if (string.IsNullOrWhiteSpace(json))
            return new List<string>();
        try
        {
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    public async Task AddFavoriteAsync(string key)
    {
        var list = (await GetFavoritesAsync()).ToList();
        if (!list.Contains(key))
        {
            list.Add(key);
            await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, JsonSerializer.Serialize(list));
        }
    }

    public async Task RemoveFavoriteAsync(string key)
    {
        var list = (await GetFavoritesAsync()).Where(k => k != key).ToList();
        await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, JsonSerializer.Serialize(list));
    }
}
