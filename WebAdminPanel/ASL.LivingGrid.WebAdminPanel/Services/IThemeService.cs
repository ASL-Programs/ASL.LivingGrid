namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IThemeService
{
    Task SetThemeAsync(string theme);
    Task<string> GetCurrentThemeAsync();
    Task<IEnumerable<string>> GetAvailableThemesAsync();
    Task ImportThemeAsync(string name, Stream content);
    Task<Stream> ExportThemeAsync(string name);
}
