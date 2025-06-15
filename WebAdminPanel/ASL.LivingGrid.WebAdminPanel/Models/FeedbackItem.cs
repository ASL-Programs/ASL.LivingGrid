namespace ASL.LivingGrid.WebAdminPanel.Models;

public record FeedbackItem
{
    public string Page { get; init; } = string.Empty;
    public int Rating { get; init; }
    public string Comments { get; init; } = string.Empty;
}
