using System.ComponentModel.DataAnnotations;

namespace ASL.LivingGrid.WebAdminPanel.Models;

public class TemplateOverride : BaseEntity
{
    [Required]
    [StringLength(10)]
    public string Culture { get; set; } = string.Empty;

    [StringLength(100)]
    public string Module { get; set; } = "General";

    [Required]
    [StringLength(100)]
    public string TemplateKey { get; set; } = string.Empty;

    public string TemplateContent { get; set; } = string.Empty;

    public Guid? CompanyId { get; set; }
    public Guid? TenantId { get; set; }

    public virtual Company? Company { get; set; }
    public virtual Tenant? Tenant { get; set; }
}
