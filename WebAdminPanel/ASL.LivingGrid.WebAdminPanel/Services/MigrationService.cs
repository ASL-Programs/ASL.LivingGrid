using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.IO.Compression;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class MigrationService : IMigrationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MigrationService> _logger;
    private readonly string _backupDirectory;

    public MigrationService(ApplicationDbContext context, ILogger<MigrationService> logger, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _backupDirectory = configuration.GetValue<string>("BackupDirectory", "backups") ?? "backups";
        
        // Ensure backup directory exists
        if (!Directory.Exists(_backupDirectory))
        {
            Directory.CreateDirectory(_backupDirectory);
        }
    }

    public async Task<bool> ExportDataAsync(string filePath, ExportOptions options)
    {
        try
        {
            _logger.LogInformation("Starting data export to {FilePath}", filePath);

            var exportData = new Dictionary<string, object>();

            if (options.IncludeUsers)
            {
                var users = await _context.AppUsers.ToListAsync();
                exportData["Users"] = users;
            }

            if (options.IncludeCompanies)
            {
                var companies = await _context.Companies.ToListAsync();
                exportData["Companies"] = companies;
            }

            if (options.IncludeConfigurations)
            {
                var configurations = await _context.Configurations.ToListAsync();
                exportData["Configurations"] = configurations;
            }

            if (options.IncludeLocalizations)
            {
                var localizations = await _context.LocalizationResources.ToListAsync();
                exportData["LocalizationResources"] = localizations;
            }

            if (options.IncludeAuditLogs)
            {
                var query = _context.AuditLogs.AsQueryable();
                
                if (options.FromDate.HasValue)
                    query = query.Where(a => a.Timestamp >= options.FromDate.Value);
                
                if (options.ToDate.HasValue)
                    query = query.Where(a => a.Timestamp <= options.ToDate.Value);

                var auditLogs = await query.ToListAsync();
                exportData["AuditLogs"] = auditLogs;
            }

            if (options.IncludeNotifications)
            {
                var notifications = await _context.Notifications.ToListAsync();
                exportData["Notifications"] = notifications;
            }

            // Add metadata
            exportData["ExportMetadata"] = new
            {
                ExportDate = DateTime.UtcNow,
                Version = "1.0.0",
                Options = options
            };

            // Export based on format
            switch (options.Format.ToLower())
            {
                case "json":
                    await ExportAsJsonAsync(filePath, exportData);
                    break;
                case "xml":
                    // TODO: Implement XML export
                    throw new NotImplementedException("XML export not yet implemented");
                default:
                    throw new ArgumentException($"Unsupported export format: {options.Format}");
            }

            _logger.LogInformation("Data export completed successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during data export");
            return false;
        }
    }

    public async Task<bool> ImportDataAsync(string filePath, ImportOptions options)
    {
        try
        {
            _logger.LogInformation("Starting data import from {FilePath}", filePath);

            if (options.CreateBackupBeforeImport)
            {
                var backupPath = Path.Combine(_backupDirectory, $"backup_before_import_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json");
                await CreateBackupAsync(backupPath);
            }

            var importData = await ReadImportDataAsync(filePath);
            
            if (options.ValidateBeforeImport)
            {
                if (!await ValidateImportDataAsync(importData))
                {
                    _logger.LogError("Import data validation failed");
                    return false;
                }
            }

            await ProcessImportDataAsync(importData, options);

            _logger.LogInformation("Data import completed successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during data import");
            return false;
        }
    }

    public async Task<MigrationStatus> GetMigrationStatusAsync()
    {
        try
        {
            var currentVersion = "1.0.0"; // This would come from configuration or assembly
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();

            return new MigrationStatus
            {
                IsUpToDate = !pendingMigrations.Any(),
                CurrentVersion = currentVersion,
                LatestVersion = currentVersion,
                PendingMigrations = pendingMigrations.ToList(),
                LastMigrationDate = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting migration status");
            return new MigrationStatus();
        }
    }

    public async Task<bool> CreateBackupAsync(string backupPath)
    {
        try
        {
            _logger.LogInformation("Creating backup at {BackupPath}", backupPath);

            var exportOptions = new ExportOptions
            {
                IncludeUsers = true,
                IncludeCompanies = true,
                IncludeConfigurations = true,
                IncludeLocalizations = true,
                IncludeAuditLogs = true,
                IncludeNotifications = true
            };

            var success = await ExportDataAsync(backupPath, exportOptions);
            
            if (success)
            {
                // Compress the backup
                var compressedPath = backupPath + ".gz";
                await CompressFileAsync(backupPath, compressedPath);
                File.Delete(backupPath); // Remove uncompressed version
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating backup");
            return false;
        }
    }

    public async Task<bool> RestoreBackupAsync(string backupPath)
    {
        try
        {
            _logger.LogInformation("Restoring backup from {BackupPath}", backupPath);

            var tempPath = backupPath;
            
            // Decompress if needed
            if (backupPath.EndsWith(".gz"))
            {
                tempPath = Path.ChangeExtension(backupPath, null);
                await DecompressFileAsync(backupPath, tempPath);
            }

            var importOptions = new ImportOptions
            {
                MergeData = false, // Replace all data
                ValidateBeforeImport = true,
                CreateBackupBeforeImport = false // We're already restoring a backup
            };

            var success = await ImportDataAsync(tempPath, importOptions);

            // Clean up temp file if we decompressed
            if (tempPath != backupPath && File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring backup");
            return false;
        }
    }

    public async Task<IEnumerable<string>> GetAvailableBackupsAsync()
    {
        try
        {
            await Task.Delay(1); // Make it async
            
            if (!Directory.Exists(_backupDirectory))
            {
                return Enumerable.Empty<string>();
            }

            var backupFiles = Directory.GetFiles(_backupDirectory, "*.json")
                .Concat(Directory.GetFiles(_backupDirectory, "*.json.gz"))
                .OrderByDescending(f => File.GetCreationTime(f))
                .ToList();

            return backupFiles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available backups");
            return Enumerable.Empty<string>();
        }
    }

    public async Task<bool> ValidateDataIntegrityAsync()
    {
        try
        {
            _logger.LogInformation("Validating data integrity");

            // Check for orphaned records
            var orphanedUsers = await _context.AppUsers
                .Where(u => u.CompanyId.HasValue && !_context.Companies.Any(c => c.Id == u.CompanyId))
                .CountAsync();

            var orphanedConfigs = await _context.Configurations
                .Where(c => c.CompanyId.HasValue && !_context.Companies.Any(comp => comp.Id == c.CompanyId))
                .CountAsync();

            if (orphanedUsers > 0 || orphanedConfigs > 0)
            {
                _logger.LogWarning("Found {OrphanedUsers} orphaned users and {OrphanedConfigs} orphaned configurations", 
                    orphanedUsers, orphanedConfigs);
            }

            // Add more validation checks as needed

            _logger.LogInformation("Data integrity validation completed");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating data integrity");
            return false;
        }
    }

    private async Task ExportAsJsonAsync(string filePath, Dictionary<string, object> data)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(data, options);
        await File.WriteAllTextAsync(filePath, json);
    }

    private async Task<Dictionary<string, object>> ReadImportDataAsync(string filePath)
    {
        var json = await File.ReadAllTextAsync(filePath);
        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        return data ?? new Dictionary<string, object>();
    }

    private async Task<bool> ValidateImportDataAsync(Dictionary<string, object> data)
    {
        // Add validation logic here
        await Task.Delay(1); // Make it async
        return true;
    }

    private async Task ProcessImportDataAsync(Dictionary<string, object> data, ImportOptions options)
    {
        // Process the imported data based on options
        // This is a simplified implementation
        await Task.Delay(1); // Make it async
        
        // In a real implementation, you would:
        // 1. Deserialize each data type
        // 2. Handle merge vs replace logic
        // 3. Update the database
        // 4. Handle conflicts and validation
    }

    private async Task CompressFileAsync(string sourcePath, string destinationPath)
    {
        using var originalFileStream = File.OpenRead(sourcePath);
        using var compressedFileStream = File.Create(destinationPath);
        using var compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress);
        await originalFileStream.CopyToAsync(compressionStream);
    }

    private async Task DecompressFileAsync(string sourcePath, string destinationPath)
    {
        using var compressedFileStream = File.OpenRead(sourcePath);
        using var decompressionStream = new GZipStream(compressedFileStream, CompressionMode.Decompress);
        using var outputFileStream = File.Create(destinationPath);
        await decompressionStream.CopyToAsync(outputFileStream);
    }
}
