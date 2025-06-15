using System.ComponentModel.DataAnnotations;

namespace ASL.LivingGrid.WebAdminPanel.Models;

public class LocalizationResourceVersion : BaseEntity
{
    public Guid ResourceId { get; set; }

    [Required]
    public string Value { get; set; } = string.Empty;

    public int Version { get; set; }

    [StringLength(100)]
    public string? CreatedByUser { get; set; }

    public DateTime? ApprovedAt { get; set; }

    [StringLength(100)]
    public string? ApprovedBy { get; set; }

    // Navigation
    public virtual LocalizationResource? Resource { get; set; }
}
