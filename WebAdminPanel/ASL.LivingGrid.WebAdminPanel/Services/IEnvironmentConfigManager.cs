namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IEnvironmentConfigManager
{
    Task<IDictionary<string, string?>> LoadAsync();
    Task SaveAsync(IDictionary<string, string?> values);
}
