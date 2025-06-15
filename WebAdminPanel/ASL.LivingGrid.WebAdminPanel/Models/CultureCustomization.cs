using System.ComponentModel.DataAnnotations;

namespace ASL.LivingGrid.WebAdminPanel.Models;

public class CultureCustomization : BaseEntity
{
    [Required]
    [StringLength(10)]
    public string Culture { get; set; } = string.Empty;

    [Required]
    [StringLength(3)]
    public string TextDirection { get; set; } = "ltr"; // ltr or rtl

    [StringLength(100)]
    public string? FontFamily { get; set; }

    public double FontScale { get; set; } = 1.0;
}
