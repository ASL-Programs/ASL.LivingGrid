using System;
namespace ASL.LivingGrid.WebAdminPanel.Models;

public class SecurityPolicy
{
    public Guid TenantId { get; set; }
    public bool RequireMfa { get; set; }
    public int PasswordExpiryDays { get; set; } = 90;
    public bool EnableJit { get; set; }
}
