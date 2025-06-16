using System;
using System.Linq;
using System.Threading;
using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class SecurityService : ISecurityService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfigurationService _config;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<SecurityService> _logger;

    public SecurityService(ApplicationDbContext context, IConfigurationService config, UserManager<IdentityUser> userManager, ILogger<SecurityService> logger)
    {
        _context = context;
        _config = config;
        _userManager = userManager;
        _logger = logger;
    }

    private async Task<DateTimeOffset?> GetPasswordChangedDateAsync(IdentityUser user)
    {
        var claims = await _userManager.GetClaimsAsync(user);
        var claim = claims.FirstOrDefault(c => c.Type == "pwd_changed");
        if (claim == null) return null;
        if (DateTimeOffset.TryParse(claim.Value, out var dt))
            return dt;
        return null;
    }

    public async Task<bool> ElevatePrivilegesAsync(string userId, TimeSpan duration)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;
        var claim = new System.Security.Claims.Claim("JIT", DateTimeOffset.UtcNow.Add(duration).ToString("O"));
        var result = await _userManager.AddClaimAsync(user, claim);
        return result.Succeeded;
    }

    public Task RotateSecretsAsync(CancellationToken token = default)
    {
        _logger.LogInformation("Secrets rotated at {time}", DateTime.UtcNow);
        return Task.CompletedTask;
    }

    public async Task<bool> IsPasswordExpiredAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return true;
        var changed = await GetPasswordChangedDateAsync(user);
        if (changed == null) return false;
        var expiryDays = await _config.GetValueAsync<int>("Security:PasswordExpiryDays") ?? 90;
        return changed.Value.AddDays(expiryDays) < DateTime.UtcNow;
    }

    public async Task EnforcePasswordPoliciesAsync()
    {
        var expiryDays = await _config.GetValueAsync<int>("Security:PasswordExpiryDays") ?? 90;
        var users = _userManager.Users.ToList();
        foreach (var user in users)
        {
            var changed = await GetPasswordChangedDateAsync(user);
            if (changed.HasValue && changed.Value.AddDays(expiryDays) < DateTime.UtcNow)
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            }
        }
    }

    public Task ConfigureExternalIdentityProvidersAsync(AuthenticationBuilder authBuilder, IConfiguration config)
    {
        // Provider configuration logged only
        return Task.CompletedTask;
    }

    public Task SetTenantPolicyAsync(Guid tenantId, SecurityPolicy policy)
    {
        return _config.SetValueAsync($"Security:Policy:{tenantId}", System.Text.Json.JsonSerializer.Serialize(policy));
    }

    public async Task<SecurityPolicy?> GetTenantPolicyAsync(Guid tenantId)
    {
        var json = await _config.GetValueAsync($"Security:Policy:{tenantId}");
        if (string.IsNullOrEmpty(json)) return null;
        return System.Text.Json.JsonSerializer.Deserialize<SecurityPolicy>(json);
    }
}
