using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IWidgetService
{
    Task<IEnumerable<WidgetDefinition>> GetInstalledWidgetsAsync();
    Task InstallWidgetAsync(WidgetDefinition widget);
    Task RemoveWidgetAsync(string id);
    Task<IList<string>> GetUserWidgetsAsync(string companyId, string userId);
    Task SaveUserWidgetsAsync(string companyId, string userId, IList<string> widgets);
    Task IncrementUsageAsync(string widgetId);
    Task<int> GetUsageAsync(string widgetId);
    Task LoadPluginWidgetsAsync(string pluginFolder);
    Task<IEnumerable<string>> GetMissingDependenciesAsync(string widgetId);
}
