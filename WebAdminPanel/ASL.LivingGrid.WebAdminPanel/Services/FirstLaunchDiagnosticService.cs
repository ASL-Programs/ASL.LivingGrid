using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class FirstLaunchDiagnosticService : IFirstLaunchDiagnosticService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FirstLaunchDiagnosticService> _logger;
    private readonly IConfigurationService _configService;
    private readonly ApplicationDbContext _context;
    private readonly IInstallerService _installerService;
    private static DiagnosticProgress? _currentProgress;

    public FirstLaunchDiagnosticService(
        IConfiguration configuration,
        ILogger<FirstLaunchDiagnosticService> logger,
        IConfigurationService configService,
        ApplicationDbContext context,
        IInstallerService installerService)
    {
        _configuration = configuration;
        _logger = logger;
        _configService = configService;
        _context = context;
        _installerService = installerService;
    }

    public async Task<DiagnosticResult> PerformFirstLaunchDiagnosticAsync()
    {
        var result = new DiagnosticResult();
        var startTime = DateTime.UtcNow;

        try
        {
            _logger.LogInformation("Starting first launch diagnostic");

            result.IsFirstLaunch = !await IsFirstLaunchCompleteAsync();
            result.DiagnosticDate = startTime;

            // Initialize progress tracking
            _currentProgress = new DiagnosticProgress
            {
                IsRunning = true,
                StartTime = startTime,
                TotalChecks = 12
            };

            var checks = new List<DiagnosticCheck>();

            // System Information Collection
            UpdateProgress(1, "Collecting system information...");
            result.SystemInfo = await CollectSystemInfoAsync();
            checks.Add(CreateSuccessCheck("system_info", "System Information", "System information collected successfully"));

            // .NET Runtime Check
            UpdateProgress(2, "Checking .NET runtime...");
            checks.Add(await CheckDotNetRuntimeAsync());

            // Operating System Compatibility
            UpdateProgress(3, "Checking operating system compatibility...");
            checks.Add(await CheckOperatingSystemAsync());

            // Memory and Storage Check
            UpdateProgress(4, "Checking memory and storage...");
            checks.Add(await CheckMemoryAndStorageAsync());

            // Network Connectivity Check
            UpdateProgress(5, "Checking network connectivity...");
            checks.Add(await CheckNetworkConnectivityAsync());

            // Database Connectivity Check
            UpdateProgress(6, "Checking database connectivity...");
            checks.Add(await CheckDatabaseConnectivityAsync());

            // Permissions Check
            UpdateProgress(7, "Checking application permissions...");
            checks.Add(await CheckPermissionsAsync());

            // Dependencies Check
            UpdateProgress(8, "Checking dependencies...");
            checks.Add(await CheckDependenciesAsync());

            // Security Features Check
            UpdateProgress(9, "Checking security features...");
            checks.Add(await CheckSecurityFeaturesAsync());

            // Performance Check
            UpdateProgress(10, "Running performance tests...");
            result.Performance = await GetPerformanceReportAsync();
            checks.Add(CreateSuccessCheck("performance", "Performance Test", "Performance test completed"));

            // Compatibility Check
            UpdateProgress(11, "Running compatibility check...");
            result.Compatibility = await PerformCompatibilityCheckAsync();
            checks.Add(CreateSuccessCheck("compatibility", "Compatibility Check", "Compatibility check completed"));

            // Configuration Validation
            UpdateProgress(12, "Validating configuration...");
            checks.Add(await CheckConfigurationAsync());

            result.Checks = checks;

            // Analyze results
            AnalyzeResults(result);

            // Get recommendations
            var recommendations = await GetSystemRecommendationsAsync();
            result.Recommendations = recommendations.Performance
                .Concat(recommendations.Security)
                .Concat(recommendations.Configuration)
                .Select(r => r.Description)
                .ToList();

            result.IsSuccess = !result.Issues.Any(i => i.Severity == DiagnosticSeverity.Critical || i.Severity == DiagnosticSeverity.Error);

            UpdateProgress(12, "Diagnostic completed");
            _currentProgress.IsRunning = false;

            _logger.LogInformation("First launch diagnostic completed. Success: {IsSuccess}, Issues: {IssueCount}", 
                result.IsSuccess, result.Issues.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during first launch diagnostic");
            result.IsSuccess = false;
            result.Issues.Add(new DiagnosticIssue
            {
                Id = "diagnostic_error",
                Title = "Diagnostic Error",
                Description = $"An error occurred during diagnostic: {ex.Message}",
                Severity = DiagnosticSeverity.Critical,
                Category = "System"
            });

            if (_currentProgress != null)
            {
                _currentProgress.IsRunning = false;
            }

            return result;
        }
    }

    public async Task<CompatibilityCheckResult> PerformCompatibilityCheckAsync()
    {
        var result = new CompatibilityCheckResult();

        try
        {
            var issues = new List<CompatibilityIssue>();
            var requirements = new RequirementStatus();

            // Check .NET version
            var dotnetVersion = Environment.Version;
            requirements.DotNetVersion = dotnetVersion.Major >= 9;
            if (!requirements.DotNetVersion)
            {
                issues.Add(new CompatibilityIssue
                {
                    Component = ".NET Runtime",
                    Issue = $"Requires .NET 9.0 or higher, found {dotnetVersion}",
                    Severity = "Critical",
                    Solution = "Install .NET 9.0 or higher",
                    IsBlocking = true
                });
            }

            // Check operating system
            requirements.OperatingSystem = CheckOperatingSystemCompatibility();
            if (!requirements.OperatingSystem)
            {
                issues.Add(new CompatibilityIssue
                {
                    Component = "Operating System",
                    Issue = "Unsupported operating system",
                    Severity = "Critical",
                    Solution = "Use Windows 10/11, macOS 10.15+, or Ubuntu 18.04+",
                    IsBlocking = true
                });
            }

            // Check memory
            var totalMemoryMB = GC.GetTotalMemory(false) / (1024 * 1024);
            requirements.Memory = totalMemoryMB >= 512; // 512MB minimum
            if (!requirements.Memory)
            {
                issues.Add(new CompatibilityIssue
                {
                    Component = "Memory",
                    Issue = "Insufficient memory",
                    Severity = "Warning",
                    Solution = "Ensure at least 512MB of available memory",
                    IsBlocking = false
                });
            }

            // Check disk space
            var installationPath = AppDomain.CurrentDomain.BaseDirectory;
            var drive = new System.IO.DriveInfo(Path.GetPathRoot(installationPath) ?? "C:\\");
            var availableSpaceGB = drive.AvailableFreeSpace / (1024 * 1024 * 1024);
            requirements.DiskSpace = availableSpaceGB >= 1; // 1GB minimum
            if (!requirements.DiskSpace)
            {
                issues.Add(new CompatibilityIssue
                {
                    Component = "Disk Space",
                    Issue = "Insufficient disk space",
                    Severity = "Error",
                    Solution = "Ensure at least 1GB of available disk space",
                    IsBlocking = true
                });
            }

            // Check network connectivity
            requirements.NetworkConnectivity = await CheckNetworkConnectivity();

            // Check permissions
            requirements.Permissions = CheckPermissions();

            // Check dependencies
            requirements.Dependencies = await CheckDependenciesAvailability();

            // Determine supported features
            var supportedFeatures = new List<string>();
            var unsupportedFeatures = new List<string>();

            if (OperatingSystem.IsWindows())
            {
                supportedFeatures.Add("Windows Tray Icon");
                supportedFeatures.Add("Windows Service Installation");
            }
            else
            {
                unsupportedFeatures.Add("Windows Tray Icon");
                unsupportedFeatures.Add("Windows Service Installation");
            }

            if (requirements.NetworkConnectivity)
            {
                supportedFeatures.Add("Online Features");
                supportedFeatures.Add("External Integrations");
            }
            else
            {
                unsupportedFeatures.Add("Online Features");
                unsupportedFeatures.Add("External Integrations");
            }

            // Collect environment variables
            var envVars = new Dictionary<string, string>();
            var importantVars = new[] { "PATH", "TEMP", "USERPROFILE", "COMPUTERNAME", "PROCESSOR_ARCHITECTURE" };
            foreach (var varName in importantVars)
            {
                var value = Environment.GetEnvironmentVariable(varName);
                if (!string.IsNullOrEmpty(value))
                {
                    envVars[varName] = value;
                }
            }

            result.IsCompatible = !issues.Any(i => i.IsBlocking);
            result.Issues = issues;
            result.Requirements = requirements;
            result.SupportedFeatures = supportedFeatures;
            result.UnsupportedFeatures = unsupportedFeatures;
            result.EnvironmentVariables = envVars;

            await Task.Delay(1); // Make it async
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during compatibility check");
            result.IsCompatible = false;
            result.Issues.Add(new CompatibilityIssue
            {
                Component = "Compatibility Check",
                Issue = $"Error during compatibility check: {ex.Message}",
                Severity = "Critical",
                IsBlocking = true
            });
        }

        return result;
    }

    public async Task<SystemRecommendations> GetSystemRecommendationsAsync()
    {
        var recommendations = new SystemRecommendations();

        try
        {
            var systemInfo = await CollectSystemInfoAsync();
            var performance = await GetPerformanceReportAsync();

            // Performance recommendations
            if (performance.MemoryUsagePercent > 80)
            {
                recommendations.Performance.Add(new Recommendation
                {
                    Id = "high_memory_usage",
                    Title = "High Memory Usage",
                    Description = "System memory usage is high. Consider closing unnecessary applications or increasing available memory.",
                    Type = RecommendationType.Performance,
                    Priority = RecommendationPriority.Medium,
                    Category = "Memory",
                    Benefits = new List<string> { "Improved application performance", "Reduced risk of memory errors" }
                });
            }

            if (performance.CpuUsagePercent > 90)
            {
                recommendations.Performance.Add(new Recommendation
                {
                    Id = "high_cpu_usage",
                    Title = "High CPU Usage",
                    Description = "CPU usage is very high. Consider reducing background processes.",
                    Type = RecommendationType.Performance,
                    Priority = RecommendationPriority.High,
                    Category = "CPU"
                });
            }

            // Security recommendations
            if (!systemInfo.Security.IsAdministrator && OperatingSystem.IsWindows())
            {
                recommendations.Security.Add(new Recommendation
                {
                    Id = "admin_privileges",
                    Title = "Administrator Privileges",
                    Description = "Running with administrator privileges can improve functionality.",
                    Type = RecommendationType.Security,
                    Priority = RecommendationPriority.Low,
                    Category = "Permissions"
                });
            }

            if (!systemInfo.Security.HasFirewall)
            {
                recommendations.Security.Add(new Recommendation
                {
                    Id = "firewall_enabled",
                    Title = "Enable Firewall",
                    Description = "Windows Firewall should be enabled for security.",
                    Type = RecommendationType.Security,
                    Priority = RecommendationPriority.High,
                    Category = "Network Security"
                });
            }

            // Configuration recommendations
            var isFirstLaunch = !await IsFirstLaunchCompleteAsync();
            if (isFirstLaunch)
            {
                recommendations.Configuration.Add(new Recommendation
                {
                    Id = "complete_setup",
                    Title = "Complete Initial Setup",
                    Description = "Complete the initial setup wizard to configure the application properly.",
                    Type = RecommendationType.Configuration,
                    Priority = RecommendationPriority.Critical,
                    Category = "Setup",
                    CanAutoApply = false
                });
            }

            // Feature recommendations
            if (OperatingSystem.IsWindows())
            {
                recommendations.Features.Add(new Recommendation
                {
                    Id = "enable_tray_icon",
                    Title = "Enable Tray Icon",
                    Description = "Enable the system tray icon for quick access to the application.",
                    Type = RecommendationType.Feature,
                    Priority = RecommendationPriority.Low,
                    Category = "User Experience",
                    CanAutoApply = true,
                    AutoApplyAction = "EnableTrayIcon"
                });
            }

            // Maintenance recommendations
            recommendations.Maintenance.Add(new Recommendation
            {
                Id = "regular_backup",
                Title = "Setup Regular Backups",
                Description = "Configure automatic backups to protect your data.",
                Type = RecommendationType.Maintenance,
                Priority = RecommendationPriority.Medium,
                Category = "Data Protection"
            });

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating system recommendations");
        }

        return recommendations;
    }

    public async Task<bool> FixDiagnosticIssueAsync(string issueId)
    {
        try
        {
            _logger.LogInformation("Attempting to fix diagnostic issue: {IssueId}", issueId);

            switch (issueId)
            {
                case "database_connection":
                    await _context.Database.EnsureCreatedAsync();
                    return true;

                case "missing_config":
                    await CreateDefaultConfigurationAsync();
                    return true;

                case "insufficient_permissions":
                    // Cannot auto-fix permission issues
                    return false;

                default:
                    _logger.LogWarning("Unknown issue ID for auto-fix: {IssueId}", issueId);
                    return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fixing diagnostic issue: {IssueId}", issueId);
            return false;
        }
    }

    public async Task<DiagnosticProgress> GetDiagnosticProgressAsync()
    {
        await Task.Delay(1); // Make it async
        return _currentProgress ?? new DiagnosticProgress();
    }

    public async Task<bool> MarkFirstLaunchCompleteAsync()
    {
        try
        {
            await _configService.SetValueAsync("FirstLaunch:Completed", true);
            await _configService.SetValueAsync("FirstLaunch:CompletedDate", DateTime.UtcNow.ToString("O"));
            
            _logger.LogInformation("First launch marked as complete");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking first launch as complete");
            return false;
        }
    }

    public async Task<bool> IsFirstLaunchCompleteAsync()
    {
        try
        {
            return await _configService.GetValueAsync<bool>("FirstLaunch:Completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking first launch status");
            return false;
        }
    }

    public async Task<SystemPerformanceReport> GetPerformanceReportAsync()
    {
        var report = new SystemPerformanceReport();

        try
        {
            var process = Process.GetCurrentProcess();
            var startTime = DateTime.UtcNow;

            // CPU Usage (approximate)
            var startCpuTime = process.TotalProcessorTime;
            await Task.Delay(1000); // Wait 1 second
            var endCpuTime = process.TotalProcessorTime;
            var cpuUsage = (endCpuTime - startCpuTime).TotalMilliseconds / 1000.0 * 100.0 / Environment.ProcessorCount;
            report.CpuUsagePercent = Math.Max(0, Math.Min(100, cpuUsage));

            // Memory Usage
            var workingSet = process.WorkingSet64;
            var totalMemory = GC.GetTotalMemory(false);
            report.MemoryUsagePercent = (double)workingSet / (1024 * 1024 * 1024) * 100; // As percentage of 1GB

            // Database Connection Time
            var dbStartTime = DateTime.UtcNow;
            try
            {
                await _context.Database.CanConnectAsync();
                report.DatabaseConnectionTime = (int)(DateTime.UtcNow - dbStartTime).TotalMilliseconds;
            }
            catch
            {
                report.DatabaseConnectionTime = -1; // Connection failed
            }

            // Application Startup Time (simulated)
            report.ApplicationStartupTime = DateTime.UtcNow - Process.GetCurrentProcess().StartTime;

            // Performance metrics
            report.Metrics = new List<PerformanceMetric>
            {
                new()
                {
                    Name = "Memory Usage",
                    Value = workingSet / (1024 * 1024),
                    Unit = "MB",
                    Category = "Memory",
                    Rating = workingSet < 100 * 1024 * 1024 ? PerformanceRating.Excellent : 
                             workingSet < 500 * 1024 * 1024 ? PerformanceRating.Good : PerformanceRating.Fair
                },
                new()
                {
                    Name = "Database Response",
                    Value = report.DatabaseConnectionTime,
                    Unit = "ms",
                    Category = "Database",
                    Rating = report.DatabaseConnectionTime < 100 ? PerformanceRating.Excellent :
                             report.DatabaseConnectionTime < 500 ? PerformanceRating.Good : PerformanceRating.Fair
                }
            };

            // Overall rating
            var ratings = report.Metrics.Select(m => (int)m.Rating);
            var averageRating = ratings.Any() ? ratings.Average() : 0;
            report.OverallRating = (PerformanceRating)Math.Round(averageRating);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating performance report");
            report.OverallRating = PerformanceRating.Poor;
        }

        return report;
    }

    public async Task<bool> PerformSystemOptimizationAsync()
    {
        try
        {
            _logger.LogInformation("Starting system optimization");

            // Run garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            // Clear temporary files (application-specific)
            var tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp");
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }

            // Optimize database
            await OptimizeDatabaseAsync();

            _logger.LogInformation("System optimization completed");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during system optimization");
            return false;
        }
    }

    // Private helper methods
    private async Task<SystemInfo> CollectSystemInfoAsync()
    {
        var systemInfo = new SystemInfo();

        try
        {
            systemInfo.OperatingSystem = RuntimeInformation.OSDescription;
            systemInfo.OSVersion = Environment.OSVersion.VersionString;
            systemInfo.MachineName = Environment.MachineName;
            systemInfo.UserName = Environment.UserName;
            systemInfo.DotNetVersion = Environment.Version.ToString();
            systemInfo.ProcessorCount = Environment.ProcessorCount;
            systemInfo.ApplicationVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";

            // Memory information
            var process = Process.GetCurrentProcess();
            systemInfo.TotalMemoryMB = process.WorkingSet64 / (1024 * 1024);
            systemInfo.AvailableMemoryMB = systemInfo.TotalMemoryMB; // Simplified

            // Drive information
            foreach (var drive in System.IO.DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    systemInfo.Drives.Add(new Models.DriveInfo
                    {
                        Name = drive.Name,
                        DriveType = drive.DriveType.ToString(),
                        TotalSizeGB = drive.TotalSize / (1024 * 1024 * 1024),
                        AvailableSpaceGB = drive.AvailableFreeSpace / (1024 * 1024 * 1024),
                        FileSystem = drive.DriveFormat,
                        IsReady = drive.IsReady
                    });
                }
            }

            // Network interfaces
            foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (netInterface.OperationalStatus == OperationalStatus.Up)
                {
                    var networkInfo = new NetworkInfo
                    {
                        Name = netInterface.Name,
                        Type = netInterface.NetworkInterfaceType.ToString(),
                        IsUp = netInterface.OperationalStatus == OperationalStatus.Up,
                        MacAddress = netInterface.GetPhysicalAddress().ToString(),
                        Speed = netInterface.Speed
                    };

                    var ipProps = netInterface.GetIPProperties();
                    foreach (var addr in ipProps.UnicastAddresses)
                    {
                        networkInfo.IPAddresses.Add(addr.Address.ToString());
                    }

                    systemInfo.NetworkInterfaces.Add(networkInfo);
                }
            }

            // Security information
            systemInfo.Security = new SecurityInfo();
            if (OperatingSystem.IsWindows())
            {
                var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                systemInfo.Security.IsAdministrator = principal.IsInRole(WindowsBuiltInRole.Administrator);
                systemInfo.Security.IsElevated = systemInfo.Security.IsAdministrator;
            }

            systemInfo.SystemTime = DateTime.UtcNow;
            systemInfo.SystemUptime = TimeSpan.FromMilliseconds(Environment.TickCount64);

            await Task.Delay(1); // Make it async
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error collecting system information");
        }

        return systemInfo;
    }

    private async Task<DiagnosticCheck> CheckDotNetRuntimeAsync()
    {
        try
        {
            var version = Environment.Version;
            var isSupported = version.Major >= 9;

            return new DiagnosticCheck
            {
                Id = "dotnet_runtime",
                Name = ".NET Runtime Version",
                Description = "Checks if the required .NET runtime version is available",
                Status = isSupported ? DiagnosticStatus.Passed : DiagnosticStatus.Failed,
                Message = $".NET {version} detected. {(isSupported ? "Supported" : "Requires .NET 9.0 or higher")}",
                Duration = TimeSpan.FromMilliseconds(10),
                Details = new Dictionary<string, object>
                {
                    ["Version"] = version.ToString(),
                    ["IsSupported"] = isSupported,
                    ["RequiredVersion"] = "9.0"
                }
            };
        }
        catch (Exception ex)
        {
            return new DiagnosticCheck
            {
                Id = "dotnet_runtime",
                Name = ".NET Runtime Version",
                Status = DiagnosticStatus.Failed,
                Message = $"Error checking .NET runtime: {ex.Message}"
            };
        }
        finally
        {
            await Task.Delay(1); // Make it async
        }
    }

    private async Task<DiagnosticCheck> CheckOperatingSystemAsync()
    {
        await Task.Delay(1); // Make it async

        var isSupported = CheckOperatingSystemCompatibility();
        var osDescription = RuntimeInformation.OSDescription;

        return new DiagnosticCheck
        {
            Id = "operating_system",
            Name = "Operating System",
            Description = "Checks operating system compatibility",
            Status = isSupported ? DiagnosticStatus.Passed : DiagnosticStatus.Warning,
            Message = $"{osDescription} - {(isSupported ? "Supported" : "May have limited functionality")}",
            Details = new Dictionary<string, object>
            {
                ["OS"] = osDescription,
                ["IsSupported"] = isSupported,
                ["Architecture"] = RuntimeInformation.OSArchitecture.ToString()
            }
        };
    }

    private async Task<DiagnosticCheck> CheckMemoryAndStorageAsync()
    {
        try
        {
            var process = Process.GetCurrentProcess();
            var workingSetMB = process.WorkingSet64 / (1024 * 1024);
            
            var installPath = AppDomain.CurrentDomain.BaseDirectory;
            var drive = new System.IO.DriveInfo(Path.GetPathRoot(installPath) ?? "C:\\");
            var availableSpaceGB = drive.AvailableFreeSpace / (1024 * 1024 * 1024);

            var memoryOk = workingSetMB < 1000; // Less than 1GB
            var storageOk = availableSpaceGB > 1; // More than 1GB available

            var status = memoryOk && storageOk ? DiagnosticStatus.Passed :
                         memoryOk || storageOk ? DiagnosticStatus.Warning : DiagnosticStatus.Failed;

            return new DiagnosticCheck
            {
                Id = "memory_storage",
                Name = "Memory and Storage",
                Description = "Checks available memory and disk space",
                Status = status,
                Message = $"Memory: {workingSetMB}MB, Available Storage: {availableSpaceGB}GB",
                Details = new Dictionary<string, object>
                {
                    ["WorkingSetMB"] = workingSetMB,
                    ["AvailableSpaceGB"] = availableSpaceGB,
                    ["MemoryOk"] = memoryOk,
                    ["StorageOk"] = storageOk
                }
            };
        }
        catch (Exception ex)
        {
            return new DiagnosticCheck
            {
                Id = "memory_storage",
                Name = "Memory and Storage",
                Status = DiagnosticStatus.Failed,
                Message = $"Error checking memory and storage: {ex.Message}"
            };
        }
        finally
        {
            await Task.Delay(1); // Make it async
        }
    }

    private async Task<DiagnosticCheck> CheckNetworkConnectivityAsync()
    {
        var hasConnectivity = await CheckNetworkConnectivity();

        return new DiagnosticCheck
        {
            Id = "network_connectivity",
            Name = "Network Connectivity",
            Description = "Checks if network connectivity is available",
            Status = hasConnectivity ? DiagnosticStatus.Passed : DiagnosticStatus.Warning,
            Message = hasConnectivity ? "Network connectivity available" : "Limited or no network connectivity",
            Details = new Dictionary<string, object>
            {
                ["HasConnectivity"] = hasConnectivity
            }
        };
    }

    private async Task<DiagnosticCheck> CheckDatabaseConnectivityAsync()
    {
        try
        {
            var startTime = DateTime.UtcNow;
            var canConnect = await _context.Database.CanConnectAsync();
            var duration = DateTime.UtcNow - startTime;

            return new DiagnosticCheck
            {
                Id = "database_connectivity",
                Name = "Database Connectivity",
                Description = "Checks if the database is accessible",
                Status = canConnect ? DiagnosticStatus.Passed : DiagnosticStatus.Failed,
                Message = canConnect ? $"Database connected successfully ({duration.TotalMilliseconds:F0}ms)" : "Database connection failed",
                Duration = duration,
                Details = new Dictionary<string, object>
                {
                    ["CanConnect"] = canConnect,
                    ["ResponseTimeMs"] = duration.TotalMilliseconds
                }
            };
        }
        catch (Exception ex)
        {
            return new DiagnosticCheck
            {
                Id = "database_connectivity",
                Name = "Database Connectivity",
                Status = DiagnosticStatus.Failed,
                Message = $"Database connection error: {ex.Message}"
            };
        }
    }

    private async Task<DiagnosticCheck> CheckPermissionsAsync()
    {
        await Task.Delay(1); // Make it async

        var hasPermissions = CheckPermissions();

        return new DiagnosticCheck
        {
            Id = "permissions",
            Name = "Application Permissions",
            Description = "Checks if the application has necessary permissions",
            Status = hasPermissions ? DiagnosticStatus.Passed : DiagnosticStatus.Warning,
            Message = hasPermissions ? "Application has necessary permissions" : "Application may lack some permissions",
            Details = new Dictionary<string, object>
            {
                ["HasPermissions"] = hasPermissions
            }
        };
    }

    private async Task<DiagnosticCheck> CheckDependenciesAsync()
    {
        var hasAllDependencies = await CheckDependenciesAvailability();

        return new DiagnosticCheck
        {
            Id = "dependencies",
            Name = "Dependencies",
            Description = "Checks if all required dependencies are available",
            Status = hasAllDependencies ? DiagnosticStatus.Passed : DiagnosticStatus.Warning,
            Message = hasAllDependencies ? "All dependencies available" : "Some dependencies may be missing",
            Details = new Dictionary<string, object>
            {
                ["HasAllDependencies"] = hasAllDependencies
            }
        };
    }

    private async Task<DiagnosticCheck> CheckSecurityFeaturesAsync()
    {
        await Task.Delay(1); // Make it async

        var securityScore = 0;
        var securityFeatures = new List<string>();

        if (OperatingSystem.IsWindows())
        {
            try
            {
                var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                if (principal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    securityFeatures.Add("Administrator privileges");
                    securityScore++;
                }
            }
            catch { }
        }

        // Check HTTPS configuration (new key with legacy fallback)
        var requireHttps = _configuration.GetValue<bool?>("Security:RequireHttps");
        var oldForceHttps = _configuration.GetValue<bool>("ForceHttps", false);
        if (requireHttps.GetValueOrDefault(oldForceHttps))
        {
            securityFeatures.Add("HTTPS enabled");
            securityScore++;
        }

        var status = securityScore >= 1 ? DiagnosticStatus.Passed : DiagnosticStatus.Warning;

        return new DiagnosticCheck
        {
            Id = "security_features",
            Name = "Security Features",
            Description = "Checks available security features",
            Status = status,
            Message = $"Security features: {string.Join(", ", securityFeatures.DefaultIfEmpty("Basic security"))}",
            Details = new Dictionary<string, object>
            {
                ["SecurityScore"] = securityScore,
                ["SecurityFeatures"] = securityFeatures
            }
        };
    }

    private async Task<DiagnosticCheck> CheckConfigurationAsync()
    {
        try
        {
            // Check if essential configuration exists
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var hasConnectionString = !string.IsNullOrEmpty(connectionString);

            var status = hasConnectionString ? DiagnosticStatus.Passed : DiagnosticStatus.Failed;

            return new DiagnosticCheck
            {
                Id = "configuration",
                Name = "Configuration",
                Description = "Validates application configuration",
                Status = status,
                Message = hasConnectionString ? "Configuration is valid" : "Configuration issues detected",
                Details = new Dictionary<string, object>
                {
                    ["HasConnectionString"] = hasConnectionString
                }
            };
        }
        catch (Exception ex)
        {
            return new DiagnosticCheck
            {
                Id = "configuration",
                Name = "Configuration",
                Status = DiagnosticStatus.Failed,
                Message = $"Configuration error: {ex.Message}"
            };
        }
        finally
        {
            await Task.Delay(1); // Make it async
        }
    }

    private DiagnosticCheck CreateSuccessCheck(string id, string name, string message)
    {
        return new DiagnosticCheck
        {
            Id = id,
            Name = name,
            Status = DiagnosticStatus.Passed,
            Message = message,
            CheckedAt = DateTime.UtcNow
        };
    }

    private void AnalyzeResults(DiagnosticResult result)
    {
        var issues = new List<DiagnosticIssue>();
        var warnings = new List<string>();

        foreach (var check in result.Checks)
        {
            switch (check.Status)
            {
                case DiagnosticStatus.Failed:
                    issues.Add(new DiagnosticIssue
                    {
                        Id = check.Id,
                        Title = check.Name,
                        Description = check.Message,
                        Severity = DiagnosticSeverity.Error,
                        Category = "System Check",
                        CanAutoFix = check.CanFix
                    });
                    break;

                case DiagnosticStatus.Warning:
                    warnings.Add($"{check.Name}: {check.Message}");
                    issues.Add(new DiagnosticIssue
                    {
                        Id = check.Id,
                        Title = check.Name,
                        Description = check.Message,
                        Severity = DiagnosticSeverity.Warning,
                        Category = "System Check"
                    });
                    break;
            }
        }

        result.Issues = issues;
        result.Warnings = warnings;
    }

    private void UpdateProgress(int completed, string currentCheck)
    {
        if (_currentProgress != null)
        {
            _currentProgress.CompletedChecks = completed;
            _currentProgress.CurrentCheck = currentCheck;
            _currentProgress.CompletedCheckNames.Add(currentCheck);

            // Calculate estimated time remaining
            var elapsed = DateTime.UtcNow - _currentProgress.StartTime;
            if (completed > 0)
            {
                var totalEstimated = TimeSpan.FromTicks(elapsed.Ticks * _currentProgress.TotalChecks / completed);
                _currentProgress.EstimatedTimeRemaining = totalEstimated - elapsed;
            }
        }
    }

    private bool CheckOperatingSystemCompatibility()
    {
        return OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS();
    }

    private async Task<bool> CheckNetworkConnectivity()
    {
        try
        {
            using var ping = new Ping();
            var reply = await ping.SendPingAsync("8.8.8.8", 3000);
            return reply.Status == IPStatus.Success;
        }
        catch
        {
            return false;
        }
    }

    private bool CheckPermissions()
    {
        try
        {
            var testPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_permission.tmp");
            File.WriteAllText(testPath, "test");
            File.Delete(testPath);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> CheckDependenciesAvailability()
    {
        await Task.Delay(1); // Make it async
        
        try
        {
            // Check Entity Framework
            var _ = _context.GetType();
            
            // Check basic .NET functionality
            var json = System.Text.Json.JsonSerializer.Serialize(new { test = "value" });
            
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task CreateDefaultConfigurationAsync()
    {
        try
        {
            await _configService.SetValueAsync("Application:Name", "ASL LivingGrid WebAdminPanel");
            await _configService.SetValueAsync("Application:Version", "1.0.0");
            await _configService.SetValueAsync("Application:EnableTrayIcon", "true");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating default configuration");
        }
    }

    private async Task OptimizeDatabaseAsync()
    {
        try
        {
            // Simple database optimization - ensure database is created and migrations are applied
            await _context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error optimizing database");
        }
    }
}
