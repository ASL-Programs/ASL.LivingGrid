using System.ComponentModel.DataAnnotations;

namespace ASL.LivingGrid.WebAdminPanel.Models;

public class TranslationRequest : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Key { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string Culture { get; set; } = string.Empty;

    public string? ProposedValue { get; set; }

    [StringLength(100)]
    public string RequestedBy { get; set; } = string.Empty;

    public TranslationRequestStatus Status { get; set; } = TranslationRequestStatus.PendingReview;

    [StringLength(100)]
    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }
}

public enum TranslationRequestStatus
{
    Machine,
    Human,
    PendingReview,
    Approved
}
