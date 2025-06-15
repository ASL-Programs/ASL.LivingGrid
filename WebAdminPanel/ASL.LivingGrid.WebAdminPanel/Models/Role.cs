using System.ComponentModel.DataAnnotations;

namespace ASL.LivingGrid.WebAdminPanel.Models;

public class Role : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    public Guid? CompanyId { get; set; }
    
    public Guid? TenantId { get; set; }
    
    public string? Permissions { get; set; } // JSON array of permission IDs
    
    // Navigation properties
    public virtual Company? Company { get; set; }
    public virtual Tenant? Tenant { get; set; }
}

public class Permission : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Code { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [StringLength(50)]
    public string Module { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string Category { get; set; } = string.Empty;
}
