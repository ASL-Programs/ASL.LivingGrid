using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IFirstLaunchDiagnosticService
{
    Task<DiagnosticResult> PerformFirstLaunchDiagnosticAsync();
    Task<CompatibilityCheckResult> PerformCompatibilityCheckAsync();
    Task<SystemRecommendations> GetSystemRecommendationsAsync();
    Task<bool> FixDiagnosticIssueAsync(string issueId);
    Task<DiagnosticProgress> GetDiagnosticProgressAsync();
    Task<bool> MarkFirstLaunchCompleteAsync();
    Task<bool> IsFirstLaunchCompleteAsync();
    Task<SystemPerformanceReport> GetPerformanceReportAsync();
    Task<bool> PerformSystemOptimizationAsync();
}

public class DiagnosticResult
{
    public bool IsSuccess { get; set; }
    public bool IsFirstLaunch { get; set; }
    public DateTime DiagnosticDate { get; set; } = DateTime.UtcNow;
    public List<DiagnosticCheck> Checks { get; set; } = new();
    public List<DiagnosticIssue> Issues { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    public SystemInfo SystemInfo { get; set; } = new();
    public CompatibilityCheckResult Compatibility { get; set; } = new();
    public SystemPerformanceReport Performance { get; set; } = new();
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

public class DiagnosticCheck
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DiagnosticStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Details { get; set; } = new();
    public bool IsRequired { get; set; } = true;
    public bool CanFix { get; set; }
    public string? FixAction { get; set; }
}

public enum DiagnosticStatus
{
    NotStarted,
    Running,
    Passed,
    Warning,
    Failed,
    Skipped
}

public class DiagnosticIssue
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DiagnosticSeverity Severity { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool CanAutoFix { get; set; }
    public string? FixAction { get; set; }
    public string? FixDescription { get; set; }
    public List<string> AffectedComponents { get; set; } = new();
    public Dictionary<string, object> IssueData { get; set; } = new();
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
}

public enum DiagnosticSeverity
{
    Info,
    Warning,
    Error,
    Critical
}

public class SystemInfo
{
    public string OperatingSystem { get; set; } = string.Empty;
    public string OSVersion { get; set; } = string.Empty;
    public string MachineName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string DotNetVersion { get; set; } = string.Empty;
    public string ApplicationVersion { get; set; } = string.Empty;
    public int ProcessorCount { get; set; }
    public long TotalMemoryMB { get; set; }
    public long AvailableMemoryMB { get; set; }
    public List<DriveInfo> Drives { get; set; } = new();
    public List<NetworkInfo> NetworkInterfaces { get; set; } = new();
    public SecurityInfo Security { get; set; } = new();
    public DateTime SystemTime { get; set; } = DateTime.UtcNow;
    public TimeSpan SystemUptime { get; set; }
}

public class DriveInfo
{
    public string Name { get; set; } = string.Empty;
    public string DriveType { get; set; } = string.Empty;
    public long TotalSizeGB { get; set; }
    public long AvailableSpaceGB { get; set; }
    public string FileSystem { get; set; } = string.Empty;
    public bool IsReady { get; set; }
}

public class NetworkInfo
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsUp { get; set; }
    public List<string> IPAddresses { get; set; } = new();
    public string MacAddress { get; set; } = string.Empty;
    public long Speed { get; set; }
}

public class SecurityInfo
{
    public bool IsAdministrator { get; set; }
    public bool IsElevated { get; set; }
    public bool HasFirewall { get; set; }
    public bool HasAntivirus { get; set; }
    public List<string> SecurityFeatures { get; set; } = new();
}

public class CompatibilityCheckResult
{
    public bool IsCompatible { get; set; }
    public List<CompatibilityIssue> Issues { get; set; } = new();
    public RequirementStatus Requirements { get; set; } = new();
    public List<string> SupportedFeatures { get; set; } = new();
    public List<string> UnsupportedFeatures { get; set; } = new();
    public Dictionary<string, string> EnvironmentVariables { get; set; } = new();
}

public class CompatibilityIssue
{
    public string Component { get; set; } = string.Empty;
    public string Issue { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Solution { get; set; } = string.Empty;
    public bool IsBlocking { get; set; }
}

public class RequirementStatus
{
    public bool DotNetVersion { get; set; }
    public bool OperatingSystem { get; set; }
    public bool Memory { get; set; }
    public bool DiskSpace { get; set; }
    public bool NetworkConnectivity { get; set; }
    public bool Permissions { get; set; }
    public bool Dependencies { get; set; }
}

public class SystemRecommendations
{
    public List<Recommendation> Performance { get; set; } = new();
    public List<Recommendation> Security { get; set; } = new();
    public List<Recommendation> Configuration { get; set; } = new();
    public List<Recommendation> Maintenance { get; set; } = new();
    public List<Recommendation> Features { get; set; } = new();
}

public class Recommendation
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RecommendationType Type { get; set; }
    public RecommendationPriority Priority { get; set; }
    public bool CanAutoApply { get; set; }
    public string? AutoApplyAction { get; set; }
    public List<string> Benefits { get; set; } = new();
    public List<string> Requirements { get; set; } = new();
    public string Category { get; set; } = string.Empty;
}

public enum RecommendationType
{
    Configuration,
    Performance,
    Security,
    Feature,
    Maintenance,
    Update
}

public enum RecommendationPriority
{
    Low,
    Medium,
    High,
    Critical
}

public class DiagnosticProgress
{
    public bool IsRunning { get; set; }
    public int CompletedChecks { get; set; }
    public int TotalChecks { get; set; }
    public string CurrentCheck { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public TimeSpan? EstimatedTimeRemaining { get; set; }
    public List<string> CompletedCheckNames { get; set; } = new();
}

public class SystemPerformanceReport
{
    public double CpuUsagePercent { get; set; }
    public double MemoryUsagePercent { get; set; }
    public double DiskUsagePercent { get; set; }
    public long NetworkBytesReceived { get; set; }
    public long NetworkBytesSent { get; set; }
    public TimeSpan ApplicationStartupTime { get; set; }
    public int DatabaseConnectionTime { get; set; }
    public List<PerformanceMetric> Metrics { get; set; } = new();
    public PerformanceRating OverallRating { get; set; }
    public DateTime MeasuredAt { get; set; } = DateTime.UtcNow;
}

public class PerformanceMetric
{
    public string Name { get; set; } = string.Empty;
    public double Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public PerformanceRating Rating { get; set; }
    public string Description { get; set; } = string.Empty;
}

public enum PerformanceRating
{
    Excellent,
    Good,
    Fair,
    Poor,
    Critical
}
