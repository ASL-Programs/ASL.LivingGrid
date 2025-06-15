namespace ASL.LivingGrid.WebAdminPanel.Models;

public class TranslationSuggestionRequest
{
    public string Text { get; set; } = string.Empty;
    public string SourceCulture { get; set; } = "en";
    public string TargetCulture { get; set; } = "az";
}
