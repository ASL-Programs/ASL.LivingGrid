namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IMultiDbWizardService
{
    Task<IEnumerable<string>> GetSupportedTypesAsync();
    Task<bool> ApplyConfigurationAsync(string type, string connectionString);
    Task<IEnumerable<DiscoveredDbInfo>> DiscoverDatabasesAsync(string type);
    Task<bool> TestConnectionAsync(string type, string connectionString);
    Task<DbSchemaInfo> GetSchemaAsync(string type, string connectionString);
    Task<bool> BackupDatabaseAsync(string type, string connectionString, string backupPath);
    Task<bool> RestoreDatabaseAsync(string type, string connectionString, string backupPath);
    Task<EnvironmentCompatibilityResult> CheckEnvironmentCompatibilityAsync(string type, string connectionString);
}

public class DiscoveredDbInfo
{
    public string Name { get; set; } = string.Empty;
    public string Server { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
}

public class DbSchemaInfo
{
    public List<string> Tables { get; set; } = new();
    public Dictionary<string, List<string>> Columns { get; set; } = new();
}

public class EnvironmentCompatibilityResult
{
    public bool IsCompatible { get; set; }
    public string Message { get; set; } = string.Empty;
}
