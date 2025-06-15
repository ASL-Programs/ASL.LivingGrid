using System.ComponentModel.DataAnnotations;

namespace ASL.LivingGrid.WebAdminPanel.Models;

public class Notification : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    public string Message { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string Type { get; set; } = "Info"; // Info, Success, Warning, Error
    
    [StringLength(50)]
    public string Priority { get; set; } = "Normal"; // Low, Normal, High, Critical
    
    public string? Recipients { get; set; } // JSON array of user IDs or roles
    
    public DateTime? ScheduledAt { get; set; }
    
    public DateTime? SentAt { get; set; }
    
    public bool IsRead { get; set; } = false;
    
    public Guid? CompanyId { get; set; }
    
    public Guid? TenantId { get; set; }
    
    public string? UserId { get; set; } // Specific user notification
    
    public string? Data { get; set; } // JSON additional data
    
    // Navigation properties
    public virtual Company? Company { get; set; }
    public virtual Tenant? Tenant { get; set; }
}
