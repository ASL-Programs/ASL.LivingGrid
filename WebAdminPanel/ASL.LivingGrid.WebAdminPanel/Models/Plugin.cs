using System.ComponentModel.DataAnnotations;

namespace ASL.LivingGrid.WebAdminPanel.Models;

public class Plugin : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Version { get; set; } = "1.0.0";
    
    [StringLength(100)]
    public string? Author { get; set; }
    
    [StringLength(500)]
    public string? FilePath { get; set; }
    
    [StringLength(100)]
    public string? EntryPoint { get; set; }
    
    public bool IsEnabled { get; set; } = false;
    
    public bool IsSystem { get; set; } = false;
    
    public DateTime? InstalledAt { get; set; }
    
    public DateTime? LastUpdatedAt { get; set; }
    
    public string? Configuration { get; set; } // JSON plugin configuration
    
    public string? Dependencies { get; set; } // JSON array of required plugins
    
    public Guid? CompanyId { get; set; }
    
    public Guid? TenantId { get; set; }
    
    // Navigation properties
    public virtual Company? Company { get; set; }
    public virtual Tenant? Tenant { get; set; }
}
