using System.ComponentModel.DataAnnotations;

namespace ASL.LivingGrid.WebAdminPanel.Models;

public class Company : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(10)]
    public string Code { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [StringLength(100)]
    public string? Email { get; set; }
    
    [StringLength(20)]
    public string? Phone { get; set; }
    
    [StringLength(200)]
    public string? Address { get; set; }
    
    [StringLength(100)]
    public string? City { get; set; }
    
    [StringLength(50)]
    public string? Country { get; set; }
    
    [StringLength(20)]
    public string? PostalCode { get; set; }
    
    [StringLength(500)]
    public string? Website { get; set; }
    
    [StringLength(500)]
    public string? LogoUrl { get; set; }
    
    public string? Settings { get; set; } // JSON configuration
    
    // Navigation properties
    public virtual ICollection&lt;Tenant&gt; Tenants { get; set; } = new List&lt;Tenant&gt;();
    public virtual ICollection&lt;AppUser&gt; Users { get; set; } = new List&lt;AppUser&gt;();
}
