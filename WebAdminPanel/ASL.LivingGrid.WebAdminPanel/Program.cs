using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Services;
using Serilog;
using System.Reflection;
using Microsoft.Extensions.Hosting.WindowsServices;

namespace ASL.LivingGrid.WebAdminPanel;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/webadminpanel-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            Log.Information("Starting ASL.LivingGrid Web Admin Panel");
            
            var builder = WebApplication.CreateBuilder(args);

            // Detect hosting mode
            var isStandaloneExe = Environment.GetCommandLineArgs().Contains("--standalone") || 
                                  !builder.Environment.IsDevelopment();
            
            // Configure logging
            builder.Host.UseSerilog();

            // Support Windows Service mode for standalone deployment
            if (WindowsServiceHelpers.IsWindowsService())
            {
                builder.Host.UseWindowsService();
            }

            // Add services to the container.
            ConfigureServices(builder.Services, builder.Configuration, isStandaloneExe);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            ConfigurePipeline(app, isStandaloneExe);

            // Initialize database
            await InitializeDatabaseAsync(app);

            // Start the application
            if (isStandaloneExe)
            {
                Log.Information("Running in standalone mode");
                // Configure Kestrel for standalone mode
                app.Urls.Add("http://localhost:5000");
                app.Urls.Add("https://localhost:5001");
            }

            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration, bool isStandaloneExe)
    {
        // Add Entity Framework
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
            "Data Source=webadminpanel.db";
        
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (connectionString.Contains(".db"))
            {
                options.UseSqlite(connectionString);
            }
            else
            {
                options.UseSqlServer(connectionString);
            }
        });

        services.AddDatabaseDeveloperPageExceptionFilter();

        // Add Identity
        services.AddDefaultIdentity<IdentityUser>(options => 
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>();

        // Add Blazor Server
        services.AddRazorPages();
        services.AddServerSideBlazor();

        // Add custom services
        services.AddScoped<IConfigurationService, ConfigurationService>();
        services.AddScoped<ILocalizationService, LocalizationService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IMigrationService, MigrationService>();
        services.AddScoped<IOnboardingService, OnboardingService>();
        services.AddScoped<IInstallerService, InstallerService>();
        services.AddScoped<IEnvironmentProvisioningService, EnvironmentProvisioningService>();
        services.AddScoped<IFirstLaunchDiagnosticService, FirstLaunchDiagnosticService>();
        services.AddScoped<IAdvancedRollbackService, AdvancedRollbackService>();
        services.AddScoped<IWireframePageBuilderService, WireframePageBuilderService>();

        // Add HTTP Client for external API calls
        services.AddHttpClient();

        // Add memory cache
        services.AddMemoryCache();

        // Add session support
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        // Add tray icon service (only on Windows and in standalone mode)
        if (OperatingSystem.IsWindows() && isStandaloneExe)
        {
            var enableTrayIcon = configuration.GetValue<bool>("Application:EnableTrayIcon", true);
            if (enableTrayIcon)
            {
                services.AddHostedService<TrayIconService>();
            }
        }
    }

    private static void ConfigurePipeline(WebApplication app, bool isStandaloneExe)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        if (!isStandaloneExe || app.Configuration.GetValue<bool>("ForceHttps"))
        {
            app.UseHttpsRedirection();
        }

        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();

        app.MapRazorPages();
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        // Health check endpoint
        app.MapGet("/health", () => Results.Ok(new 
        { 
            Status = "Healthy", 
            Timestamp = DateTime.UtcNow,
            Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString()
        }));
    }

    private static async Task InitializeDatabaseAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        try
        {
            await context.Database.EnsureCreatedAsync();
            Log.Information("Database initialized successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error initializing database");
            throw;
        }
    }
}
