using ASL.LivingGrid.WebAdminPanel.Data;
using System.Data.SqlClient;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class MultiDbWizardService : IMultiDbWizardService
{
    private readonly IConfigurationService _config;
    private readonly ILogger<MultiDbWizardService> _logger;

    public MultiDbWizardService(IConfigurationService config, ILogger<MultiDbWizardService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public Task<IEnumerable<string>> GetSupportedTypesAsync()
    {
        IEnumerable<string> types = new[] { "SQLServer", "PostgreSQL", "SQLite" };
        return Task.FromResult(types);
    }

    public async Task<bool> ApplyConfigurationAsync(string type, string connectionString)
    {
        try
        {
            await _config.SetValueAsync("Database:Type", type);
            await _config.SetValueAsync("ConnectionStrings:DefaultConnection", connectionString);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply DB config");
            return false;
        }
    }

    public async Task<IEnumerable<DiscoveredDbInfo>> DiscoverDatabasesAsync(string type)
    {
        var result = new List<DiscoveredDbInfo>();
        if (type == "SQLServer")
        {
            // Sadə nümunə: local SQL Server instance-ları üçün (realda network scan və discovery üçün əlavə paketlər lazımdır)
            try
            {
                // Demo məqsədli statik nəticə
                result.Add(new DiscoveredDbInfo { Name = "master", Server = ".", ConnectionString = "Server=.;Database=master;Trusted_Connection=True;" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DB discovery failed");
            }
        }
        // Digər DB tipləri üçün də əlavə oluna bilər
        return result;
    }

    public async Task<bool> TestConnectionAsync(string type, string connectionString)
    {
        try
        {
            if (type == "SQLServer")
            {
                using var conn = new SqlConnection(connectionString);
                await conn.OpenAsync();
                return conn.State == System.Data.ConnectionState.Open;
            }
            // Digər DB tipləri üçün də əlavə oluna bilər
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Test connection failed");
            return false;
        }
    }

    public async Task<DbSchemaInfo> GetSchemaAsync(string type, string connectionString)
    {
        var schema = new DbSchemaInfo();
        if (type == "SQLServer")
        {
            try
            {
                using var conn = new SqlConnection(connectionString);
                await conn.OpenAsync();
                var tables = new List<string>();
                var columns = new Dictionary<string, List<string>>();
                using var cmd = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'", conn);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    tables.Add(reader.GetString(0));
                }
                reader.Close();
                foreach (var table in tables)
                {
                    var colCmd = new SqlCommand($"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME=@table", conn);
                    colCmd.Parameters.AddWithValue("@table", table);
                    var colReader = await colCmd.ExecuteReaderAsync();
                    var cols = new List<string>();
                    while (await colReader.ReadAsync())
                    {
                        cols.Add(colReader.GetString(0));
                    }
                    columns[table] = cols;
                    colReader.Close();
                }
                schema.Tables = tables;
                schema.Columns = columns;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSchema failed");
            }
        }
        return schema;
    }

    public async Task<bool> BackupDatabaseAsync(string type, string connectionString, string backupPath)
    {
        if (type == "SQLServer")
        {
            try
            {
                using var conn = new SqlConnection(connectionString);
                await conn.OpenAsync();
                var dbName = conn.Database;
                var cmd = new SqlCommand($"BACKUP DATABASE [{dbName}] TO DISK=@backupPath WITH INIT", conn);
                cmd.Parameters.AddWithValue("@backupPath", backupPath);
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Backup failed");
                return false;
            }
        }
        return false;
    }

    public async Task<bool> RestoreDatabaseAsync(string type, string connectionString, string backupPath)
    {
        if (type == "SQLServer")
        {
            try
            {
                using var conn = new SqlConnection(connectionString);
                await conn.OpenAsync();
                var dbName = conn.Database;
                var cmd = new SqlCommand($"RESTORE DATABASE [{dbName}] FROM DISK=@backupPath WITH REPLACE", conn);
                cmd.Parameters.AddWithValue("@backupPath", backupPath);
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Restore failed");
                return false;
            }
        }
        return false;
    }

    public async Task<EnvironmentCompatibilityResult> CheckEnvironmentCompatibilityAsync(string type, string connectionString)
    {
        var result = new EnvironmentCompatibilityResult();
        if (type == "SQLServer")
        {
            try
            {
                using var conn = new SqlConnection(connectionString);
                await conn.OpenAsync();
                // Sadə yoxlama: SQL Server versiyası və əsas parametrlər
                var cmd = new SqlCommand("SELECT SERVERPROPERTY('ProductVersion')", conn);
                var version = (string?)await cmd.ExecuteScalarAsync();
                result.IsCompatible = !string.IsNullOrEmpty(version) && version.StartsWith("15."); // SQL Server 2019 nümunəsi
                result.Message = result.IsCompatible ? $"Compatible: SQL Server {version}" : $"Incompatible SQL Server version: {version}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Env compatibility check failed");
                result.IsCompatible = false;
                result.Message = ex.Message;
            }
        }
        return result;
    }
}
