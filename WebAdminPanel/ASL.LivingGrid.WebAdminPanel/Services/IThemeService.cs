namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IThemeService
{
    Task SetThemeAsync(string theme);
    Task<string> GetCurrentThemeAsync();
}
