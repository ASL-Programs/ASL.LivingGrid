using System.ComponentModel.DataAnnotations;

namespace ASL.LivingGrid.WebAdminPanel.Models;

public class Device : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string? Type { get; set; } // Mobile, Desktop, Tablet, IoT, etc.
    
    [StringLength(100)]
    public string? Brand { get; set; }
    
    [StringLength(100)]
    public string? Model { get; set; }
    
    [StringLength(500)]
    public string? DeviceId { get; set; } // Unique device identifier
    
    [StringLength(50)]
    public string? IPAddress { get; set; }
    
    [StringLength(500)]
    public string? UserAgent { get; set; }
    
    [StringLength(100)]
    public string? OperatingSystem { get; set; }
    
    [StringLength(50)]
    public string? Version { get; set; }
    
    public DateTime? LastSeenAt { get; set; }
    
    public bool IsOnline { get; set; } = false;
    
    public bool IsBlocked { get; set; } = false;
    
    public Guid? CompanyId { get; set; }
    
    public Guid? TenantId { get; set; }
    
    public string? UserId { get; set; }
    
    public string? Settings { get; set; } // JSON device-specific settings
    
    // Navigation properties
    public virtual Company? Company { get; set; }
    public virtual Tenant? Tenant { get; set; }
}
