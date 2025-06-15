using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IInstallerService
{
    Task<InstallationStatus> GetInstallationStatusAsync();
    Task<bool> PerformInstallationAsync(InstallationOptions options);
    Task<bool> RepairInstallationAsync();
    Task<bool> UninstallAsync();
    Task<SystemHealthCheck> PerformHealthCheckAsync();
    Task<bool> SelfHealAsync();
    Task<IEnumerable<InstallationComponent>> GetRequiredComponentsAsync();
    Task<bool> ValidateSystemRequirementsAsync();
    Task<InstallationProgress> GetInstallationProgressAsync();
}

public class InstallationStatus
{
    public bool IsInstalled { get; set; }
    public bool IsHealthy { get; set; }
    public DateTime? InstallationDate { get; set; }
    public string Version { get; set; } = string.Empty;
    public string InstallationPath { get; set; } = string.Empty;
    public List<string> InstalledComponents { get; set; } = new();
    public List<string> MissingComponents { get; set; } = new();
    public List<SystemIssue> Issues { get; set; } = new();
}

public class InstallationOptions
{
    public string InstallationPath { get; set; } = string.Empty;
    public bool CreateDesktopShortcut { get; set; } = true;
    public bool CreateStartMenuShortcut { get; set; } = true;
    public bool AutoStart { get; set; } = false;
    public bool InstallAsService { get; set; } = false;
    public bool CreateFirewallRules { get; set; } = true;
    public string DatabaseType { get; set; } = "sqlite";
    public string DatabaseConnectionString { get; set; } = string.Empty;
    public bool BackupExistingData { get; set; } = true;
    public List<string> ComponentsToInstall { get; set; } = new();
}

public class SystemHealthCheck
{
    public bool IsHealthy { get; set; }
    public List<SystemIssue> Issues { get; set; } = new();
    public Dictionary<string, object> SystemInfo { get; set; } = new();
    public DateTime CheckDate { get; set; } = DateTime.UtcNow;
}

public class SystemIssue
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IssueSeverity Severity { get; set; }
    public bool CanAutoFix { get; set; }
    public string? FixAction { get; set; }
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
}

public enum IssueSeverity
{
    Info,
    Warning,
    Error,
    Critical
}

public class InstallationComponent
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsRequired { get; set; } = true;
    public bool IsInstalled { get; set; }
    public string Version { get; set; } = string.Empty;
    public string InstallationPath { get; set; } = string.Empty;
    public List<string> Dependencies { get; set; } = new();
}

public class InstallationProgress
{
    public bool IsInProgress { get; set; }
    public int ProgressPercentage { get; set; }
    public string CurrentOperation { get; set; } = string.Empty;
    public List<string> CompletedSteps { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public DateTime StartTime { get; set; }
    public TimeSpan? EstimatedTimeRemaining { get; set; }
}
