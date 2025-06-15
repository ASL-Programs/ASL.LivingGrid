using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Management;
using System.Security.Principal;
using System.Text.Json;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class InstallerService : IInstallerService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<InstallerService> _logger;
    private readonly IConfigurationService _configService;
    private readonly ApplicationDbContext _context;
    private readonly string _installationPath;
    private static InstallationProgress? _currentProgress;

    public InstallerService(
        IConfiguration configuration,
        ILogger<InstallerService> logger,
        IConfigurationService configService,
        ApplicationDbContext context)
    {
        _configuration = configuration;
        _logger = logger;
        _configService = configService;
        _context = context;
        _installationPath = AppDomain.CurrentDomain.BaseDirectory;
    }

    public async Task<InstallationStatus> GetInstallationStatusAsync()
    {
        try
        {
            var isInstalled = await _configService.GetValueAsync<bool>("Installation:Completed");
            var installationDate = await _configService.GetValueAsync<DateTime?>("Installation:Date");
            var version = await _configService.GetValueAsync<string>("Installation:Version") ?? "1.0.0";
            
            var components = await GetRequiredComponentsAsync();
            var installedComponents = components.Where(c => c.IsInstalled).Select(c => c.Id).ToList();
            var missingComponents = components.Where(c => !c.IsInstalled && c.IsRequired).Select(c => c.Id).ToList();

            var healthCheck = await PerformHealthCheckAsync();

            return new InstallationStatus
            {
                IsInstalled = isInstalled,
                IsHealthy = healthCheck.IsHealthy,
                InstallationDate = installationDate,
                Version = version,
                InstallationPath = _installationPath,
                InstalledComponents = installedComponents,
                MissingComponents = missingComponents,
                Issues = healthCheck.Issues
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting installation status");
            return new InstallationStatus();
        }
    }

    public async Task<bool> PerformInstallationAsync(InstallationOptions options)
    {
        try
        {
            _logger.LogInformation("Starting installation with options: {@Options}", options);

            _currentProgress = new InstallationProgress
            {
                IsInProgress = true,
                StartTime = DateTime.UtcNow,
                CurrentOperation = "Preparing installation..."
            };

            // Step 1: Validate system requirements
            UpdateProgress(10, "Validating system requirements...");
            if (!await ValidateSystemRequirementsAsync())
            {
                throw new InvalidOperationException("System requirements not met");
            }

            // Step 2: Backup existing data if requested
            if (options.BackupExistingData)
            {
                UpdateProgress(20, "Creating backup...");
                await CreateInstallationBackupAsync();
            }

            // Step 3: Install components
            UpdateProgress(30, "Installing components...");
            await InstallComponentsAsync(options.ComponentsToInstall);

            // Step 4: Configure database
            UpdateProgress(50, "Configuring database...");
            await ConfigureDatabaseAsync(options);

            // Step 5: Create shortcuts
            if (options.CreateDesktopShortcut || options.CreateStartMenuShortcut)
            {
                UpdateProgress(70, "Creating shortcuts...");
                await CreateShortcutsAsync(options);
            }

            // Step 6: Configure service
            if (options.InstallAsService)
            {
                UpdateProgress(80, "Installing as Windows service...");
                await InstallAsServiceAsync();
            }

            // Step 7: Configure firewall
            if (options.CreateFirewallRules)
            {
                UpdateProgress(90, "Configuring firewall rules...");
                await ConfigureFirewallAsync();
            }

            // Step 8: Finalize installation
            UpdateProgress(95, "Finalizing installation...");
            await FinalizeInstallationAsync(options);

            UpdateProgress(100, "Installation completed successfully!");
            _currentProgress.IsInProgress = false;

            _logger.LogInformation("Installation completed successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Installation failed");
            if (_currentProgress != null)
            {
                _currentProgress.IsInProgress = false;
                _currentProgress.ErrorMessage = ex.Message;
            }
            return false;
        }
    }

    public async Task<bool> RepairInstallationAsync()
    {
        try
        {
            _logger.LogInformation("Starting installation repair");

            _currentProgress = new InstallationProgress
            {
                IsInProgress = true,
                StartTime = DateTime.UtcNow,
                CurrentOperation = "Analyzing installation..."
            };

            // Check for issues
            UpdateProgress(20, "Performing health check...");
            var healthCheck = await PerformHealthCheckAsync();

            // Attempt auto-fix for each issue
            UpdateProgress(40, "Fixing detected issues...");
            foreach (var issue in healthCheck.Issues.Where(i => i.CanAutoFix))
            {
                await FixIssueAsync(issue);
            }

            // Reinstall missing components
            UpdateProgress(70, "Reinstalling missing components...");
            var status = await GetInstallationStatusAsync();
            if (status.MissingComponents.Any())
            {
                await InstallComponentsAsync(status.MissingComponents);
            }

            // Verify repair
            UpdateProgress(90, "Verifying repair...");
            var postRepairCheck = await PerformHealthCheckAsync();

            UpdateProgress(100, "Repair completed!");
            _currentProgress.IsInProgress = false;

            _logger.LogInformation("Installation repair completed");
            return postRepairCheck.IsHealthy;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Installation repair failed");
            if (_currentProgress != null)
            {
                _currentProgress.IsInProgress = false;
                _currentProgress.ErrorMessage = ex.Message;
            }
            return false;
        }
    }

    public async Task<bool> UninstallAsync()
    {
        try
        {
            _logger.LogInformation("Starting uninstallation");

            // Create final backup
            await CreateInstallationBackupAsync();

            // Remove shortcuts
            await RemoveShortcutsAsync();

            // Uninstall service
            await UninstallServiceAsync();

            // Remove firewall rules
            await RemoveFirewallRulesAsync();

            // Clear configuration
            await _configService.SetValueAsync("Installation:Completed", "false");

            _logger.LogInformation("Uninstallation completed");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Uninstallation failed");
            return false;
        }
    }

    public async Task<SystemHealthCheck> PerformHealthCheckAsync()
    {
        var healthCheck = new SystemHealthCheck();
        var issues = new List<SystemIssue>();

        try
        {
            // Check database connectivity
            try
            {
                await _context.Database.CanConnectAsync();
            }
            catch (Exception ex)
            {
                issues.Add(new SystemIssue
                {
                    Id = "database_connection",
                    Title = "Database Connection Failed",
                    Description = ex.Message,
                    Severity = IssueSeverity.Critical,
                    CanAutoFix = true,
                    FixAction = "Recreate database connection"
                });
            }

            // Check required files
            var requiredFiles = new[] { "appsettings.json", "ASL.LivingGrid.WebAdminPanel.exe" };
            foreach (var file in requiredFiles)
            {
                var filePath = Path.Combine(_installationPath, file);
                if (!File.Exists(filePath))
                {
                    issues.Add(new SystemIssue
                    {
                        Id = $"missing_file_{file}",
                        Title = $"Missing File: {file}",
                        Description = $"Required file {file} is missing",
                        Severity = IssueSeverity.Error,
                        CanAutoFix = false
                    });
                }
            }

            // Check permissions
            if (!HasWritePermission(_installationPath))
            {
                issues.Add(new SystemIssue
                {
                    Id = "insufficient_permissions",
                    Title = "Insufficient Permissions",
                    Description = "Application lacks write permissions to installation directory",
                    Severity = IssueSeverity.Warning,
                    CanAutoFix = false
                });
            }

            // Check system resources
            await CheckSystemResourcesAsync(issues);

            // Collect system info
            healthCheck.SystemInfo = await CollectSystemInfoAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during health check");
            issues.Add(new SystemIssue
            {
                Id = "health_check_error",
                Title = "Health Check Error",
                Description = ex.Message,
                Severity = IssueSeverity.Error,
                CanAutoFix = false
            });
        }

        healthCheck.Issues = issues;
        healthCheck.IsHealthy = !issues.Any(i => i.Severity == IssueSeverity.Critical || i.Severity == IssueSeverity.Error);

        return healthCheck;
    }

    public async Task<bool> SelfHealAsync()
    {
        try
        {
            _logger.LogInformation("Starting self-healing process");

            var healthCheck = await PerformHealthCheckAsync();
            var fixedIssues = 0;

            foreach (var issue in healthCheck.Issues.Where(i => i.CanAutoFix))
            {
                if (await FixIssueAsync(issue))
                {
                    fixedIssues++;
                }
            }

            _logger.LogInformation("Self-healing completed. Fixed {FixedCount} issues", fixedIssues);
            return fixedIssues > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Self-healing failed");
            return false;
        }
    }

    public async Task<IEnumerable<InstallationComponent>> GetRequiredComponentsAsync()
    {
        await Task.Delay(1); // Make it async

        return new List<InstallationComponent>
        {
            new()
            {
                Id = "core_application",
                Name = "Core Application",
                Description = "Main application files",
                IsRequired = true,
                IsInstalled = File.Exists(Path.Combine(_installationPath, "ASL.LivingGrid.WebAdminPanel.exe")),
                Version = "1.0.0"
            },
            new()
            {
                Id = "database",
                Name = "Database",
                Description = "Application database",
                IsRequired = true,
                IsInstalled = await CheckDatabaseAsync(),
                Version = "1.0.0"
            },
            new()
            {
                Id = "configuration",
                Name = "Configuration",
                Description = "Application configuration files",
                IsRequired = true,
                IsInstalled = File.Exists(Path.Combine(_installationPath, "appsettings.json")),
                Version = "1.0.0"
            },
            new()
            {
                Id = "shortcuts",
                Name = "Desktop Shortcuts",
                Description = "Desktop and start menu shortcuts",
                IsRequired = false,
                IsInstalled = await CheckShortcutsAsync(),
                Version = "1.0.0"
            }
        };
    }

    public async Task<bool> ValidateSystemRequirementsAsync()
    {
        try
        {
            // Check .NET runtime
            var dotnetVersion = Environment.Version;
            if (dotnetVersion.Major < 9)
            {
                _logger.LogError("Unsupported .NET version: {Version}. Requires .NET 9.0 or higher", dotnetVersion);
                return false;
            }

            // Check operating system
            if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux() && !OperatingSystem.IsMacOS())
            {
                _logger.LogError("Unsupported operating system");
                return false;
            }

            // Check disk space (require at least 100MB)
            var drive = new DriveInfo(Path.GetPathRoot(_installationPath) ?? "C:\\");
            if (drive.AvailableFreeSpace < 100 * 1024 * 1024) // 100MB
            {
                _logger.LogError("Insufficient disk space. Required: 100MB, Available: {Available}MB", 
                    drive.AvailableFreeSpace / (1024 * 1024));
                return false;
            }

            await Task.Delay(1); // Make it async
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating system requirements");
            return false;
        }
    }

    public async Task<InstallationProgress> GetInstallationProgressAsync()
    {
        await Task.Delay(1); // Make it async
        return _currentProgress ?? new InstallationProgress();
    }

    private void UpdateProgress(int percentage, string operation)
    {
        if (_currentProgress != null)
        {
            _currentProgress.ProgressPercentage = percentage;
            _currentProgress.CurrentOperation = operation;
            
            if (!_currentProgress.CompletedSteps.Contains(operation))
            {
                _currentProgress.CompletedSteps.Add(operation);
            }

            // Calculate estimated time remaining
            var elapsed = DateTime.UtcNow - _currentProgress.StartTime;
            if (percentage > 0)
            {
                var totalEstimated = TimeSpan.FromTicks(elapsed.Ticks * 100 / percentage);
                _currentProgress.EstimatedTimeRemaining = totalEstimated - elapsed;
            }
        }
    }

    private async Task CreateInstallationBackupAsync()
    {
        // Implementation for creating installation backup
        await Task.Delay(100);
    }

    private async Task InstallComponentsAsync(List<string> components)
    {
        // Implementation for installing components
        await Task.Delay(100);
    }

    private async Task ConfigureDatabaseAsync(InstallationOptions options)
    {
        await _configService.SetValueAsync("Database:Type", options.DatabaseType);
        if (!string.IsNullOrEmpty(options.DatabaseConnectionString))
        {
            await _configService.SetValueAsync("ConnectionStrings:DefaultConnection", options.DatabaseConnectionString);
        }
    }

    private async Task CreateShortcutsAsync(InstallationOptions options)
    {
        // Implementation for creating shortcuts
        await Task.Delay(100);
    }

    private async Task InstallAsServiceAsync()
    {
        // Implementation for installing as Windows service
        await Task.Delay(100);
    }

    private async Task ConfigureFirewallAsync()
    {
        // Implementation for configuring firewall rules
        await Task.Delay(100);
    }

    private async Task FinalizeInstallationAsync(InstallationOptions options)
    {
        await _configService.SetValueAsync("Installation:Completed", "true");
        await _configService.SetValueAsync("Installation:Date", DateTime.UtcNow.ToString("O"));
        await _configService.SetValueAsync("Installation:Version", "1.0.0");
        await _configService.SetValueAsync("Installation:Path", options.InstallationPath);
    }

    private async Task<bool> FixIssueAsync(SystemIssue issue)
    {
        try
        {
            switch (issue.Id)
            {
                case "database_connection":
                    await _context.Database.EnsureCreatedAsync();
                    return true;
                default:
                    return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fix issue: {IssueId}", issue.Id);
            return false;
        }
    }

    private async Task RemoveShortcutsAsync()
    {
        await Task.Delay(100);
    }

    private async Task UninstallServiceAsync()
    {
        await Task.Delay(100);
    }

    private async Task RemoveFirewallRulesAsync()
    {
        await Task.Delay(100);
    }

    private async Task<bool> CheckDatabaseAsync()
    {
        try
        {
            return await _context.Database.CanConnectAsync();
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> CheckShortcutsAsync()
    {
        await Task.Delay(1);
        // Check if shortcuts exist
        return false; // Simplified for now
    }

    private async Task CheckSystemResourcesAsync(List<SystemIssue> issues)
    {
        await Task.Delay(1);
        
        // Check memory usage
        var process = Process.GetCurrentProcess();
        var memoryUsage = process.WorkingSet64 / (1024 * 1024); // MB
        
        if (memoryUsage > 500) // More than 500MB
        {
            issues.Add(new SystemIssue
            {
                Id = "high_memory_usage",
                Title = "High Memory Usage",
                Description = $"Application is using {memoryUsage}MB of memory",
                Severity = IssueSeverity.Warning,
                CanAutoFix = false
            });
        }
    }

    private async Task<Dictionary<string, object>> CollectSystemInfoAsync()
    {
        await Task.Delay(1);
        
        return new Dictionary<string, object>
        {
            ["OperatingSystem"] = Environment.OSVersion.ToString(),
            ["ProcessorCount"] = Environment.ProcessorCount,
            ["DotNetVersion"] = Environment.Version.ToString(),
            ["MachineName"] = Environment.MachineName,
            ["UserName"] = Environment.UserName,
            ["InstallationPath"] = _installationPath,
            ["CurrentTime"] = DateTime.UtcNow
        };
    }

    private bool HasWritePermission(string path)
    {
        try
        {
            var testFile = Path.Combine(path, "test_write_permission.tmp");
            File.WriteAllText(testFile, "test");
            File.Delete(testFile);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
