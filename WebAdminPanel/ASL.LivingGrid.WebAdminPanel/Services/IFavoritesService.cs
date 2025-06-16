namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IFavoritesService
{
    Task<IList<string>> GetFavoritesAsync();
    Task AddFavoriteAsync(string key);
    Task RemoveFavoriteAsync(string key);
}
