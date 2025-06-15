using System.ComponentModel.DataAnnotations;

namespace ASL.LivingGrid.WebAdminPanel.Models;

public class Documentation : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Content { get; set; }

    [StringLength(100)]
    public string? Category { get; set; }
}
