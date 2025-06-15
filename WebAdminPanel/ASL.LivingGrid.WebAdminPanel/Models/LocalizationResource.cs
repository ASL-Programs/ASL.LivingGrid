using System.ComponentModel.DataAnnotations;

namespace ASL.LivingGrid.WebAdminPanel.Models;

public class LocalizationResource : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Key { get; set; } = string.Empty;
    
    [Required]
    public string Value { get; set; } = string.Empty;
    
    [Required]
    [StringLength(10)]
    public string Culture { get; set; } = string.Empty; // az, en, tr, ru
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [StringLength(50)]
    public string Category { get; set; } = "General";
    
    public Guid? CompanyId { get; set; }
    
    public Guid? TenantId { get; set; }
    
    // Navigation properties
    public virtual Company? Company { get; set; }
    public virtual Tenant? Tenant { get; set; }
}
