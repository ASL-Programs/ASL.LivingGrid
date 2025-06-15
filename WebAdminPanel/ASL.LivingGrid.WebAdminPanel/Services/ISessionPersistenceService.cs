namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface ISessionPersistenceService
{
    Task SaveAsync(string key, string data);
    Task<string?> LoadAsync(string key);
}
