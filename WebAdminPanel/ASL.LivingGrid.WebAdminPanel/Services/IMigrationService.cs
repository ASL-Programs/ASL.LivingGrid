using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IMigrationService
{
    Task<bool> ExportDataAsync(string filePath, ExportOptions options);
    Task<bool> ImportDataAsync(string filePath, ImportOptions options);
    Task<MigrationStatus> GetMigrationStatusAsync();
    Task<bool> CreateBackupAsync(string backupPath);
    Task<bool> RestoreBackupAsync(string backupPath);
    Task<IEnumerable<string>> GetAvailableBackupsAsync();
    Task<bool> ValidateDataIntegrityAsync();
}

public class ExportOptions
{
    public bool IncludeUsers { get; set; } = true;
    public bool IncludeCompanies { get; set; } = true;
    public bool IncludeConfigurations { get; set; } = true;
    public bool IncludeAuditLogs { get; set; } = false;
    public bool IncludeLocalizations { get; set; } = true;
    public bool IncludeNotifications { get; set; } = false;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string Format { get; set; } = "json"; // json, xml, csv
}

public class ImportOptions
{
    public bool MergeData { get; set; } = false; // false = replace, true = merge
    public bool ValidateBeforeImport { get; set; } = true;
    public bool CreateBackupBeforeImport { get; set; } = true;
    public bool SkipExistingRecords { get; set; } = true;
}

public class MigrationStatus
{
    public bool IsUpToDate { get; set; }
    public string CurrentVersion { get; set; } = string.Empty;
    public string LatestVersion { get; set; } = string.Empty;
    public List<string> PendingMigrations { get; set; } = new();
    public DateTime LastMigrationDate { get; set; }
}
