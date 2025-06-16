using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IPluginService
{
    Task<IEnumerable<Plugin>> GetInstalledPluginsAsync();
    Task InstallPluginAsync(Plugin plugin);
    Task RemovePluginAsync(string id);
}
