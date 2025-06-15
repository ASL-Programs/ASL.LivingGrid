using System.ComponentModel.DataAnnotations;

namespace ASL.LivingGrid.WebAdminPanel.Models;

public class TranslationKey : BaseEntity
{
    public Guid ProjectId { get; set; }

    [Required]
    [StringLength(200)]
    public string Key { get; set; } = string.Empty;

    [StringLength(100)]
    public string Category { get; set; } = "General";

    [StringLength(500)]
    public string? Description { get; set; }

    public TranslationStatus Status { get; set; } = TranslationStatus.Draft;

    // Navigation
    public virtual TranslationProject? Project { get; set; }
    public virtual ICollection<LocalizationResource> Resources { get; set; } = new List<LocalizationResource>();
}
