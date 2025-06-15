namespace ASL.LivingGrid.WebAdminPanel.Models;

public class MarketplaceLanguagePack
{
    public string Id { get; set; } = string.Empty;
    public string Culture { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;
    public double Rating { get; set; }
    public int RatingsCount { get; set; }
}
