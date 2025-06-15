namespace ASL.LivingGrid.WebAdminPanel.Models;

public class TranslationProvider
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string? ApiKey { get; set; }
    public string? WebhookUrl { get; set; }
}
