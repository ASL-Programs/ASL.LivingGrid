using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ASL.LivingGrid.WebAdminPanel.Models;

public class AppUser : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [StringLength(20)]
    public string? Phone { get; set; }
    
    public Guid? CompanyId { get; set; }
    
    public Guid? TenantId { get; set; }
    
    [StringLength(10)]
    public string PreferredLanguage { get; set; } = "az";
    
    [StringLength(20)]
    public string PreferredTheme { get; set; } = "light";
    
    public DateTime? LastLoginAt { get; set; }
    
    public string? Settings { get; set; } // JSON user preferences
    
    // Navigation properties
    public virtual Company? Company { get; set; }
    public virtual Tenant? Tenant { get; set; }
}
