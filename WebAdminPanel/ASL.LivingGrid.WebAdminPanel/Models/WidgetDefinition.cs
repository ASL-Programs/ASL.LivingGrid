namespace ASL.LivingGrid.WebAdminPanel.Models;

public class WidgetDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Component { get; set; } = string.Empty;
    public List<string> Dependencies { get; set; } = new();
    public string? PluginAssembly { get; set; }
    public bool IsRealTime { get; set; }
}
