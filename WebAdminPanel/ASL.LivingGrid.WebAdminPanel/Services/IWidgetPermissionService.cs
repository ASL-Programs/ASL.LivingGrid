using System.Security.Claims;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IWidgetPermissionService
{
    bool HasAccess(string widgetId, ClaimsPrincipal user, string? tenantId = null, string? module = null);
}
