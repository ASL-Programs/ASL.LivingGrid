using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface INavigationService
{
    Task<IEnumerable<NavigationItem>> GetMenuItemsAsync();
}
