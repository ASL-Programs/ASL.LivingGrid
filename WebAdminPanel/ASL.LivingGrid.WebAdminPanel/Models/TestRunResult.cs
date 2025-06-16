namespace ASL.LivingGrid.WebAdminPanel.Models;

public class TestRunResult
{
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public bool Success { get; set; }
    public string Log { get; set; } = string.Empty;
}
