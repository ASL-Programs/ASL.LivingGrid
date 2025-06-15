using System.ComponentModel.DataAnnotations;

namespace ASL.LivingGrid.WebAdminPanel.Models;

public class Tenant : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    public Guid CompanyId { get; set; }
    
    public string? Settings { get; set; } // JSON configuration
    
    // Navigation properties
    public virtual Company Company { get; set; } = null!;
}
