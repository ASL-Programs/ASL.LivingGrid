using System.ComponentModel.DataAnnotations;

namespace ASL.LivingGrid.WebAdminPanel.Models;

public class CultureCustomization : BaseEntity
{
    [Required]
    [StringLength(10)]
    public string Culture { get; set; } = string.Empty;

    [Required]
    [StringLength(3)]
    public string TextDirection { get; set; } = "ltr"; // ltr or rtl

    [StringLength(100)]
    public string? FontFamily { get; set; }

    public double FontScale { get; set; } = 1.0;

    public Guid? CompanyId { get; set; }

    public Guid? TenantId { get; set; }

    [StringLength(100)]
    public string Module { get; set; } = "General";

    public virtual Company? Company { get; set; }

    public virtual Tenant? Tenant { get; set; }
}
