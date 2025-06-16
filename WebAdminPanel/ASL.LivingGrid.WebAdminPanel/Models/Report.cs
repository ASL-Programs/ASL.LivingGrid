using System.ComponentModel.DataAnnotations;

namespace ASL.LivingGrid.WebAdminPanel.Models;

public class Report : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public string? Query { get; set; }

    public string? AllowedRoles { get; set; }

    public Guid? CompanyId { get; set; }

    public Guid? TenantId { get; set; }

    public virtual Company? Company { get; set; }

    public virtual Tenant? Tenant { get; set; }
}
