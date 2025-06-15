using System.ComponentModel.DataAnnotations;

namespace ASL.LivingGrid.WebAdminPanel.Models;

public class Configuration : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Key { get; set; } = string.Empty;
    
    public string? Value { get; set; }
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [StringLength(50)]
    public string DataType { get; set; } = "string"; // string, int, bool, json, etc.
    
    [StringLength(50)]
    public string Category { get; set; } = "General";
    
    public bool IsSystem { get; set; } = false;
    
    public bool IsEncrypted { get; set; } = false;
    
    public Guid? CompanyId { get; set; }
    
    public Guid? TenantId { get; set; }
    
    // Navigation properties
    public virtual Company? Company { get; set; }
    public virtual Tenant? Tenant { get; set; }
}
