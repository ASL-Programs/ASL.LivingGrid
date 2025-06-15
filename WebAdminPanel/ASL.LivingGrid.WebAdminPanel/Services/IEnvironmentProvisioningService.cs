using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IEnvironmentProvisioningService
{
    Task<EnvironmentProvisioningWizardState> GetWizardStateAsync();
    Task<bool> InitializeEnvironmentAsync(EnvironmentSetupOptions options);
    Task<IEnumerable<EnvironmentTemplate>> GetAvailableTemplatesAsync();
    Task<EnvironmentValidationResult> ValidateEnvironmentConfigurationAsync(EnvironmentSetupOptions options);
    Task<bool> ProvisionEnvironmentAsync(EnvironmentSetupOptions options);
    Task<bool> SwitchEnvironmentAsync(string environmentName);
    Task<IEnumerable<EnvironmentInfo>> GetExistingEnvironmentsAsync();
    Task<bool> CloneEnvironmentAsync(string sourceEnvironment, string targetEnvironment);
    Task<bool> DeleteEnvironmentAsync(string environmentName);
    Task<EnvironmentHealth> CheckEnvironmentHealthAsync(string environmentName);
    Task<bool> ApplyEnvironmentConfigurationAsync(string environmentName, Dictionary<string, object> configuration);
}

public class EnvironmentProvisioningWizardState
{
    public bool IsCompleted { get; set; }
    public int CurrentStep { get; set; }
    public int TotalSteps { get; set; } = 5;
    public string CurrentEnvironment { get; set; } = "Development";
    public List<WizardStep> Steps { get; set; } = new();
    public Dictionary<string, object> CollectedData { get; set; } = new();
    public bool HasExistingEnvironments { get; set; }
}

public class WizardStep
{
    public int StepNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public bool IsActive { get; set; }
    public bool IsRequired { get; set; } = true;
    public Dictionary<string, object> StepData { get; set; } = new();
}

public class EnvironmentSetupOptions
{
    public string EnvironmentName { get; set; } = string.Empty;
    public EnvironmentType EnvironmentType { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public bool IsIsolated { get; set; } = true;
    
    // Database Configuration
    public DatabaseConfiguration Database { get; set; } = new();
    
    // Network Configuration
    public NetworkConfiguration Network { get; set; } = new();
    
    // Security Configuration
    public SecurityConfiguration Security { get; set; } = new();
    
    // Features Configuration
    public FeatureConfiguration Features { get; set; } = new();
    
    // Logging Configuration
    public LoggingConfiguration Logging { get; set; } = new();
    
    // Integration Configuration
    public IntegrationConfiguration Integrations { get; set; } = new();
    
    // Custom Settings
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

public class DatabaseConfiguration
{
    public string DatabaseType { get; set; } = "sqlite"; // sqlite, sqlserver, postgresql
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public bool CreateDatabase { get; set; } = true;
    public bool RunMigrations { get; set; } = true;
    public bool SeedDemoData { get; set; } = false;
    public bool EnableBackup { get; set; } = true;
    public string BackupSchedule { get; set; } = "Daily";
}

public class NetworkConfiguration
{
    public string HttpPort { get; set; } = "5000";
    public string HttpsPort { get; set; } = "5001";
    public bool EnableHttps { get; set; } = true;
    public bool RequireHttps { get; set; } = false;
    public List<string> AllowedHosts { get; set; } = new() { "*" };
    public string BaseUrl { get; set; } = string.Empty;
    public bool EnableCors { get; set; } = true;
    public List<string> CorsOrigins { get; set; } = new();
}

public class SecurityConfiguration
{
    public bool EnableAuthentication { get; set; } = true;
    public bool RequireHttps { get; set; } = false;
    public string JwtSecretKey { get; set; } = string.Empty;
    public int SessionTimeoutMinutes { get; set; } = 30;
    public bool EnableTwoFactorAuth { get; set; } = false;
    public bool EnablePasswordPolicy { get; set; } = true;
    public bool EnableApiKeyAuth { get; set; } = true;
    public List<string> TrustedIpRanges { get; set; } = new();
}

public class FeatureConfiguration
{
    public bool EnableTrayIcon { get; set; } = true;
    public bool EnableNotifications { get; set; } = true;
    public bool EnablePlugins { get; set; } = true;
    public bool EnableWorkflowDesigner { get; set; } = true;
    public bool EnableFormBuilder { get; set; } = true;
    public bool EnableReporting { get; set; } = true;
    public bool EnableAuditLogging { get; set; } = true;
    public bool EnableAI { get; set; } = false;
    public List<string> EnabledModules { get; set; } = new();
}

public class LoggingConfiguration
{
    public string LogLevel { get; set; } = "Information";
    public bool EnableFileLogging { get; set; } = true;
    public bool EnableConsoleLogging { get; set; } = true;
    public bool EnableDatabaseLogging { get; set; } = false;
    public string LogFilePath { get; set; } = "logs";
    public int RetentionDays { get; set; } = 30;
    public bool EnableStructuredLogging { get; set; } = true;
}

public class IntegrationConfiguration
{
    public List<ExternalIntegration> ExternalSystems { get; set; } = new();
    public bool EnableWebhooks { get; set; } = true;
    public bool EnableApiAccess { get; set; } = true;
    public string ApiVersion { get; set; } = "v1";
    public bool EnableSwaggerUI { get; set; } = true;
}

public class ExternalIntegration
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public Dictionary<string, string> Settings { get; set; } = new();
    public bool IsEnabled { get; set; } = true;
}

public enum EnvironmentType
{
    Development,
    Testing,
    Staging,
    Production,
    Demo,
    Training,
    Backup
}

public class EnvironmentTemplate
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public EnvironmentType DefaultType { get; set; }
    public string IconClass { get; set; } = string.Empty;
    public EnvironmentSetupOptions DefaultOptions { get; set; } = new();
    public bool IsCustom { get; set; }
    public List<string> RequiredComponents { get; set; } = new();
    public List<string> OptionalComponents { get; set; } = new();
}

public class EnvironmentValidationResult
{
    public bool IsValid { get; set; }
    public List<ValidationIssue> Issues { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    public Dictionary<string, object> ValidationDetails { get; set; } = new();
}

public class ValidationIssue
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public ValidationSeverity Severity { get; set; }
    public string SuggestedFix { get; set; } = string.Empty;
}

public enum ValidationSeverity
{
    Info,
    Warning,
    Error,
    Critical
}

public class EnvironmentInfo
{
    public string Name { get; set; } = string.Empty;
    public EnvironmentType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModified { get; set; }
    public EnvironmentHealth Health { get; set; } = new();
    public Dictionary<string, object> Configuration { get; set; } = new();
    public List<string> Tags { get; set; } = new();
}

public class EnvironmentHealth
{
    public bool IsHealthy { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime LastChecked { get; set; }
    public List<HealthCheck> Checks { get; set; } = new();
    public Dictionary<string, object> Metrics { get; set; } = new();
}

public class HealthCheck
{
    public string Name { get; set; } = string.Empty;
    public bool IsHealthy { get; set; }
    public string Message { get; set; } = string.Empty;
    public TimeSpan ResponseTime { get; set; }
    public DateTime CheckedAt { get; set; }
}
