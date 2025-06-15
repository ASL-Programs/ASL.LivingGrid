using System.ComponentModel.DataAnnotations;

namespace ASL.LivingGrid.WebAdminPanel.Models;

public class TranslationProject : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(10)]
    public string DefaultLanguage { get; set; } = "en";

    public string SupportedLanguages { get; set; } = "az,en,tr,ru";

    public ProjectStatus Status { get; set; } = ProjectStatus.Active;

    public virtual ICollection<TranslationKey> Keys { get; set; } = new List<TranslationKey>();
}

public enum ProjectStatus
{
    Active,
    Inactive,
    Archived,
    Draft
}
