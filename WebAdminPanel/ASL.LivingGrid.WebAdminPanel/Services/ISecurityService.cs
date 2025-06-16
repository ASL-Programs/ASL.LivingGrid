using System;
using System.Threading;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface ISecurityService
{
    Task<bool> ElevatePrivilegesAsync(string userId, TimeSpan duration);
    Task RotateSecretsAsync(CancellationToken token = default);
    Task<bool> IsPasswordExpiredAsync(string userId);
    Task EnforcePasswordPoliciesAsync();
    Task ConfigureExternalIdentityProvidersAsync(AuthenticationBuilder authBuilder, IConfiguration config);
    Task SetTenantPolicyAsync(Guid tenantId, SecurityPolicy policy);
    Task<SecurityPolicy?> GetTenantPolicyAsync(Guid tenantId);
}
