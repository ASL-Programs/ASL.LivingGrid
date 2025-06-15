using System.Security.Claims;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IRoleBasedUiService
{
    Task<bool> HasAccessAsync(string menuKey, ClaimsPrincipal user);
    void SetSimulationRole(string? role);
}
