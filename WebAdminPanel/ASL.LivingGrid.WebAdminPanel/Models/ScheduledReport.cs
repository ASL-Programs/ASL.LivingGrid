using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace ASL.LivingGrid.WebAdminPanel.Models;

public class ScheduledReport : BaseEntity
{
    [Required]
    public Guid ReportId { get; set; }

    public string? FilterJson { get; set; }

    public string? Recipients { get; set; }

    public DateTime ScheduledAt { get; set; }

    public DateTime? SentAt { get; set; }

    public virtual Report? Report { get; set; }
}
