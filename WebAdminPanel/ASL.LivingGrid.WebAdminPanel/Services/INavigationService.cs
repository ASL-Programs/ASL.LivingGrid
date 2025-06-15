using ASL.LivingGrid.WebAdminPanel.Models;
using System.Security.Claims;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface INavigationService
{
    Task<IEnumerable<NavigationItem>> GetMenuItemsAsync(ClaimsPrincipal? user = null, string? tenantId = null);
}
