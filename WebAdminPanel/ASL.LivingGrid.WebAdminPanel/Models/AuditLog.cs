using System.ComponentModel.DataAnnotations;

namespace ASL.LivingGrid.WebAdminPanel.Models;

public class AuditLog : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Action { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string EntityType { get; set; } = string.Empty;
    
    public string? EntityId { get; set; }
    
    public string? UserId { get; set; }
    
    [StringLength(100)]
    public string? UserName { get; set; }
    
    [StringLength(50)]
    public string? IPAddress { get; set; }
    
    [StringLength(500)]
    public string? UserAgent { get; set; }
    
    public string? OldValues { get; set; } // JSON
    
    public string? NewValues { get; set; } // JSON
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public Guid? CompanyId { get; set; }
    
    public Guid? TenantId { get; set; }
    
    // Navigation properties
    public virtual Company? Company { get; set; }
    public virtual Tenant? Tenant { get; set; }
}
