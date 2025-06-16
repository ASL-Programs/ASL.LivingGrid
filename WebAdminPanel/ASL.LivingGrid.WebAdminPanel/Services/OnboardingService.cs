using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class OnboardingService : IOnboardingService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfigurationService _configService;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<OnboardingService> _logger;
    private readonly IAuditService _auditService;

    public OnboardingService(
        ApplicationDbContext context,
        IConfigurationService configService,
        UserManager<IdentityUser> userManager,
        ILogger<OnboardingService> logger,
        IAuditService auditService)
    {
        _context = context;
        _configService = configService;
        _userManager = userManager;
        _logger = logger;
        _auditService = auditService;
    }

    public async Task<OnboardingStatus> GetOnboardingStatusAsync()
    {
        try
        {
            var isCompleted = await _configService.GetValueAsync<bool>("Onboarding:Completed");
            var completedSteps = await _configService.GetValueAsync<string>("Onboarding:CompletedSteps") ?? "[]";
            var completedStepsList = JsonSerializer.Deserialize<List<string>>(completedSteps) ?? new List<string>();
            var currentStepId = await _configService.GetValueAsync<string>("Onboarding:CurrentStep");
            var completedAt = await _configService.GetValueAsync<DateTime?>("Onboarding:CompletedAt");

            var allSteps = await GetOnboardingStepsAsync();
            var totalSteps = allSteps.Count();
            var currentStep = allSteps.FirstOrDefault(s => s.Id == currentStepId);
            var currentStepIndex = currentStep != null ? currentStep.Order : 1;

            return new OnboardingStatus
            {
                IsCompleted = isCompleted,
                CompletedAt = completedAt,
                CurrentStep = currentStepIndex,
                TotalSteps = totalSteps,
                CompletedSteps = completedStepsList,
                CurrentStepId = currentStepId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting onboarding status");
            return new OnboardingStatus();
        }
    }

    public async Task<bool> CompleteSetupStepAsync(string stepId, object stepData)
    {
        try
        {
            _logger.LogInformation("Completing onboarding step: {StepId}", stepId);

            // Save step data
            var stepDataJson = JsonSerializer.Serialize(stepData);
            await _configService.SetValueAsync($"Onboarding:StepData:{stepId}", stepDataJson);

            // Update completed steps
            var completedSteps = await _configService.GetValueAsync<string>("Onboarding:CompletedSteps") ?? "[]";
            var completedStepsList = JsonSerializer.Deserialize<List<string>>(completedSteps) ?? new List<string>();
            
            if (!completedStepsList.Contains(stepId))
            {
                completedStepsList.Add(stepId);
                await _configService.SetValueAsync("Onboarding:CompletedSteps", JsonSerializer.Serialize(completedStepsList));
            }

            // Process step-specific logic
            await ProcessStepDataAsync(stepId, stepData);

            // Update current step to next step
            var allSteps = await GetOnboardingStepsAsync();
            var currentStep = allSteps.FirstOrDefault(s => s.Id == stepId);
            if (currentStep != null)
            {
                var nextStep = allSteps.FirstOrDefault(s => s.Order > currentStep.Order && !completedStepsList.Contains(s.Id));
                if (nextStep != null)
                {
                    await _configService.SetValueAsync("Onboarding:CurrentStep", nextStep.Id);
                }
                else
                {
                    // All steps completed
                    await CompleteOnboardingAsync();
                }
            }

            await _auditService.LogAsync("OnboardingStepCompleted", "Onboarding", stepId, 
                null, null, null, stepData);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing setup step: {StepId}", stepId);
            return false;
        }
    }

    public async Task<OnboardingWizardData> GetWizardDataAsync()
    {
        try
        {
            var wizardData = new OnboardingWizardData();

            // Load saved data from each step
            var stepDataTasks = new[]
            {
                LoadStepDataAsync<CompanySetup>("company"),
                LoadStepDataAsync<AdminUserSetup>("admin"),
                LoadStepDataAsync<DatabaseSetup>("database"),
                LoadStepDataAsync<SecuritySettings>("security"),
                LoadStepDataAsync<LanguageSettings>("language"),
                LoadStepDataAsync<NotificationSettings>("notifications")
            };

            var results = await Task.WhenAll(stepDataTasks);

            wizardData.Company = results[0] as CompanySetup ?? new CompanySetup();
            wizardData.AdminUser = results[1] as AdminUserSetup ?? new AdminUserSetup();
            wizardData.Database = results[2] as DatabaseSetup ?? new DatabaseSetup();
            wizardData.Security = results[3] as SecuritySettings ?? new SecuritySettings();
            wizardData.Language = results[4] as LanguageSettings ?? new LanguageSettings();
            wizardData.Notifications = results[5] as NotificationSettings ?? new NotificationSettings();

            return wizardData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting wizard data");
            return new OnboardingWizardData();
        }
    }

    public async Task<bool> CompleteOnboardingAsync()
    {
        try
        {
            _logger.LogInformation("Completing onboarding process");

            await _configService.SetValueAsync("Onboarding:Completed", "true");
            await _configService.SetValueAsync("Onboarding:CompletedAt", DateTime.UtcNow.ToString("O"));

            await _auditService.LogAsync("OnboardingCompleted", "System", null);

            _logger.LogInformation("Onboarding completed successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing onboarding");
            return false;
        }
    }

    public async Task<bool> ResetOnboardingAsync()
    {
        try
        {
            _logger.LogInformation("Resetting onboarding process");

            await _configService.SetValueAsync("Onboarding:Completed", "false");
            await _configService.DeleteAsync("Onboarding:CompletedAt");
            await _configService.DeleteAsync("Onboarding:CompletedSteps");
            await _configService.SetValueAsync("Onboarding:CurrentStep", "welcome");

            // Clear step data
            var steps = await GetOnboardingStepsAsync();
            foreach (var step in steps)
            {
                await _configService.DeleteAsync($"Onboarding:StepData:{step.Id}");
            }

            await _auditService.LogAsync("OnboardingReset", "System", null);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting onboarding");
            return false;
        }
    }

    public async Task<IEnumerable<OnboardingStep>> GetOnboardingStepsAsync()
    {
        await Task.Delay(1); // Make it async

        return new List<OnboardingStep>
        {
            new() { Id = "welcome", Title = "Welcome", Description = "Welcome to ASL LivingGrid", Order = 1, StepType = "info" },
            new() { Id = "company", Title = "Company Setup", Description = "Configure your company information", Order = 2, StepType = "form" },
            new() { Id = "admin", Title = "Admin User", Description = "Create the administrator account", Order = 3, StepType = "form" },
            new() { Id = "database", Title = "Database", Description = "Configure database settings", Order = 4, StepType = "form" },
            new() { Id = "security", Title = "Security", Description = "Set up security preferences", Order = 5, StepType = "form" },
            new() { Id = "language", Title = "Languages", Description = "Configure language settings", Order = 6, StepType = "form" },
            new() { Id = "notifications", Title = "Notifications", Description = "Set up notification preferences", Order = 7, StepType = "form", IsRequired = false },
            new() { Id = "completion", Title = "Complete", Description = "Finalize the setup", Order = 8, StepType = "confirmation" }
        };
    }

    private async Task<object?> LoadStepDataAsync<T>(string stepId) where T : new()
    {
        try
        {
            var stepDataJson = await _configService.GetValueAsync<string>($"Onboarding:StepData:{stepId}");
            if (!string.IsNullOrEmpty(stepDataJson))
            {
                return JsonSerializer.Deserialize<T>(stepDataJson);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading step data for {StepId}", stepId);
        }

        return new T();
    }

    private async Task ProcessStepDataAsync(string stepId, object stepData)
    {
        try
        {
            switch (stepId)
            {
                case "company":
                    await ProcessCompanySetupAsync(stepData as CompanySetup);
                    break;
                case "admin":
                    await ProcessAdminUserSetupAsync(stepData as AdminUserSetup);
                    break;
                case "database":
                    await ProcessDatabaseSetupAsync(stepData as DatabaseSetup);
                    break;
                case "security":
                    await ProcessSecuritySettingsAsync(stepData as SecuritySettings);
                    break;
                case "language":
                    await ProcessLanguageSettingsAsync(stepData as LanguageSettings);
                    break;
                case "notifications":
                    await ProcessNotificationSettingsAsync(stepData as NotificationSettings);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing step data for {StepId}", stepId);
            throw;
        }
    }

    private async Task ProcessCompanySetupAsync(CompanySetup? setup)
    {
        if (setup == null) return;

        // Check if company already exists
        var existingCompany = await _context.Companies.FirstOrDefaultAsync(c => c.Code == setup.Code);
        if (existingCompany == null)
        {
            var company = new Company
            {
                Name = setup.Name,
                Code = setup.Code,
                Description = setup.Description,
                Email = setup.Email,
                Phone = setup.Phone,
                Address = setup.Address,
                City = setup.City,
                Country = setup.Country,
                Website = setup.Website,
                CreatedAt = DateTime.UtcNow
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            await _configService.SetValueAsync("Application:DefaultCompanyId", company.Id.ToString());
        }
    }

    private async Task ProcessAdminUserSetupAsync(AdminUserSetup? setup)
    {
        if (setup == null) return;

        var existingUser = await _userManager.FindByEmailAsync(setup.Email);
        if (existingUser == null)
        {
            var user = new IdentityUser
            {
                UserName = setup.Email,
                Email = setup.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, setup.Password);
            if (result.Succeeded)
            {
                // Create AppUser record
                var defaultCompanyIdStr = await _configService.GetValueAsync<string>("Application:DefaultCompanyId");
                var defaultCompanyId = Guid.TryParse(defaultCompanyIdStr, out var id) ? id : (Guid?)null;

                var appUser = new AppUser
                {
                    FirstName = setup.FirstName,
                    LastName = setup.LastName,
                    Email = setup.Email,
                    Phone = setup.Phone,
                    CompanyId = defaultCompanyId,
                    PreferredLanguage = setup.PreferredLanguage,
                    CreatedAt = DateTime.UtcNow
                };

                _context.AppUsers.Add(appUser);
                await _context.SaveChangesAsync();
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to create admin user {Email}: {Errors}", setup.Email, errors);
            }
        }
    }

    private async Task ProcessDatabaseSetupAsync(DatabaseSetup? setup)
    {
        if (setup == null) return;

        await _configService.SetValueAsync("Database:ConnectionType", setup.ConnectionType);
        await _configService.SetValueAsync("Database:AutoBackup", setup.AutoBackup.ToString());
        await _configService.SetValueAsync("Database:BackupRetentionDays", setup.BackupRetentionDays.ToString());
    }

    private async Task ProcessSecuritySettingsAsync(SecuritySettings? setup)
    {
        if (setup == null) return;

        await _configService.SetValueAsync("Security:RequireHttps", setup.RequireHttps.ToString());
        await _configService.SetValueAsync("Security:EnableTwoFactorAuth", setup.EnableTwoFactorAuth.ToString());
        await _configService.SetValueAsync("Security:SessionTimeoutMinutes", setup.SessionTimeoutMinutes.ToString());
        await _configService.SetValueAsync("Security:EnableAuditLogging", setup.EnableAuditLogging.ToString());
        await _configService.SetValueAsync("Security:PasswordMinLength", setup.PasswordMinLength.ToString());
        await _configService.SetValueAsync("Security:RequirePasswordComplexity", setup.RequirePasswordComplexity.ToString());
        await _configService.SetValueAsync("Security:PasswordExpiryDays", setup.PasswordExpiryDays.ToString());
        await _configService.SetValueAsync("Security:SecretRotationDays", setup.SecretRotationDays.ToString());
        await _configService.SetValueAsync("Security:EnableJitPrivilegeElevation", setup.EnableJitPrivilegeElevation.ToString());
        await _configService.SetValueAsync("Security:EnforcePerTenantPolicy", setup.EnforcePerTenantPolicy.ToString());
    }

    private async Task ProcessLanguageSettingsAsync(LanguageSettings? setup)
    {
        if (setup == null) return;

        await _configService.SetValueAsync("Language:DefaultLanguage", setup.DefaultLanguage);
        await _configService.SetValueAsync("Language:EnabledLanguages", JsonSerializer.Serialize(setup.EnabledLanguages));
        await _configService.SetValueAsync("Language:AllowUserLanguageChange", setup.AllowUserLanguageChange.ToString());
    }

    private async Task ProcessNotificationSettingsAsync(NotificationSettings? setup)
    {
        if (setup == null) return;

        await _configService.SetValueAsync("Notifications:EnableEmailNotifications", setup.EnableEmailNotifications.ToString());
        await _configService.SetValueAsync("Notifications:EnableSystemNotifications", setup.EnableSystemNotifications.ToString());
        await _configService.SetValueAsync("Notifications:SmtpServer", setup.SmtpServer);
        await _configService.SetValueAsync("Notifications:SmtpPort", setup.SmtpPort.ToString());
        await _configService.SetValueAsync("Notifications:SmtpUsername", setup.SmtpUsername);
        await _configService.SetValueAsync("Notifications:SmtpPassword", setup.SmtpPassword);
        await _configService.SetValueAsync("Notifications:SmtpUseSsl", setup.SmtpUseSsl.ToString());
        await _configService.SetValueAsync("Notifications:EnableSmsNotifications", setup.EnableSmsNotifications.ToString());
        await _configService.SetValueAsync("Notifications:SmsApiUrl", setup.SmsApiUrl);
        await _configService.SetValueAsync("Notifications:SmsApiKey", setup.SmsApiKey);
        await _configService.SetValueAsync("Notifications:EnableTelegramNotifications", setup.EnableTelegramNotifications.ToString());
        await _configService.SetValueAsync("Notifications:TelegramBotToken", setup.TelegramBotToken);
        await _configService.SetValueAsync("Notifications:TelegramChatId", setup.TelegramChatId);
        await _configService.SetValueAsync("Notifications:EnableWebhookNotifications", setup.EnableWebhookNotifications.ToString());
        await _configService.SetValueAsync("Notifications:WebhookUrl", setup.WebhookUrl);
    }
}
