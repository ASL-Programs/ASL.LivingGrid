using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class EnvironmentProvisioningService : IEnvironmentProvisioningService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EnvironmentProvisioningService> _logger;
    private readonly IConfigurationService _configService;
    private readonly ApplicationDbContext _context;
    private readonly string _environmentsPath;

    public EnvironmentProvisioningService(
        IConfiguration configuration,
        ILogger<EnvironmentProvisioningService> logger,
        IConfigurationService configService,
        ApplicationDbContext context)
    {
        _configuration = configuration;
        _logger = logger;
        _configService = configService;
        _context = context;
        _environmentsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Environments");
        
        // Ensure environments directory exists
        Directory.CreateDirectory(_environmentsPath);
    }

    public async Task<EnvironmentProvisioningWizardState> GetWizardStateAsync()
    {
        try
        {
            var isCompleted = await _configService.GetValueAsync<bool>("EnvironmentProvisioning:Completed");
            var currentStep = await _configService.GetValueAsync<int>("EnvironmentProvisioning:CurrentStep");
            var currentEnvironment = await _configService.GetValueAsync<string>("EnvironmentProvisioning:CurrentEnvironment") ?? "Development";
            
            var existingEnvironments = await GetExistingEnvironmentsAsync();

            var steps = new List<WizardStep>
            {
                new()
                {
                    StepNumber = 1,
                    Title = "Environment Selection",
                    Description = "Choose environment type and template",
                    IsCompleted = currentStep > 1,
                    IsActive = currentStep == 1
                },
                new()
                {
                    StepNumber = 2,
                    Title = "Database Configuration",
                    Description = "Configure database settings",
                    IsCompleted = currentStep > 2,
                    IsActive = currentStep == 2
                },
                new()
                {
                    StepNumber = 3,
                    Title = "Network & Security",
                    Description = "Setup network and security configuration",
                    IsCompleted = currentStep > 3,
                    IsActive = currentStep == 3
                },
                new()
                {
                    StepNumber = 4,
                    Title = "Features & Integration",
                    Description = "Configure features and external integrations",
                    IsCompleted = currentStep > 4,
                    IsActive = currentStep == 4
                },
                new()
                {
                    StepNumber = 5,
                    Title = "Review & Deploy",
                    Description = "Review configuration and deploy environment",
                    IsCompleted = isCompleted,
                    IsActive = currentStep == 5
                }
            };

            return new EnvironmentProvisioningWizardState
            {
                IsCompleted = isCompleted,
                CurrentStep = Math.Max(1, currentStep),
                TotalSteps = 5,
                CurrentEnvironment = currentEnvironment,
                Steps = steps,
                HasExistingEnvironments = existingEnvironments.Any()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting wizard state");
            return new EnvironmentProvisioningWizardState();
        }
    }

    public async Task<bool> InitializeEnvironmentAsync(EnvironmentSetupOptions options)
    {
        try
        {
            _logger.LogInformation("Initializing environment: {EnvironmentName}", options.EnvironmentName);

            // Validate options
            var validationResult = await ValidateEnvironmentConfigurationAsync(options);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Environment validation failed: {Issues}", 
                    string.Join(", ", validationResult.Issues.Select(i => i.Message)));
                return false;
            }

            // Create environment directory
            var envPath = Path.Combine(_environmentsPath, options.EnvironmentName);
            Directory.CreateDirectory(envPath);

            // Save environment configuration
            var configPath = Path.Combine(envPath, "environment.json");
            var configJson = JsonSerializer.Serialize(options, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            await File.WriteAllTextAsync(configPath, configJson);

            // Create environment-specific configuration files
            await CreateEnvironmentConfigurationFilesAsync(envPath, options);

            // Update wizard state
            await _configService.SetValueAsync("EnvironmentProvisioning:CurrentStep", 2);

            _logger.LogInformation("Environment initialized successfully: {EnvironmentName}", options.EnvironmentName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing environment: {EnvironmentName}", options.EnvironmentName);
            return false;
        }
    }

    public async Task<IEnumerable<EnvironmentTemplate>> GetAvailableTemplatesAsync()
    {
        await Task.Delay(1); // Make it async

        return new List<EnvironmentTemplate>
        {
            new()
            {
                Id = "development",
                Name = "Development",
                Description = "Development environment with debugging enabled and relaxed security",
                DefaultType = EnvironmentType.Development,
                IconClass = "fas fa-code",
                DefaultOptions = new EnvironmentSetupOptions
                {
                    EnvironmentType = EnvironmentType.Development,
                    Database = new DatabaseConfiguration
                    {
                        DatabaseType = "sqlite",
                        SeedDemoData = true,
                        EnableBackup = false
                    },
                    Network = new NetworkConfiguration
                    {
                        EnableHttps = false,
                        RequireHttps = false
                    },
                    Security = new SecurityConfiguration
                    {
                        RequireHttps = false,
                        EnablePasswordPolicy = false,
                        SessionTimeoutMinutes = 60
                    },
                    Logging = new LoggingConfiguration
                    {
                        LogLevel = "Debug",
                        EnableConsoleLogging = true
                    }
                }
            },
            new()
            {
                Id = "testing",
                Name = "Testing/QA",
                Description = "Testing environment for quality assurance with test data",
                DefaultType = EnvironmentType.Testing,
                IconClass = "fas fa-flask",
                DefaultOptions = new EnvironmentSetupOptions
                {
                    EnvironmentType = EnvironmentType.Testing,
                    Database = new DatabaseConfiguration
                    {
                        DatabaseType = "sqlserver",
                        SeedDemoData = true,
                        EnableBackup = true,
                        BackupSchedule = "Daily"
                    },
                    Security = new SecurityConfiguration
                    {
                        EnablePasswordPolicy = true,
                        SessionTimeoutMinutes = 30
                    },
                    Features = new FeatureConfiguration
                    {
                        EnableAuditLogging = true
                    }
                }
            },
            new()
            {
                Id = "production",
                Name = "Production",
                Description = "Production environment with enhanced security and monitoring",
                DefaultType = EnvironmentType.Production,
                IconClass = "fas fa-server",
                DefaultOptions = new EnvironmentSetupOptions
                {
                    EnvironmentType = EnvironmentType.Production,
                    Database = new DatabaseConfiguration
                    {
                        DatabaseType = "sqlserver",
                        SeedDemoData = false,
                        EnableBackup = true,
                        BackupSchedule = "Hourly"
                    },
                    Network = new NetworkConfiguration
                    {
                        EnableHttps = true,
                        RequireHttps = true
                    },
                    Security = new SecurityConfiguration
                    {
                        RequireHttps = true,
                        EnablePasswordPolicy = true,
                        EnableTwoFactorAuth = true,
                        SessionTimeoutMinutes = 15
                    },
                    Features = new FeatureConfiguration
                    {
                        EnableAuditLogging = true
                    },
                    Logging = new LoggingConfiguration
                    {
                        LogLevel = "Warning",
                        EnableDatabaseLogging = true,
                        RetentionDays = 90
                    }
                }
            },
            new()
            {
                Id = "demo",
                Name = "Demo/Training",
                Description = "Demo environment for presentations and training",
                DefaultType = EnvironmentType.Demo,
                IconClass = "fas fa-presentation",
                DefaultOptions = new EnvironmentSetupOptions
                {
                    EnvironmentType = EnvironmentType.Demo,
                    Database = new DatabaseConfiguration
                    {
                        DatabaseType = "sqlite",
                        SeedDemoData = true,
                        EnableBackup = false
                    },
                    Security = new SecurityConfiguration
                    {
                        RequireHttps = false,
                        EnablePasswordPolicy = false,
                        SessionTimeoutMinutes = 120
                    }
                }
            }
        };
    }

    public async Task<EnvironmentValidationResult> ValidateEnvironmentConfigurationAsync(EnvironmentSetupOptions options)
    {
        var result = new EnvironmentValidationResult();
        var issues = new List<ValidationIssue>();
        var warnings = new List<string>();
        var recommendations = new List<string>();

        try
        {
            // Validate environment name
            if (string.IsNullOrWhiteSpace(options.EnvironmentName))
            {
                issues.Add(new ValidationIssue
                {
                    Code = "ENV_NAME_REQUIRED",
                    Message = "Environment name is required",
                    Field = "EnvironmentName",
                    Severity = ValidationSeverity.Error,
                    SuggestedFix = "Provide a valid environment name"
                });
            }
            else if (options.EnvironmentName.Length < 3)
            {
                issues.Add(new ValidationIssue
                {
                    Code = "ENV_NAME_TOO_SHORT",
                    Message = "Environment name must be at least 3 characters",
                    Field = "EnvironmentName",
                    Severity = ValidationSeverity.Error
                });
            }

            // Check if environment already exists
            var existingEnvironments = await GetExistingEnvironmentsAsync();
            if (existingEnvironments.Any(e => e.Name.Equals(options.EnvironmentName, StringComparison.OrdinalIgnoreCase)))
            {
                issues.Add(new ValidationIssue
                {
                    Code = "ENV_ALREADY_EXISTS",
                    Message = "Environment with this name already exists",
                    Field = "EnvironmentName",
                    Severity = ValidationSeverity.Error,
                    SuggestedFix = "Choose a different environment name"
                });
            }

            // Validate database configuration
            await ValidateDatabaseConfigurationAsync(options.Database, issues, warnings, recommendations);

            // Validate network configuration
            ValidateNetworkConfiguration(options.Network, issues, warnings, recommendations);

            // Validate security configuration
            ValidateSecurityConfiguration(options.Security, issues, warnings, recommendations);

            // Environment-specific validations
            ValidateEnvironmentSpecificSettings(options, issues, warnings, recommendations);

            result.IsValid = !issues.Any(i => i.Severity == ValidationSeverity.Error || i.Severity == ValidationSeverity.Critical);
            result.Issues = issues;
            result.Warnings = warnings;
            result.Recommendations = recommendations;

            _logger.LogInformation("Environment validation completed. Valid: {IsValid}, Issues: {IssueCount}", 
                result.IsValid, issues.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during environment validation");
            issues.Add(new ValidationIssue
            {
                Code = "VALIDATION_ERROR",
                Message = $"Validation error: {ex.Message}",
                Severity = ValidationSeverity.Critical
            });
            result.IsValid = false;
            result.Issues = issues;
        }

        return result;
    }

    public async Task<bool> ProvisionEnvironmentAsync(EnvironmentSetupOptions options)
    {
        try
        {
            _logger.LogInformation("Provisioning environment: {EnvironmentName}", options.EnvironmentName);

            // Initialize environment
            if (!await InitializeEnvironmentAsync(options))
            {
                return false;
            }

            // Setup database
            if (!await SetupDatabaseAsync(options))
            {
                return false;
            }

            // Configure network settings
            await ConfigureNetworkSettingsAsync(options);

            // Apply security configuration
            await ApplySecurityConfigurationAsync(options);

            // Enable features
            await ConfigureFeaturesAsync(options);

            // Setup logging
            await ConfigureLoggingAsync(options);

            // Setup integrations
            await ConfigureIntegrationsAsync(options);

            // Mark as primary if specified
            if (options.IsPrimary)
            {
                await SetPrimaryEnvironmentAsync(options.EnvironmentName);
            }

            // Complete wizard
            await _configService.SetValueAsync("EnvironmentProvisioning:Completed", true);
            await _configService.SetValueAsync("EnvironmentProvisioning:CurrentEnvironment", options.EnvironmentName);

            _logger.LogInformation("Environment provisioned successfully: {EnvironmentName}", options.EnvironmentName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error provisioning environment: {EnvironmentName}", options.EnvironmentName);
            return false;
        }
    }

    public async Task<bool> SwitchEnvironmentAsync(string environmentName)
    {
        try
        {
            var environments = await GetExistingEnvironmentsAsync();
            var targetEnvironment = environments.FirstOrDefault(e => e.Name.Equals(environmentName, StringComparison.OrdinalIgnoreCase));
            
            if (targetEnvironment == null)
            {
                _logger.LogWarning("Environment not found: {EnvironmentName}", environmentName);
                return false;
            }

            // Load environment configuration
            var envPath = Path.Combine(_environmentsPath, environmentName);
            var configPath = Path.Combine(envPath, "environment.json");
            
            if (!File.Exists(configPath))
            {
                _logger.LogWarning("Environment configuration not found: {ConfigPath}", configPath);
                return false;
            }

            var configJson = await File.ReadAllTextAsync(configPath);
            var environmentConfig = JsonSerializer.Deserialize<EnvironmentSetupOptions>(configJson);

            if (environmentConfig != null)
            {
                // Apply environment configuration
                await ApplyEnvironmentConfigurationAsync(environmentName, environmentConfig.CustomSettings);
                await _configService.SetValueAsync("EnvironmentProvisioning:CurrentEnvironment", environmentName);
                
                _logger.LogInformation("Switched to environment: {EnvironmentName}", environmentName);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error switching to environment: {EnvironmentName}", environmentName);
            return false;
        }
    }

    public async Task<IEnumerable<EnvironmentInfo>> GetExistingEnvironmentsAsync()
    {
        try
        {
            var environments = new List<EnvironmentInfo>();
            
            if (!Directory.Exists(_environmentsPath))
            {
                return environments;
            }

            var envDirectories = Directory.GetDirectories(_environmentsPath);
            
            foreach (var envDir in envDirectories)
            {
                var envName = Path.GetFileName(envDir);
                var configPath = Path.Combine(envDir, "environment.json");
                
                if (File.Exists(configPath))
                {
                    try
                    {
                        var configJson = await File.ReadAllTextAsync(configPath);
                        var config = JsonSerializer.Deserialize<EnvironmentSetupOptions>(configJson);
                        
                        if (config != null)
                        {
                            var envInfo = new EnvironmentInfo
                            {
                                Name = envName,
                                Type = config.EnvironmentType,
                                Description = config.Description,
                                IsActive = await IsEnvironmentActiveAsync(envName),
                                IsPrimary = config.IsPrimary,
                                CreatedAt = Directory.GetCreationTime(envDir),
                                LastModified = Directory.GetLastWriteTime(envDir),
                                Health = await CheckEnvironmentHealthAsync(envName),
                                Configuration = config.CustomSettings
                            };
                            
                            environments.Add(envInfo);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error reading environment config: {ConfigPath}", configPath);
                    }
                }
            }

            return environments.OrderBy(e => e.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting existing environments");
            return new List<EnvironmentInfo>();
        }
    }

    public async Task<bool> CloneEnvironmentAsync(string sourceEnvironment, string targetEnvironment)
    {
        try
        {
            var sourcePath = Path.Combine(_environmentsPath, sourceEnvironment);
            var targetPath = Path.Combine(_environmentsPath, targetEnvironment);

            if (!Directory.Exists(sourcePath))
            {
                _logger.LogWarning("Source environment not found: {SourceEnvironment}", sourceEnvironment);
                return false;
            }

            if (Directory.Exists(targetPath))
            {
                _logger.LogWarning("Target environment already exists: {TargetEnvironment}", targetEnvironment);
                return false;
            }

            // Copy directory structure
            await CopyDirectoryAsync(sourcePath, targetPath);

            // Update environment configuration
            var configPath = Path.Combine(targetPath, "environment.json");
            if (File.Exists(configPath))
            {
                var configJson = await File.ReadAllTextAsync(configPath);
                var config = JsonSerializer.Deserialize<EnvironmentSetupOptions>(configJson);
                
                if (config != null)
                {
                    config.EnvironmentName = targetEnvironment;
                    config.IsPrimary = false; // Cloned environments are never primary by default
                    
                    var updatedJson = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                    await File.WriteAllTextAsync(configPath, updatedJson);
                }
            }

            _logger.LogInformation("Environment cloned successfully: {SourceEnvironment} -> {TargetEnvironment}", 
                sourceEnvironment, targetEnvironment);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cloning environment: {SourceEnvironment} -> {TargetEnvironment}", 
                sourceEnvironment, targetEnvironment);
            return false;
        }
    }

    public async Task<bool> DeleteEnvironmentAsync(string environmentName)
    {
        try
        {
            var envPath = Path.Combine(_environmentsPath, environmentName);
            
            if (!Directory.Exists(envPath))
            {
                _logger.LogWarning("Environment not found: {EnvironmentName}", environmentName);
                return false;
            }

            // Check if it's the current environment
            var currentEnvironment = await _configService.GetValueAsync<string>("EnvironmentProvisioning:CurrentEnvironment");
            if (string.Equals(currentEnvironment, environmentName, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Cannot delete active environment: {EnvironmentName}", environmentName);
                return false;
            }

            // Create backup before deletion
            var backupPath = Path.Combine(_environmentsPath, $"{environmentName}_backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}");
            await CopyDirectoryAsync(envPath, backupPath);

            // Delete environment
            Directory.Delete(envPath, true);

            _logger.LogInformation("Environment deleted successfully: {EnvironmentName}", environmentName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting environment: {EnvironmentName}", environmentName);
            return false;
        }
    }

    public async Task<EnvironmentHealth> CheckEnvironmentHealthAsync(string environmentName)
    {
        var health = new EnvironmentHealth
        {
            LastChecked = DateTime.UtcNow,
            Checks = new List<HealthCheck>()
        };

        try
        {
            // Check if environment exists
            var envPath = Path.Combine(_environmentsPath, environmentName);
            var configExists = File.Exists(Path.Combine(envPath, "environment.json"));
            
            health.Checks.Add(new HealthCheck
            {
                Name = "Configuration File",
                IsHealthy = configExists,
                Message = configExists ? "Configuration file exists" : "Configuration file missing",
                CheckedAt = DateTime.UtcNow
            });

            // Check database connectivity if this is the active environment
            var currentEnvironment = await _configService.GetValueAsync<string>("EnvironmentProvisioning:CurrentEnvironment");
            if (string.Equals(currentEnvironment, environmentName, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var canConnect = await _context.Database.CanConnectAsync();
                    health.Checks.Add(new HealthCheck
                    {
                        Name = "Database Connection",
                        IsHealthy = canConnect,
                        Message = canConnect ? "Database is accessible" : "Database connection failed",
                        CheckedAt = DateTime.UtcNow
                    });
                }
                catch (Exception ex)
                {
                    health.Checks.Add(new HealthCheck
                    {
                        Name = "Database Connection",
                        IsHealthy = false,
                        Message = $"Database error: {ex.Message}",
                        CheckedAt = DateTime.UtcNow
                    });
                }
            }

            health.IsHealthy = health.Checks.All(c => c.IsHealthy);
            health.Status = health.IsHealthy ? "Healthy" : "Unhealthy";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking environment health: {EnvironmentName}", environmentName);
            health.IsHealthy = false;
            health.Status = "Error";
        }

        return health;
    }

    public async Task<bool> ApplyEnvironmentConfigurationAsync(string environmentName, Dictionary<string, object> configuration)
    {
        try
        {
            foreach (var setting in configuration)
            {
                await _configService.SetValueAsync($"Environment:{environmentName}:{setting.Key}", setting.Value?.ToString() ?? "");
            }

            _logger.LogInformation("Applied configuration for environment: {EnvironmentName}", environmentName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying environment configuration: {EnvironmentName}", environmentName);
            return false;
        }
    }

    // Private helper methods
    private async Task CreateEnvironmentConfigurationFilesAsync(string envPath, EnvironmentSetupOptions options)
    {
        // Create appsettings.environment.json
        var appSettings = new
        {
            ConnectionStrings = new
            {
                DefaultConnection = options.Database.ConnectionString
            },
            Logging = new
            {
                LogLevel = new
                {
                    Default = options.Logging.LogLevel
                }
            },
            Network = options.Network,
            Security = options.Security,
            Features = options.Features
        };

        var appSettingsPath = Path.Combine(envPath, "appsettings.json");
        var appSettingsJson = JsonSerializer.Serialize(appSettings, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(appSettingsPath, appSettingsJson);
    }

    private async Task ValidateDatabaseConfigurationAsync(DatabaseConfiguration database, 
        List<ValidationIssue> issues, List<string> warnings, List<string> recommendations)
    {
        await Task.Delay(1); // Make it async

        if (string.IsNullOrWhiteSpace(database.DatabaseType))
        {
            issues.Add(new ValidationIssue
            {
                Code = "DB_TYPE_REQUIRED",
                Message = "Database type is required",
                Field = "Database.DatabaseType",
                Severity = ValidationSeverity.Error
            });
        }

        if (database.DatabaseType == "sqlserver" && string.IsNullOrWhiteSpace(database.ConnectionString))
        {
            warnings.Add("SQL Server connection string not provided. Default connection will be used.");
        }

        if (database.SeedDemoData)
        {
            recommendations.Add("Consider disabling demo data for production environments.");
        }
    }

    private void ValidateNetworkConfiguration(NetworkConfiguration network, 
        List<ValidationIssue> issues, List<string> warnings, List<string> recommendations)
    {
        if (!int.TryParse(network.HttpPort, out var httpPort) || httpPort < 1 || httpPort > 65535)
        {
            issues.Add(new ValidationIssue
            {
                Code = "INVALID_HTTP_PORT",
                Message = "Invalid HTTP port number",
                Field = "Network.HttpPort",
                Severity = ValidationSeverity.Error
            });
        }

        if (!int.TryParse(network.HttpsPort, out var httpsPort) || httpsPort < 1 || httpsPort > 65535)
        {
            issues.Add(new ValidationIssue
            {
                Code = "INVALID_HTTPS_PORT",
                Message = "Invalid HTTPS port number",
                Field = "Network.HttpsPort",
                Severity = ValidationSeverity.Error
            });
        }

        if (httpPort == httpsPort)
        {
            issues.Add(new ValidationIssue
            {
                Code = "PORTS_CONFLICT",
                Message = "HTTP and HTTPS ports cannot be the same",
                Severity = ValidationSeverity.Error
            });
        }

        if (!network.EnableHttps)
        {
            warnings.Add("HTTPS is disabled. Consider enabling HTTPS for better security.");
        }
    }

    private void ValidateSecurityConfiguration(SecurityConfiguration security, 
        List<ValidationIssue> issues, List<string> warnings, List<string> recommendations)
    {
        if (security.SessionTimeoutMinutes < 5)
        {
            warnings.Add("Session timeout is very short. This may impact user experience.");
        }

        if (security.SessionTimeoutMinutes > 480) // 8 hours
        {
            warnings.Add("Session timeout is very long. This may impact security.");
        }

        if (!security.EnablePasswordPolicy)
        {
            recommendations.Add("Consider enabling password policy for better security.");
        }

        if (!security.EnableAuthentication)
        {
            warnings.Add("Authentication is disabled. This may be a security risk.");
        }
    }

    private void ValidateEnvironmentSpecificSettings(EnvironmentSetupOptions options, 
        List<ValidationIssue> issues, List<string> warnings, List<string> recommendations)
    {
        switch (options.EnvironmentType)
        {
            case EnvironmentType.Production:
                if (!options.Security.RequireHttps)
                {
                    warnings.Add("HTTPS should be required for production environments.");
                }
                if (!options.Features.EnableAuditLogging)
                {
                    recommendations.Add("Enable audit logging for production environments.");
                }
                break;

            case EnvironmentType.Development:
                if (options.Security.RequireHttps)
                {
                    recommendations.Add("HTTPS requirement can be relaxed for development environments.");
                }
                break;
        }
    }

    private async Task<bool> SetupDatabaseAsync(EnvironmentSetupOptions options)
    {
        try
        {
            if (options.Database.CreateDatabase)
            {
                await _context.Database.EnsureCreatedAsync();
            }

            if (options.Database.RunMigrations)
            {
                await _context.Database.MigrateAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting up database");
            return false;
        }
    }

    private async Task ConfigureNetworkSettingsAsync(EnvironmentSetupOptions options)
    {
        await _configService.SetValueAsync("Network:HttpPort", options.Network.HttpPort);
        await _configService.SetValueAsync("Network:HttpsPort", options.Network.HttpsPort);
        await _configService.SetValueAsync("Network:EnableHttps", options.Network.EnableHttps.ToString());
        await _configService.SetValueAsync("Network:RequireHttps", options.Network.RequireHttps.ToString());
    }

    private async Task ApplySecurityConfigurationAsync(EnvironmentSetupOptions options)
    {
        await _configService.SetValueAsync("Security:RequireHttps", options.Security.RequireHttps.ToString());
        await _configService.SetValueAsync("Security:SessionTimeoutMinutes", options.Security.SessionTimeoutMinutes.ToString());
        await _configService.SetValueAsync("Security:EnablePasswordPolicy", options.Security.EnablePasswordPolicy.ToString());
    }

    private async Task ConfigureFeaturesAsync(EnvironmentSetupOptions options)
    {
        await _configService.SetValueAsync("Features:EnableTrayIcon", options.Features.EnableTrayIcon.ToString());
        await _configService.SetValueAsync("Features:EnableNotifications", options.Features.EnableNotifications.ToString());
        await _configService.SetValueAsync("Features:EnableAuditLogging", options.Features.EnableAuditLogging.ToString());
    }

    private async Task ConfigureLoggingAsync(EnvironmentSetupOptions options)
    {
        await _configService.SetValueAsync("Logging:LogLevel", options.Logging.LogLevel);
        await _configService.SetValueAsync("Logging:EnableFileLogging", options.Logging.EnableFileLogging.ToString());
        await _configService.SetValueAsync("Logging:RetentionDays", options.Logging.RetentionDays.ToString());
    }

    private async Task ConfigureIntegrationsAsync(EnvironmentSetupOptions options)
    {
        await _configService.SetValueAsync("Integrations:EnableWebhooks", options.Integrations.EnableWebhooks.ToString());
        await _configService.SetValueAsync("Integrations:EnableApiAccess", options.Integrations.EnableApiAccess.ToString());
    }

    private async Task SetPrimaryEnvironmentAsync(string environmentName)
    {
        await _configService.SetValueAsync("Environment:Primary", environmentName);
    }

    private async Task<bool> IsEnvironmentActiveAsync(string environmentName)
    {
        var currentEnvironment = await _configService.GetValueAsync<string>("EnvironmentProvisioning:CurrentEnvironment");
        return string.Equals(currentEnvironment, environmentName, StringComparison.OrdinalIgnoreCase);
    }

    private async Task CopyDirectoryAsync(string sourcePath, string targetPath)
    {
        await Task.Run(() =>
        {
            Directory.CreateDirectory(targetPath);

            foreach (var file in Directory.GetFiles(sourcePath))
            {
                var targetFile = Path.Combine(targetPath, Path.GetFileName(file));
                File.Copy(file, targetFile, true);
            }

            foreach (var dir in Directory.GetDirectories(sourcePath))
            {
                var targetDir = Path.Combine(targetPath, Path.GetFileName(dir));
                CopyDirectoryAsync(dir, targetDir).Wait();
            }
        });
    }
}
