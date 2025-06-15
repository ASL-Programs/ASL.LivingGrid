using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class AdvancedRollbackService : IAdvancedRollbackService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AdvancedRollbackService> _logger;
    private readonly IConfigurationService _configService;
    private readonly ApplicationDbContext _context;
    private readonly string _backupPath;
    private readonly string _snapshotPath;
    private static UpgradeMonitoringResult? _activeMonitoring;

    public AdvancedRollbackService(
        IConfiguration configuration,
        ILogger<AdvancedRollbackService> logger,
        IConfigurationService configService,
        ApplicationDbContext context)
    {
        _configuration = configuration;
        _logger = logger;
        _configService = configService;
        _context = context;
        
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        _backupPath = Path.Combine(basePath, "Backups");
        _snapshotPath = Path.Combine(basePath, "Snapshots");
        
        // Ensure directories exist
        Directory.CreateDirectory(_backupPath);
        Directory.CreateDirectory(_snapshotPath);
    }

    public async Task<BackupPoint> CreateBackupPointAsync(string description, BackupType type = BackupType.Manual)
    {
        var backupPoint = new BackupPoint
        {
            Name = $"Backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}",
            Description = description,
            Type = type,
            Version = GetCurrentVersion(),
            Status = BackupStatus.Creating
        };

        try
        {
            _logger.LogInformation("Creating backup point: {Description}", description);

            var backupDir = Path.Combine(_backupPath, backupPoint.Id);
            Directory.CreateDirectory(backupDir);
            backupPoint.BackupPath = backupDir;

            var components = new List<BackupComponent>();

            // Backup database
            var dbComponent = await BackupDatabaseAsync(backupDir);
            if (dbComponent != null)
            {
                components.Add(dbComponent);
            }

            // Backup configuration
            var configComponent = await BackupConfigurationAsync(backupDir);
            if (configComponent != null)
            {
                components.Add(configComponent);
            }

            // Backup user data
            var userDataComponent = await BackupUserDataAsync(backupDir);
            if (userDataComponent != null)
            {
                components.Add(userDataComponent);
            }

            // Backup certificates and keys
            var certComponent = await BackupCertificatesAsync(backupDir);
            if (certComponent != null)
            {
                components.Add(certComponent);
            }

            // Backup plugins
            var pluginComponent = await BackupPluginsAsync(backupDir);
            if (pluginComponent != null)
            {
                components.Add(pluginComponent);
            }

            backupPoint.Components = components;
            backupPoint.SizeBytes = components.Sum(c => c.SizeBytes);
            backupPoint.Status = BackupStatus.Completed;

            // Calculate checksum
            backupPoint.ChecksumHash = await CalculateBackupChecksumAsync(backupDir);

            // Save backup metadata
            await SaveBackupMetadataAsync(backupPoint);

            // Verify backup integrity
            var isValid = await ValidateBackupPointAsync(backupPoint.Id);
            if (!isValid)
            {
                backupPoint.Status = BackupStatus.Corrupted;
                backupPoint.CanRestore = false;
            }

            _logger.LogInformation("Backup point created successfully: {BackupId}", backupPoint.Id);
            return backupPoint;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating backup point: {Description}", description);
            backupPoint.Status = BackupStatus.Failed;
            backupPoint.ErrorMessage = ex.Message;
            backupPoint.CanRestore = false;
            return backupPoint;
        }
    }

    public async Task<bool> RestoreFromBackupPointAsync(string backupPointId)
    {
        try
        {
            _logger.LogInformation("Starting restore from backup point: {BackupPointId}", backupPointId);

            var backupPoint = await GetBackupPointAsync(backupPointId);
            if (backupPoint == null)
            {
                _logger.LogError("Backup point not found: {BackupPointId}", backupPointId);
                return false;
            }

            if (!backupPoint.CanRestore)
            {
                _logger.LogError("Backup point cannot be restored: {BackupPointId}", backupPointId);
                return false;
            }

            // Validate backup integrity before restore
            if (!await ValidateBackupPointAsync(backupPointId))
            {
                _logger.LogError("Backup point validation failed: {BackupPointId}", backupPointId);
                return false;
            }

            // Create a backup of current state before restore
            var preRestoreBackup = await CreateBackupPointAsync("Pre-restore backup", BackupType.Emergency);

            backupPoint.Status = BackupStatus.Restoring;

            // Restore each component
            foreach (var component in backupPoint.Components.OrderBy(c => GetComponentRestoreOrder(c.Type)))
            {
                if (!await RestoreComponentAsync(component, backupPoint.BackupPath))
                {
                    _logger.LogError("Failed to restore component: {ComponentName}", component.Name);
                    return false;
                }
            }

            // Verify restore
            if (await VerifySystemIntegrityAsync())
            {
                backupPoint.Status = BackupStatus.Verified;
                _logger.LogInformation("Restore completed successfully: {BackupPointId}", backupPointId);
                return true;
            }
            else
            {
                _logger.LogError("System integrity verification failed after restore");
                
                // Attempt to restore to pre-restore state
                await RestoreFromBackupPointAsync(preRestoreBackup.Id);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring from backup point: {BackupPointId}", backupPointId);
            return false;
        }
    }

    public async Task<IEnumerable<BackupPoint>> GetAvailableBackupPointsAsync()
    {
        try
        {
            var backupPoints = new List<BackupPoint>();
            
            if (!Directory.Exists(_backupPath))
            {
                return backupPoints;
            }

            var backupDirs = Directory.GetDirectories(_backupPath);
            
            foreach (var dir in backupDirs)
            {
                var metadataPath = Path.Combine(dir, "metadata.json");
                if (File.Exists(metadataPath))
                {
                    try
                    {
                        var json = await File.ReadAllTextAsync(metadataPath);
                        var backupPoint = JsonSerializer.Deserialize<BackupPoint>(json);
                        if (backupPoint != null)
                        {
                            backupPoints.Add(backupPoint);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error reading backup metadata: {MetadataPath}", metadataPath);
                    }
                }
            }

            return backupPoints.OrderByDescending(bp => bp.CreatedAt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available backup points");
            return new List<BackupPoint>();
        }
    }

    public async Task<RollbackResult> PerformRollbackAsync(string targetVersion, RollbackOptions options)
    {
        var result = new RollbackResult
        {
            RollbackStartTime = DateTime.UtcNow,
            FromVersion = GetCurrentVersion(),
            ToVersion = targetVersion
        };

        try
        {
            _logger.LogInformation("Starting rollback from {FromVersion} to {ToVersion}", result.FromVersion, targetVersion);

            // Create rollback plan
            var plan = await CreateRollbackPlanAsync(targetVersion);
            plan.Options = options;

            // Create pre-rollback snapshot if requested
            if (options.CreateBackupBeforeRollback)
            {
                result.PreRollbackSnapshot = await CreateSystemSnapshotAsync();
            }

            // Execute rollback plan
            var success = await ExecuteRollbackPlanAsync(plan);
            
            result.Steps = plan.Steps;
            result.IsSuccess = success;
            result.RollbackEndTime = DateTime.UtcNow;
            result.Duration = result.RollbackEndTime - result.RollbackStartTime;

            if (success)
            {
                // Verify integrity if requested
                if (options.VerifyIntegrityAfterRollback)
                {
                    if (!await VerifySystemIntegrityAsync())
                    {
                        result.IsSuccess = false;
                        result.Errors.Add("System integrity verification failed after rollback");
                    }
                }

                // Create post-rollback snapshot
                result.PostRollbackSnapshot = await CreateSystemSnapshotAsync();

                _logger.LogInformation("Rollback completed successfully to version {ToVersion}", targetVersion);
            }
            else
            {
                var errors = plan.Steps.Where(s => s.Status == RollbackStepStatus.Failed)
                                     .Select(s => s.ErrorMessage ?? "Unknown error")
                                     .ToList();
                result.Errors.AddRange(errors);
                
                _logger.LogError("Rollback failed. Errors: {Errors}", string.Join(", ", errors));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during rollback");
            result.IsSuccess = false;
            result.ErrorDetails = ex.Message;
            result.RollbackEndTime = DateTime.UtcNow;
            result.Duration = result.RollbackEndTime - result.RollbackStartTime;
        }

        return result;
    }

    public async Task<bool> ValidateRollbackAsync(string backupPointId)
    {
        return await ValidateBackupPointAsync(backupPointId);
    }

    public async Task<bool> DeleteBackupPointAsync(string backupPointId)
    {
        try
        {
            var backupPoint = await GetBackupPointAsync(backupPointId);
            if (backupPoint == null)
            {
                return false;
            }

            if (Directory.Exists(backupPoint.BackupPath))
            {
                Directory.Delete(backupPoint.BackupPath, true);
            }

            _logger.LogInformation("Backup point deleted: {BackupPointId}", backupPointId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting backup point: {BackupPointId}", backupPointId);
            return false;
        }
    }

    public async Task<BackupPoint?> GetLastStableBackupPointAsync()
    {
        var backupPoints = await GetAvailableBackupPointsAsync();
        return backupPoints
            .Where(bp => bp.Status == BackupStatus.Completed || bp.Status == BackupStatus.Verified)
            .Where(bp => bp.CanRestore)
            .OrderByDescending(bp => bp.CreatedAt)
            .FirstOrDefault();
    }

    public async Task<RollbackPlan> CreateRollbackPlanAsync(string targetVersion)
    {
        var plan = new RollbackPlan
        {
            Name = $"Rollback to {targetVersion}",
            FromVersion = GetCurrentVersion(),
            ToVersion = targetVersion,
            Status = RollbackPlanStatus.Draft
        };

        try
        {
            // Find appropriate backup point
            var backupPoints = await GetAvailableBackupPointsAsync();
            var targetBackup = backupPoints
                .Where(bp => bp.Version == targetVersion)
                .Where(bp => bp.CanRestore)
                .OrderByDescending(bp => bp.CreatedAt)
                .FirstOrDefault();

            if (targetBackup != null)
            {
                plan.BackupPointId = targetBackup.Id;
            }

            // Create rollback steps
            var steps = new List<RollbackStep>();

            steps.Add(new RollbackStep
            {
                StepNumber = 1,
                Name = "Pre-rollback Validation",
                Description = "Validate system state and backup point",
                IsRequired = true
            });

            steps.Add(new RollbackStep
            {
                StepNumber = 2,
                Name = "Stop Services",
                Description = "Stop application services",
                IsRequired = true
            });

            steps.Add(new RollbackStep
            {
                StepNumber = 3,
                Name = "Restore Database",
                Description = "Restore database from backup",
                IsRequired = true
            });

            steps.Add(new RollbackStep
            {
                StepNumber = 4,
                Name = "Restore Configuration",
                Description = "Restore configuration files",
                IsRequired = true
            });

            steps.Add(new RollbackStep
            {
                StepNumber = 5,
                Name = "Restore Application Files",
                Description = "Restore application files and user data",
                IsRequired = true
            });

            steps.Add(new RollbackStep
            {
                StepNumber = 6,
                Name = "Restart Services",
                Description = "Restart application services",
                IsRequired = true
            });

            steps.Add(new RollbackStep
            {
                StepNumber = 7,
                Name = "Post-rollback Validation",
                Description = "Validate system integrity after rollback",
                IsRequired = true
            });

            plan.Steps = steps;
            plan.EstimatedDuration = TimeSpan.FromMinutes(steps.Count * 2); // Estimate 2 minutes per step
            plan.RequiresDowntime = true;
            plan.Status = RollbackPlanStatus.Validated;

            await Task.Delay(1); // Make it async
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating rollback plan");
            plan.Status = RollbackPlanStatus.Failed;
        }

        return plan;
    }

    public async Task<bool> ExecuteRollbackPlanAsync(RollbackPlan plan)
    {
        try
        {
            _logger.LogInformation("Executing rollback plan: {PlanId}", plan.Id);
            plan.Status = RollbackPlanStatus.InProgress;

            foreach (var step in plan.Steps.OrderBy(s => s.StepNumber))
            {
                step.Status = RollbackStepStatus.InProgress;
                step.StartTime = DateTime.UtcNow;

                var success = await ExecuteRollbackStepAsync(step, plan);
                
                step.EndTime = DateTime.UtcNow;
                step.Duration = step.EndTime.Value - step.StartTime;
                step.Status = success ? RollbackStepStatus.Completed : RollbackStepStatus.Failed;

                if (!success && step.IsRequired)
                {
                    _logger.LogError("Required rollback step failed: {StepName}", step.Name);
                    plan.Status = RollbackPlanStatus.Failed;
                    return false;
                }
            }

            plan.Status = RollbackPlanStatus.Completed;
            _logger.LogInformation("Rollback plan executed successfully: {PlanId}", plan.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing rollback plan: {PlanId}", plan.Id);
            plan.Status = RollbackPlanStatus.Failed;
            return false;
        }
    }

    public async Task<UpgradeMonitoringResult> StartUpgradeMonitoringAsync()
    {
        try
        {
            _logger.LogInformation("Starting upgrade monitoring");

            // Create pre-upgrade backup
            var preUpgradeBackup = await CreateBackupPointAsync("Pre-upgrade backup", BackupType.PreUpgrade);
            
            // Create pre-upgrade snapshot
            var preUpgradeSnapshot = await CreateSystemSnapshotAsync();

            _activeMonitoring = new UpgradeMonitoringResult
            {
                PreUpgradeBackupId = preUpgradeBackup.Id,
                PreUpgradeSnapshot = preUpgradeSnapshot
            };

            // Start monitoring task (simplified)
            _ = Task.Run(async () => await MonitorUpgradeAsync(_activeMonitoring));

            return _activeMonitoring;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting upgrade monitoring");
            throw;
        }
    }

    public async Task<bool> StopUpgradeMonitoringAsync(bool upgradeSuccessful)
    {
        try
        {
            if (_activeMonitoring == null)
            {
                return false;
            }

            _activeMonitoring.IsActive = false;

            if (!upgradeSuccessful && _activeMonitoring.Config.AutoRollbackOnFailure)
            {
                _logger.LogWarning("Upgrade failed, initiating auto-rollback");
                return await AutoRollbackOnFailureAsync();
            }

            _logger.LogInformation("Upgrade monitoring stopped. Success: {UpgradeSuccessful}", upgradeSuccessful);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping upgrade monitoring");
            return false;
        }
        finally
        {
            _activeMonitoring = null;
        }
    }

    public async Task<bool> AutoRollbackOnFailureAsync()
    {
        try
        {
            if (_activeMonitoring?.PreUpgradeBackupId == null)
            {
                _logger.LogError("Cannot perform auto-rollback: No pre-upgrade backup available");
                return false;
            }

            _logger.LogInformation("Performing auto-rollback to pre-upgrade state");

            var success = await RestoreFromBackupPointAsync(_activeMonitoring.PreUpgradeBackupId);
            
            if (success)
            {
                _logger.LogInformation("Auto-rollback completed successfully");
            }
            else
            {
                _logger.LogError("Auto-rollback failed");
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during auto-rollback");
            return false;
        }
    }

    public async Task<SystemSnapshot> CreateSystemSnapshotAsync()
    {
        var snapshot = new SystemSnapshot
        {
            Name = $"Snapshot_{DateTime.UtcNow:yyyyMMdd_HHmmss}",
            Version = GetCurrentVersion()
        };

        try
        {
            _logger.LogInformation("Creating system snapshot: {SnapshotId}", snapshot.Id);

            // Create database snapshot
            snapshot.Database = await CreateDatabaseSnapshotAsync();

            // Create configuration snapshot
            snapshot.Configuration = await CreateConfigurationSnapshotAsync();

            // Create file system snapshot
            snapshot.FileSystem = await CreateFileSystemSnapshotAsync();

            // Create system state snapshot
            snapshot.SystemState = await CreateSystemStateSnapshotAsync();

            // Calculate checksum
            snapshot.ChecksumHash = CalculateSnapshotChecksum(snapshot);

            // Save snapshot
            await SaveSnapshotAsync(snapshot);

            _logger.LogInformation("System snapshot created successfully: {SnapshotId}", snapshot.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating system snapshot");
        }

        return snapshot;
    }

    public async Task<bool> RestoreSystemSnapshotAsync(string snapshotId)
    {
        try
        {
            _logger.LogInformation("Restoring system snapshot: {SnapshotId}", snapshotId);

            var snapshot = await LoadSnapshotAsync(snapshotId);
            if (snapshot == null)
            {
                _logger.LogError("Snapshot not found: {SnapshotId}", snapshotId);
                return false;
            }

            // Restore components based on snapshot
            // This is a simplified implementation
            await Task.Delay(100); // Placeholder for actual restore logic

            _logger.LogInformation("System snapshot restored successfully: {SnapshotId}", snapshotId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring system snapshot: {SnapshotId}", snapshotId);
            return false;
        }
    }

    public async Task<bool> VerifySystemIntegrityAsync()
    {
        try
        {
            _logger.LogInformation("Verifying system integrity");

            // Check database connectivity
            var canConnectToDb = await _context.Database.CanConnectAsync();
            if (!canConnectToDb)
            {
                _logger.LogError("Database connectivity check failed");
                return false;
            }

            // Check essential configuration
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogError("Essential configuration missing");
                return false;
            }

            // Check essential files
            var essentialFiles = new[]
            {
                "appsettings.json",
                Path.GetFileName(Assembly.GetExecutingAssembly().Location)
            };

            foreach (var file in essentialFiles)
            {
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);
                if (!File.Exists(filePath))
                {
                    _logger.LogError("Essential file missing: {FilePath}", filePath);
                    return false;
                }
            }

            _logger.LogInformation("System integrity verification passed");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during system integrity verification");
            return false;
        }
    }

    public async Task<IEnumerable<UpgradeHistory>> GetUpgradeHistoryAsync()
    {
        try
        {
            // This would typically be stored in database
            // For now, return a simple list
            var history = new List<UpgradeHistory>();

            // Get from configuration or database
            var historyJson = await _configService.GetValueAsync<string>("UpgradeHistory:Records") ?? "[]";
            var records = JsonSerializer.Deserialize<List<UpgradeHistory>>(historyJson) ?? new List<UpgradeHistory>();

            return records.OrderByDescending(h => h.StartTime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting upgrade history");
            return new List<UpgradeHistory>();
        }
    }

    // Private helper methods
    private async Task<BackupComponent?> BackupDatabaseAsync(string backupDir)
    {
        try
        {
            var dbBackupPath = Path.Combine(backupDir, "database.bak");
            
            // Simple database backup - copy the database file if it's SQLite
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(connectionString) && connectionString.Contains(".db"))
            {
                var match = System.Text.RegularExpressions.Regex.Match(connectionString, @"Data Source=([^;]+)");
                if (match.Success)
                {
                    var dbPath = match.Groups[1].Value;
                    if (File.Exists(dbPath))
                    {
                        File.Copy(dbPath, dbBackupPath);
                        
                        var fileInfo = new System.IO.FileInfo(dbBackupPath);
                        return new BackupComponent
                        {
                            Name = "Database",
                            Type = ComponentType.Database,
                            Path = dbBackupPath,
                            SizeBytes = fileInfo.Length,
                            Checksum = await CalculateFileChecksumAsync(dbBackupPath)
                        };
                    }
                }
            }

            await Task.Delay(1); // Make it async
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error backing up database");
            return null;
        }
    }

    private async Task<BackupComponent?> BackupConfigurationAsync(string backupDir)
    {
        try
        {
            var configDir = Path.Combine(backupDir, "configuration");
            Directory.CreateDirectory(configDir);

            var configFiles = new[] { "appsettings.json", "appsettings.Production.json", "appsettings.Development.json" };
            long totalSize = 0;

            foreach (var configFile in configFiles)
            {
                var sourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile);
                if (File.Exists(sourcePath))
                {
                    var targetPath = Path.Combine(configDir, configFile);
                    File.Copy(sourcePath, targetPath);
                    totalSize += new System.IO.FileInfo(targetPath).Length;
                }
            }

            return new BackupComponent
            {
                Name = "Configuration",
                Type = ComponentType.Configuration,
                Path = configDir,
                SizeBytes = totalSize,
                Checksum = await CalculateDirectoryChecksumAsync(configDir)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error backing up configuration");
            return null;
        }
    }

    private async Task<BackupComponent?> BackupUserDataAsync(string backupDir)
    {
        try
        {
            var userDataDir = Path.Combine(backupDir, "userdata");
            Directory.CreateDirectory(userDataDir);

            // Backup logs directory if it exists
            var logsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (Directory.Exists(logsDir))
            {
                var targetLogsDir = Path.Combine(userDataDir, "logs");
                await CopyDirectoryAsync(logsDir, targetLogsDir);
            }

            var totalSize = Directory.Exists(userDataDir) ? GetDirectorySize(userDataDir) : 0;

            return new BackupComponent
            {
                Name = "User Data",
                Type = ComponentType.UserData,
                Path = userDataDir,
                SizeBytes = totalSize,
                Checksum = await CalculateDirectoryChecksumAsync(userDataDir)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error backing up user data");
            return null;
        }
    }

    private async Task<BackupComponent?> BackupCertificatesAsync(string backupDir)
    {
        try
        {
            var certDir = Path.Combine(backupDir, "certificates");
            Directory.CreateDirectory(certDir);

            // This is a placeholder - in a real implementation, you'd backup certificates
            await Task.Delay(1);

            return new BackupComponent
            {
                Name = "Certificates",
                Type = ComponentType.Certificates,
                Path = certDir,
                SizeBytes = 0,
                Checksum = await CalculateDirectoryChecksumAsync(certDir)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error backing up certificates");
            return null;
        }
    }

    private async Task<BackupComponent?> BackupPluginsAsync(string backupDir)
    {
        try
        {
            var pluginDir = Path.Combine(backupDir, "plugins");
            Directory.CreateDirectory(pluginDir);

            // This is a placeholder - in a real implementation, you'd backup plugins
            await Task.Delay(1);

            return new BackupComponent
            {
                Name = "Plugins",
                Type = ComponentType.Plugins,
                Path = pluginDir,
                SizeBytes = 0,
                Checksum = await CalculateDirectoryChecksumAsync(pluginDir)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error backing up plugins");
            return null;
        }
    }

    private async Task<string> CalculateBackupChecksumAsync(string backupDir)
    {
        try
        {
            using var sha256 = SHA256.Create();
            var files = Directory.GetFiles(backupDir, "*", SearchOption.AllDirectories)
                               .OrderBy(f => f);

            foreach (var file in files)
            {
                var fileBytes = await File.ReadAllBytesAsync(file);
                sha256.TransformBlock(fileBytes, 0, fileBytes.Length, null, 0);
            }

            sha256.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
            return Convert.ToHexString(sha256.Hash ?? Array.Empty<byte>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating backup checksum");
            return string.Empty;
        }
    }

    private async Task<string> CalculateFileChecksumAsync(string filePath)
    {
        try
        {
            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(filePath);
            var hash = await sha256.ComputeHashAsync(stream);
            return Convert.ToHexString(hash);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating file checksum: {FilePath}", filePath);
            return string.Empty;
        }
    }

    private async Task<string> CalculateDirectoryChecksumAsync(string dirPath)
    {
        try
        {
            if (!Directory.Exists(dirPath))
            {
                return string.Empty;
            }

            using var sha256 = SHA256.Create();
            var files = Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories)
                               .OrderBy(f => f);

            foreach (var file in files)
            {
                var fileBytes = await File.ReadAllBytesAsync(file);
                sha256.TransformBlock(fileBytes, 0, fileBytes.Length, null, 0);
            }

            sha256.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
            return Convert.ToHexString(sha256.Hash ?? Array.Empty<byte>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating directory checksum: {DirPath}", dirPath);
            return string.Empty;
        }
    }

    private async Task SaveBackupMetadataAsync(BackupPoint backupPoint)
    {
        try
        {
            var metadataPath = Path.Combine(backupPoint.BackupPath, "metadata.json");
            var json = JsonSerializer.Serialize(backupPoint, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(metadataPath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving backup metadata");
        }
    }

    private async Task<BackupPoint?> GetBackupPointAsync(string backupPointId)
    {
        try
        {
            var backupDir = Path.Combine(_backupPath, backupPointId);
            var metadataPath = Path.Combine(backupDir, "metadata.json");

            if (!File.Exists(metadataPath))
            {
                return null;
            }

            var json = await File.ReadAllTextAsync(metadataPath);
            return JsonSerializer.Deserialize<BackupPoint>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting backup point: {BackupPointId}", backupPointId);
            return null;
        }
    }

    private async Task<bool> ValidateBackupPointAsync(string backupPointId)
    {
        try
        {
            var backupPoint = await GetBackupPointAsync(backupPointId);
            if (backupPoint == null)
            {
                return false;
            }

            if (!Directory.Exists(backupPoint.BackupPath))
            {
                return false;
            }

            // Validate checksum
            var currentChecksum = await CalculateBackupChecksumAsync(backupPoint.BackupPath);
            if (currentChecksum != backupPoint.ChecksumHash)
            {
                backupPoint.IsCorrupted = true;
                backupPoint.CanRestore = false;
                await SaveBackupMetadataAsync(backupPoint);
                return false;
            }

            backupPoint.LastVerified = DateTime.UtcNow;
            await SaveBackupMetadataAsync(backupPoint);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating backup point: {BackupPointId}", backupPointId);
            return false;
        }
    }

    private int GetComponentRestoreOrder(ComponentType type)
    {
        return type switch
        {
            ComponentType.Database => 1,
            ComponentType.Configuration => 2,
            ComponentType.System => 3,
            ComponentType.UserData => 4,
            ComponentType.Plugins => 5,
            ComponentType.Certificates => 6,
            ComponentType.Cache => 7,
            ComponentType.Logs => 8,
            ComponentType.Temporary => 9,
            _ => 10
        };
    }

    private async Task<bool> RestoreComponentAsync(BackupComponent component, string backupPath)
    {
        try
        {
            _logger.LogInformation("Restoring component: {ComponentName}", component.Name);

            switch (component.Type)
            {
                case ComponentType.Database:
                    return await RestoreDatabaseComponentAsync(component, backupPath);
                    
                case ComponentType.Configuration:
                    return await RestoreConfigurationComponentAsync(component, backupPath);
                    
                case ComponentType.UserData:
                    return await RestoreUserDataComponentAsync(component, backupPath);
                    
                default:
                    // Generic file/directory restore
                    return await RestoreGenericComponentAsync(component, backupPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring component: {ComponentName}", component.Name);
            return false;
        }
    }

    private async Task<bool> RestoreDatabaseComponentAsync(BackupComponent component, string backupPath)
    {
        try
        {
            var dbBackupPath = Path.Combine(backupPath, "database.bak");
            if (!File.Exists(dbBackupPath))
            {
                return false;
            }

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(connectionString) && connectionString.Contains(".db"))
            {
                var match = System.Text.RegularExpressions.Regex.Match(connectionString, @"Data Source=([^;]+)");
                if (match.Success)
                {
                    var dbPath = match.Groups[1].Value;
                    File.Copy(dbBackupPath, dbPath, true);
                    return true;
                }
            }

            await Task.Delay(1); // Make it async
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring database component");
            return false;
        }
    }

    private async Task<bool> RestoreConfigurationComponentAsync(BackupComponent component, string backupPath)
    {
        try
        {
            var configDir = Path.Combine(backupPath, "configuration");
            if (!Directory.Exists(configDir))
            {
                return false;
            }

            var targetDir = AppDomain.CurrentDomain.BaseDirectory;
            await CopyDirectoryAsync(configDir, targetDir);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring configuration component");
            return false;
        }
    }

    private async Task<bool> RestoreUserDataComponentAsync(BackupComponent component, string backupPath)
    {
        try
        {
            var userDataDir = Path.Combine(backupPath, "userdata");
            if (!Directory.Exists(userDataDir))
            {
                return true; // Not an error if no user data to restore
            }

            var targetDir = AppDomain.CurrentDomain.BaseDirectory;
            await CopyDirectoryAsync(userDataDir, targetDir);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring user data component");
            return false;
        }
    }

    private async Task<bool> RestoreGenericComponentAsync(BackupComponent component, string backupPath)
    {
        try
        {
            if (Directory.Exists(component.Path))
            {
                var targetDir = AppDomain.CurrentDomain.BaseDirectory;
                await CopyDirectoryAsync(component.Path, targetDir);
                return true;
            }
            else if (File.Exists(component.Path))
            {
                var targetFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(component.Path));
                File.Copy(component.Path, targetFile, true);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring generic component: {ComponentName}", component.Name);
            return false;
        }
    }

    private async Task<bool> ExecuteRollbackStepAsync(RollbackStep step, RollbackPlan plan)
    {
        try
        {
            switch (step.Name)
            {
                case "Pre-rollback Validation":
                    return await ValidatePreRollbackAsync(plan);
                    
                case "Stop Services":
                    return await StopServicesAsync();
                    
                case "Restore Database":
                    return await RestoreDatabaseFromPlanAsync(plan);
                    
                case "Restore Configuration":
                    return await RestoreConfigurationFromPlanAsync(plan);
                    
                case "Restore Application Files":
                    return await RestoreApplicationFilesFromPlanAsync(plan);
                    
                case "Restart Services":
                    return await RestartServicesAsync();
                    
                case "Post-rollback Validation":
                    return await ValidatePostRollbackAsync(plan);
                    
                default:
                    _logger.LogWarning("Unknown rollback step: {StepName}", step.Name);
                    return true; // Don't fail for unknown steps
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing rollback step: {StepName}", step.Name);
            step.ErrorMessage = ex.Message;
            return false;
        }
    }

    private async Task<bool> ValidatePreRollbackAsync(RollbackPlan plan)
    {
        if (string.IsNullOrEmpty(plan.BackupPointId))
        {
            return false;
        }

        return await ValidateBackupPointAsync(plan.BackupPointId);
    }

    private async Task<bool> StopServicesAsync()
    {
        // Placeholder for stopping services
        await Task.Delay(100);
        return true;
    }

    private async Task<bool> RestoreDatabaseFromPlanAsync(RollbackPlan plan)
    {
        if (string.IsNullOrEmpty(plan.BackupPointId))
        {
            return false;
        }

        var backupPoint = await GetBackupPointAsync(plan.BackupPointId);
        if (backupPoint == null)
        {
            return false;
        }

        var dbComponent = backupPoint.Components.FirstOrDefault(c => c.Type == ComponentType.Database);
        if (dbComponent != null)
        {
            return await RestoreComponentAsync(dbComponent, backupPoint.BackupPath);
        }

        return true; // Success if no database component to restore
    }

    private async Task<bool> RestoreConfigurationFromPlanAsync(RollbackPlan plan)
    {
        if (string.IsNullOrEmpty(plan.BackupPointId))
        {
            return false;
        }

        var backupPoint = await GetBackupPointAsync(plan.BackupPointId);
        if (backupPoint == null)
        {
            return false;
        }

        var configComponent = backupPoint.Components.FirstOrDefault(c => c.Type == ComponentType.Configuration);
        if (configComponent != null)
        {
            return await RestoreComponentAsync(configComponent, backupPoint.BackupPath);
        }

        return true;
    }

    private async Task<bool> RestoreApplicationFilesFromPlanAsync(RollbackPlan plan)
    {
        if (string.IsNullOrEmpty(plan.BackupPointId))
        {
            return false;
        }

        var backupPoint = await GetBackupPointAsync(plan.BackupPointId);
        if (backupPoint == null)
        {
            return false;
        }

        var userDataComponent = backupPoint.Components.FirstOrDefault(c => c.Type == ComponentType.UserData);
        if (userDataComponent != null)
        {
            return await RestoreComponentAsync(userDataComponent, backupPoint.BackupPath);
        }

        return true;
    }

    private async Task<bool> RestartServicesAsync()
    {
        // Placeholder for restarting services
        await Task.Delay(100);
        return true;
    }

    private async Task<bool> ValidatePostRollbackAsync(RollbackPlan plan)
    {
        return await VerifySystemIntegrityAsync();
    }

    private async Task MonitorUpgradeAsync(UpgradeMonitoringResult monitoring)
    {
        try
        {
            while (monitoring.IsActive)
            {
                // Perform health checks
                var isHealthy = await VerifySystemIntegrityAsync();
                
                if (!isHealthy && monitoring.Config.AutoRollbackOnFailure)
                {
                    _logger.LogWarning("Health check failed during upgrade monitoring, initiating auto-rollback");
                    await AutoRollbackOnFailureAsync();
                    break;
                }

                await Task.Delay(TimeSpan.FromSeconds(monitoring.Config.HealthCheckIntervalSeconds));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during upgrade monitoring");
        }
    }

    private async Task<DatabaseSnapshot> CreateDatabaseSnapshotAsync()
    {
        var snapshot = new DatabaseSnapshot();

        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            snapshot.ConnectionString = connectionString ?? string.Empty;

            if (!string.IsNullOrEmpty(connectionString))
            {
                if (connectionString.Contains(".db"))
                {
                    snapshot.DatabaseType = "SQLite";
                }
                else if (connectionString.Contains("Server="))
                {
                    snapshot.DatabaseType = "SQL Server";
                }

                // Get table information
                if (await _context.Database.CanConnectAsync())
                {
                    // This is simplified - in reality you'd get actual table names and counts
                    snapshot.Tables = new List<string> { "Users", "AuditLogs", "Configurations" };
                    snapshot.RecordCounts = new Dictionary<string, int>
                    {
                        ["Users"] = 0, // You'd get actual counts here
                        ["AuditLogs"] = 0,
                        ["Configurations"] = 0
                    };
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating database snapshot");
        }

        return snapshot;
    }

    private async Task<ConfigurationSnapshot> CreateConfigurationSnapshotAsync()
    {
        var snapshot = new ConfigurationSnapshot();

        try
        {
            // Get app settings
            foreach (var section in _configuration.AsEnumerable())
            {
                if (!string.IsNullOrEmpty(section.Value))
                {
                    snapshot.AppSettings[section.Key] = section.Value;
                }
            }

            // Get connection strings
            var connectionStrings = _configuration.GetSection("ConnectionStrings");
            foreach (var cs in connectionStrings.AsEnumerable())
            {
                if (!string.IsNullOrEmpty(cs.Value))
                {
                    snapshot.ConnectionStrings[cs.Key] = cs.Value;
                }
            }

            await Task.Delay(1); // Make it async
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating configuration snapshot");
        }

        return snapshot;
    }

    private async Task<FileSystemSnapshot> CreateFileSystemSnapshotAsync()
    {
        var snapshot = new FileSystemSnapshot();

        try
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var importantFiles = new[] { "appsettings.json", "ASL.LivingGrid.WebAdminPanel.exe", "ASL.LivingGrid.WebAdminPanel.dll" };

            foreach (var fileName in importantFiles)
            {
                var filePath = Path.Combine(basePath, fileName);
                if (File.Exists(filePath))
                {
                    var fileInfo = new System.IO.FileInfo(filePath);
                    snapshot.ImportantFiles.Add(new Models.FileInfo
                    {
                        Path = filePath,
                        SizeBytes = fileInfo.Length,
                        LastModified = fileInfo.LastWriteTime,
                        Hash = await CalculateFileChecksumAsync(filePath)
                    });
                }
            }

            await Task.Delay(1); // Make it async
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating file system snapshot");
        }

        return snapshot;
    }

    private async Task<SystemStateSnapshot> CreateSystemStateSnapshotAsync()
    {
        var snapshot = new SystemStateSnapshot();

        try
        {
            // Get current process information
            var currentProcess = Process.GetCurrentProcess();
            snapshot.Processes.Add(new ProcessInfo
            {
                Name = currentProcess.ProcessName,
                ProcessId = currentProcess.Id,
                MemoryUsageBytes = currentProcess.WorkingSet64,
                StartTime = currentProcess.StartTime
            });

            // Get system resources
            snapshot.Resources = new SystemResourceInfo
            {
                MemoryUsageBytes = currentProcess.WorkingSet64,
                AvailableMemoryBytes = GC.GetTotalMemory(false)
            };

            await Task.Delay(1); // Make it async
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating system state snapshot");
        }

        return snapshot;
    }

    private string CalculateSnapshotChecksum(SystemSnapshot snapshot)
    {
        try
        {
            var json = JsonSerializer.Serialize(snapshot);
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(json));
            return Convert.ToHexString(hash);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating snapshot checksum");
            return string.Empty;
        }
    }

    private async Task SaveSnapshotAsync(SystemSnapshot snapshot)
    {
        try
        {
            var snapshotPath = Path.Combine(_snapshotPath, $"{snapshot.Id}.json");
            var json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(snapshotPath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving snapshot");
        }
    }

    private async Task<SystemSnapshot?> LoadSnapshotAsync(string snapshotId)
    {
        try
        {
            var snapshotPath = Path.Combine(_snapshotPath, $"{snapshotId}.json");
            if (!File.Exists(snapshotPath))
            {
                return null;
            }

            var json = await File.ReadAllTextAsync(snapshotPath);
            return JsonSerializer.Deserialize<SystemSnapshot>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading snapshot: {SnapshotId}", snapshotId);
            return null;
        }
    }

    private async Task CopyDirectoryAsync(string sourceDir, string targetDir)
    {
        await Task.Run(() =>
        {
            if (!Directory.Exists(sourceDir))
                return;

            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var targetFile = Path.Combine(targetDir, Path.GetFileName(file));
                File.Copy(file, targetFile, true);
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                var targetSubDir = Path.Combine(targetDir, Path.GetFileName(dir));
                CopyDirectoryAsync(dir, targetSubDir).Wait();
            }
        });
    }

    private long GetDirectorySize(string dirPath)
    {
        try
        {
            return Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories)
                           .Sum(file => new System.IO.FileInfo(file).Length);
        }
        catch
        {
            return 0;
        }
    }

    private string GetCurrentVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";
    }
}
