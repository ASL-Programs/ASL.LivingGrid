using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IOnboardingService
{
    Task<OnboardingStatus> GetOnboardingStatusAsync();
    Task<bool> CompleteSetupStepAsync(string stepId, object stepData);
    Task<OnboardingWizardData> GetWizardDataAsync();
    Task<bool> CompleteOnboardingAsync();
    Task<bool> ResetOnboardingAsync();
    Task<IEnumerable<OnboardingStep>> GetOnboardingStepsAsync();
}

public class OnboardingStatus
{
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int CurrentStep { get; set; }
    public int TotalSteps { get; set; }
    public List<string> CompletedSteps { get; set; } = new();
    public string? CurrentStepId { get; set; }
}

public class OnboardingStep
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsRequired { get; set; } = true;
    public string StepType { get; set; } = "form"; // form, info, confirmation
    public Dictionary<string, object> Data { get; set; } = new();
}

public class OnboardingWizardData
{
    public CompanySetup Company { get; set; } = new();
    public AdminUserSetup AdminUser { get; set; } = new();
    public DatabaseSetup Database { get; set; } = new();
    public SecuritySettings Security { get; set; } = new();
    public LanguageSettings Language { get; set; } = new();
    public NotificationSettings Notifications { get; set; } = new();
}

public class CompanySetup
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string TimeZone { get; set; } = string.Empty;
}

public class AdminUserSetup
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PreferredLanguage { get; set; } = "az";
}

public class DatabaseSetup
{
    public string ConnectionType { get; set; } = "sqlite"; // sqlite, sqlserver, postgresql
    public string ConnectionString { get; set; } = string.Empty;
    public bool CreateSampleData { get; set; } = true;
    public bool AutoBackup { get; set; } = true;
    public int BackupRetentionDays { get; set; } = 30;
}

public class SecuritySettings
{
    public bool RequireHttps { get; set; } = true;
    public bool EnableTwoFactorAuth { get; set; } = false;
    public int SessionTimeoutMinutes { get; set; } = 30;
    public bool EnableAuditLogging { get; set; } = true;
    public int PasswordMinLength { get; set; } = 8;
    public bool RequirePasswordComplexity { get; set; } = true;
    public int PasswordExpiryDays { get; set; } = 90;
    public int SecretRotationDays { get; set; } = 30;
    public bool EnableJitPrivilegeElevation { get; set; } = false;
    public bool EnforcePerTenantPolicy { get; set; } = false;
}

public class LanguageSettings
{
    public string DefaultLanguage { get; set; } = "az";
    public List<string> EnabledLanguages { get; set; } = new() { "az", "en", "tr", "ru" };
    public bool AllowUserLanguageChange { get; set; } = true;
}

public class NotificationSettings
{
    public bool EnableEmailNotifications { get; set; } = true;
    public bool EnableSystemNotifications { get; set; } = true;
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SmtpUsername { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public bool SmtpUseSsl { get; set; } = true;
    public bool EnableSmsNotifications { get; set; } = false;
    public string SmsApiUrl { get; set; } = string.Empty;
    public string SmsApiKey { get; set; } = string.Empty;
    public bool EnableTelegramNotifications { get; set; } = false;
    public string TelegramBotToken { get; set; } = string.Empty;
    public string TelegramChatId { get; set; } = string.Empty;
    public bool EnableWebhookNotifications { get; set; } = false;
    public string WebhookUrl { get; set; } = string.Empty;
}
