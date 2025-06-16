namespace ASL.LivingGrid.WebAdminPanel.Models;

public class MarketplacePlugin
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
    public string Description { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;
    public string? PreviewImage { get; set; }
}
