using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IAdvancedRollbackService
{
    Task<BackupPoint> CreateBackupPointAsync(string description, BackupType type = BackupType.Manual);
    Task<bool> RestoreFromBackupPointAsync(string backupPointId);
    Task<IEnumerable<BackupPoint>> GetAvailableBackupPointsAsync();
    Task<RollbackResult> PerformRollbackAsync(string targetVersion, RollbackOptions options);
    Task<bool> ValidateRollbackAsync(string backupPointId);
    Task<bool> DeleteBackupPointAsync(string backupPointId);
    Task<BackupPoint?> GetLastStableBackupPointAsync();
    Task<RollbackPlan> CreateRollbackPlanAsync(string targetVersion);
    Task<bool> ExecuteRollbackPlanAsync(RollbackPlan plan);
    Task<UpgradeMonitoringResult> StartUpgradeMonitoringAsync();
    Task<bool> StopUpgradeMonitoringAsync(bool upgradeSuccessful);
    Task<bool> AutoRollbackOnFailureAsync();
    Task<SystemSnapshot> CreateSystemSnapshotAsync();
    Task<bool> RestoreSystemSnapshotAsync(string snapshotId);
    Task<bool> VerifySystemIntegrityAsync();
    Task<IEnumerable<UpgradeHistory>> GetUpgradeHistoryAsync();
}

public enum BackupType
{
    Manual,
    PreUpgrade,
    Scheduled,
    Emergency,
    Migration,
    Configuration,
    FullSystem
}

public class BackupPoint
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public BackupType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = Environment.UserName;
    public string Version { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public BackupStatus Status { get; set; }
    public string BackupPath { get; set; } = string.Empty;
    public List<BackupComponent> Components { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
    public string ChecksumHash { get; set; } = string.Empty;
    public bool IsCorrupted { get; set; }
    public DateTime? LastVerified { get; set; }
    public string? ErrorMessage { get; set; }
    public bool CanRestore { get; set; } = true;
    public TimeSpan EstimatedRestoreTime { get; set; }
    public List<string> Dependencies { get; set; } = new();
}

public enum BackupStatus
{
    Creating,
    Completed,
    Failed,
    Corrupted,
    Expired,
    Restoring,
    Verified
}

public class BackupComponent
{
    public string Name { get; set; } = string.Empty;
    public ComponentType Type { get; set; }
    public string Path { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string Checksum { get; set; } = string.Empty;
    public DateTime BackedUpAt { get; set; } = DateTime.UtcNow;
    public bool IsEssential { get; set; } = true;
    public Dictionary<string, object> ComponentData { get; set; } = new();
}

public enum ComponentType
{
    Database,
    Configuration,
    UserData,
    Logs,
    Certificates,
    Plugins,
    Cache,
    Temporary,
    System
}

public class RollbackResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime RollbackStartTime { get; set; }
    public DateTime RollbackEndTime { get; set; }
    public TimeSpan Duration { get; set; }
    public string FromVersion { get; set; } = string.Empty;
    public string ToVersion { get; set; } = string.Empty;
    public List<RollbackStep> Steps { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public string? ErrorDetails { get; set; }
    public bool RequiresManualIntervention { get; set; }
    public List<string> ManualSteps { get; set; } = new();
    public SystemSnapshot? PreRollbackSnapshot { get; set; }
    public SystemSnapshot? PostRollbackSnapshot { get; set; }
}

public class RollbackStep
{
    public int StepNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RollbackStepStatus Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public TimeSpan Duration { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsReversible { get; set; } = true;
    public bool IsRequired { get; set; } = true;
    public Dictionary<string, object> StepData { get; set; } = new();
}

public enum RollbackStepStatus
{
    NotStarted,
    InProgress,
    Completed,
    Failed,
    Skipped,
    Reversed
}

public class RollbackOptions
{
    public bool CreateBackupBeforeRollback { get; set; } = true;
    public bool VerifyIntegrityAfterRollback { get; set; } = true;
    public bool RestartServicesAfterRollback { get; set; } = true;
    public bool RunPostRollbackValidation { get; set; } = true;
    public bool SkipNonEssentialComponents { get; set; } = false;
    public bool ForceRollback { get; set; } = false;
    public List<string> ExcludeComponents { get; set; } = new();
    public Dictionary<string, object> CustomOptions { get; set; } = new();
    public int TimeoutMinutes { get; set; } = 30;
    public bool NotifyOnCompletion { get; set; } = true;
}

public class RollbackPlan
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string FromVersion { get; set; } = string.Empty;
    public string ToVersion { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public RollbackPlanStatus Status { get; set; }
    public List<RollbackStep> Steps { get; set; } = new();
    public List<string> Prerequisites { get; set; } = new();
    public List<string> Risks { get; set; } = new();
    public TimeSpan EstimatedDuration { get; set; }
    public bool RequiresDowntime { get; set; }
    public string? BackupPointId { get; set; }
    public RollbackOptions Options { get; set; } = new();
}

public enum RollbackPlanStatus
{
    Draft,
    Validated,
    Approved,
    InProgress,
    Completed,
    Failed,
    Cancelled
}

public class UpgradeMonitoringResult
{
    public string MonitoringId { get; set; } = Guid.NewGuid().ToString();
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public string PreUpgradeBackupId { get; set; } = string.Empty;
    public SystemSnapshot PreUpgradeSnapshot { get; set; } = new();
    public List<UpgradeValidationCheck> ValidationChecks { get; set; } = new();
    public UpgradeMonitoringConfig Config { get; set; } = new();
}

public class UpgradeValidationCheck
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsRequired { get; set; } = true;
    public bool IsCompleted { get; set; }
    public bool IsSuccessful { get; set; }
    public DateTime? CheckedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object> CheckData { get; set; } = new();
}

public class UpgradeMonitoringConfig
{
    public TimeSpan MonitoringDuration { get; set; } = TimeSpan.FromMinutes(30);
    public bool AutoRollbackOnFailure { get; set; } = true;
    public bool ValidateDatabaseIntegrity { get; set; } = true;
    public bool ValidateConfigurationIntegrity { get; set; } = true;
    public bool ValidateApplicationHealth { get; set; } = true;
    public int HealthCheckIntervalSeconds { get; set; } = 30;
    public int MaxFailedHealthChecks { get; set; } = 3;
    public List<string> CriticalServices { get; set; } = new();
}

public class SystemSnapshot
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Version { get; set; } = string.Empty;
    public DatabaseSnapshot Database { get; set; } = new();
    public ConfigurationSnapshot Configuration { get; set; } = new();
    public FileSystemSnapshot FileSystem { get; set; } = new();
    public SystemStateSnapshot SystemState { get; set; } = new();
    public Dictionary<string, object> CustomData { get; set; } = new();
    public string ChecksumHash { get; set; } = string.Empty;
    public long TotalSizeBytes { get; set; }
}

public class DatabaseSnapshot
{
    public string DatabaseType { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public string SchemaVersion { get; set; } = string.Empty;
    public List<string> Tables { get; set; } = new();
    public Dictionary<string, int> RecordCounts { get; set; } = new();
    public string BackupFilePath { get; set; } = string.Empty;
    public string SchemaHash { get; set; } = string.Empty;
}

public class ConfigurationSnapshot
{
    public Dictionary<string, object> AppSettings { get; set; } = new();
    public Dictionary<string, string> ConnectionStrings { get; set; } = new();
    public Dictionary<string, object> CustomSettings { get; set; } = new();
    public string ConfigurationHash { get; set; } = string.Empty;
    public List<string> ConfigurationFiles { get; set; } = new();
}

public class FileSystemSnapshot
{
    public List<FileInfo> ImportantFiles { get; set; } = new();
    public List<DirectoryInfo> ImportantDirectories { get; set; } = new();
    public Dictionary<string, string> FileHashes { get; set; } = new();
    public long TotalSizeBytes { get; set; }
}

public class FileInfo
{
    public string Path { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public DateTime LastModified { get; set; }
    public string Hash { get; set; } = string.Empty;
    public bool IsReadOnly { get; set; }
}

public class DirectoryInfo
{
    public string Path { get; set; } = string.Empty;
    public int FileCount { get; set; }
    public long TotalSizeBytes { get; set; }
    public DateTime LastModified { get; set; }
}

public class SystemStateSnapshot
{
    public List<ServiceInfo> Services { get; set; } = new();
    public List<ProcessInfo> Processes { get; set; } = new();
    public Dictionary<string, string> EnvironmentVariables { get; set; } = new();
    public List<PortInfo> OpenPorts { get; set; } = new();
    public SystemResourceInfo Resources { get; set; } = new();
}

public class ServiceInfo
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string StartType { get; set; } = string.Empty;
}

public class ProcessInfo
{
    public string Name { get; set; } = string.Empty;
    public int ProcessId { get; set; }
    public long MemoryUsageBytes { get; set; }
    public DateTime StartTime { get; set; }
}

public class PortInfo
{
    public int Port { get; set; }
    public string Protocol { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
}

public class SystemResourceInfo
{
    public double CpuUsagePercent { get; set; }
    public long MemoryUsageBytes { get; set; }
    public long AvailableMemoryBytes { get; set; }
    public Dictionary<string, long> DiskUsageBytes { get; set; } = new();
}

public class UpgradeHistory
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string FromVersion { get; set; } = string.Empty;
    public string ToVersion { get; set; } = string.Empty;
    public UpgradeStatus Status { get; set; }
    public string InitiatedBy { get; set; } = string.Empty;
    public string? BackupPointId { get; set; }
    public string? RollbackPointId { get; set; }
    public List<string> Issues { get; set; } = new();
    public TimeSpan Duration { get; set; }
    public Dictionary<string, object> UpgradeData { get; set; } = new();
}

public enum UpgradeStatus
{
    Started,
    InProgress,
    Completed,
    Failed,
    RolledBack,
    Cancelled
}
