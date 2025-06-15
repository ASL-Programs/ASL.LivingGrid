using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Services;
using ASL.LivingGrid.WebAdminPanel.Models;
using Serilog;
using System.Reflection;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.AspNetCore.HttpOverrides;
using System.IO;
using System.Security.Claims;

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
            builder.WebHost.UseIISIntegration();

            // Detect hosting mode from configuration or command line
            var cfgMode = builder.Configuration.GetValue<string>("Hosting:Mode") ?? "Standalone";
            if (Environment.GetCommandLineArgs().Contains("--standalone"))
                cfgMode = "Standalone";
            if (Environment.GetCommandLineArgs().Contains("--hosted"))
                cfgMode = "WebServer";
            var isStandaloneExe = string.Equals(cfgMode, "Standalone", StringComparison.OrdinalIgnoreCase);
            
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

            // Handle hosting mode switch
            await HandleHostingModeSwitchAsync(app, cfgMode);

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
        services.AddScoped<IThemeService, ThemeService>();
        services.AddScoped<INavigationService, NavigationService>();
        services.AddScoped<ITranslationWorkflowService, TranslationWorkflowService>();
        services.AddScoped<IThemeMarketplaceService, ThemeMarketplaceService>();
        services.AddScoped<ILayoutMarketplaceService, LayoutMarketplaceService>();
        services.AddScoped<IRoleBasedUiService, RoleBasedUiService>();
        services.AddScoped<IModuleCustomizationService, ModuleCustomizationService>();
        services.AddScoped<IFeedbackService, FeedbackService>();
        services.AddScoped<ISessionPersistenceService, SessionPersistenceService>();
        services.AddScoped<ISearchService, SearchService>();

        // Add HTTP Client for external API calls
        services.AddHttpClient();

        // Add memory cache
        services.AddMemoryCache();

        services.AddHttpContextAccessor();

        // Add session support
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                       ForwardedHeaders.XForwardedProto;
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

        app.UseForwardedHeaders();

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

        var locGroup = app.MapGroup("/api/localization");
        locGroup.MapGet("/{culture}", async (string culture, ILocalizationService svc) =>
        {
            return Results.Ok(await svc.GetAllStringsAsync(culture));
        });

        locGroup.MapPost("/bulk", async (IEnumerable<LocalizationResource> items, ILocalizationService svc) =>
        {
            await svc.BulkSetAsync(items);
            return Results.Ok();
        });

        locGroup.MapGet("/export/{culture}", async (string culture, ILocalizationService svc) =>
        {
            var json = await svc.ExportAsync(culture);
            return Results.Text(json, "application/json");
        });

        locGroup.MapPost("/import/{culture}", async (string culture, HttpRequest req, ILocalizationService svc) =>
        {
            using var reader = new StreamReader(req.Body);
            var json = await reader.ReadToEndAsync();
            await svc.ImportAsync(json, culture);
            return Results.Ok();
        });

        locGroup.MapPost("/approve/{id}", async (Guid id, ILocalizationService svc) =>
        {
            await svc.ApproveAsync(id, "system");
            return Results.Ok();
        });

        var trGroup = app.MapGroup("/api/translationrequests");
        trGroup.MapGet("/pending", async (ITranslationWorkflowService svc) => Results.Ok(await svc.GetPendingRequestsAsync()));
        trGroup.MapGet("/status/{status}", async (TranslationRequestStatus status, ITranslationWorkflowService svc) =>
            Results.Ok(await svc.GetRequestsByStatusAsync(status)));
        trGroup.MapPost("/submit", async (TranslationRequest req, ITranslationWorkflowService svc, ClaimsPrincipal user) =>
        {
            var created = await svc.SubmitRequestAsync(req.Key, req.Culture, req.ProposedValue ?? string.Empty, user.Identity?.Name ?? "anon", req.Status);
            return Results.Ok(created);
        });
        trGroup.MapPost("/suggest", async (TranslationSuggestionRequest req, ITranslationWorkflowService svc) =>
        {
            var result = await svc.SuggestAsync(req.Text, req.SourceCulture, req.TargetCulture);
            return Results.Ok(new { suggestion = result });
        });
        trGroup.MapPost("/approve/{id}", async (Guid id, ITranslationWorkflowService svc, ClaimsPrincipal user) =>
        {
            await svc.ApproveRequestAsync(id, user.Identity?.Name ?? "system", apply: true);
            return Results.Ok();
        });
        trGroup.MapPost("/review/{id}", async (Guid id, TranslationReviewRequest review, ITranslationWorkflowService svc, ClaimsPrincipal user) =>
        {
            await svc.ReviewRequestAsync(id, review.Accept, user.Identity?.Name ?? "system", review.Comments, review.Escalate);
            return Results.Ok();
        });
        trGroup.MapPost("/reject/{id}", async (Guid id, TranslationReviewRequest review, ITranslationWorkflowService svc, ClaimsPrincipal user) =>
        {
            await svc.RejectRequestAsync(id, user.Identity?.Name ?? "system", review.Comments, review.Escalate);
            return Results.Ok();
        });
        trGroup.MapPost("/status/{id}", async (Guid id, TranslationRequestStatus status, ITranslationWorkflowService svc, ClaimsPrincipal user) =>
        {
            await svc.UpdateStatusAsync(id, status, user.Identity?.Name ?? "system");
            return Results.Ok();
        });

        var themeGroup = app.MapGroup("/api/themes");
        themeGroup.MapGet("/", async (IThemeMarketplaceService svc) => Results.Ok(await svc.ListAvailableThemesAsync()));
        themeGroup.MapPost("/import/{id}", async (string id, IThemeMarketplaceService svc) =>
        {
            var theme = await svc.ImportThemeAsync(id);
            return theme is not null ? Results.Ok(theme) : Results.NotFound();
        });
        themeGroup.MapGet("/export/{id}", async (string id, IThemeMarketplaceService svc) =>
        {
            var css = await svc.ExportThemeAsync(id);
            return string.IsNullOrEmpty(css) ? Results.NotFound() : Results.Text(css, "text/css");
        });

        var layoutGroup = app.MapGroup("/api/layouts");
        layoutGroup.MapGet("/", async (ILayoutMarketplaceService svc) => Results.Ok(await svc.ListAvailableLayoutsAsync()));
        layoutGroup.MapPost("/import/{id}", async (string id, ILayoutMarketplaceService svc) =>
        {
            var layout = await svc.ImportLayoutAsync(id);
            return layout is not null ? Results.Ok(layout) : Results.NotFound();
        });
        layoutGroup.MapGet("/export/{id}", async (string id, ILayoutMarketplaceService svc) =>
        {
            var json = await svc.ExportLayoutAsync(id);
            return string.IsNullOrEmpty(json) ? Results.NotFound() : Results.Text(json, "application/json");
        });

        app.MapGet("/api/search", async (string q, ISearchService svc) =>
        {
            var result = await svc.SearchAsync(q);
            return Results.Ok(result);
        });

        app.MapPost("/api/feedback", async (FeedbackItem item, IFeedbackService svc) =>
        {
            await svc.SubmitAsync(item.Page, item.Rating, item.Comments);
            return Results.Ok();
        });

        app.MapPost("/api/sync/ping", () => Results.Ok(new { Status = "Ok" }));
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

    private static async Task HandleHostingModeSwitchAsync(WebApplication app, string currentMode)
    {
        try
        {
            var file = Path.Combine(AppContext.BaseDirectory, "hosting_mode.txt");
            var previousMode = File.Exists(file) ? await File.ReadAllTextAsync(file) : null;

            if (!string.IsNullOrWhiteSpace(previousMode) && !string.Equals(previousMode, currentMode, StringComparison.OrdinalIgnoreCase))
            {
                var migration = app.Services.GetRequiredService<IMigrationService>();

                if (app.Configuration.GetValue<bool>("Hosting:BackupBeforeSwitch", true))
                {
                    var backupPath = Path.Combine(app.Configuration.GetValue<string>("BackupDirectory", "backups"),
                        $"mode_switch_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json");
                    await migration.CreateBackupAsync(backupPath);
                }

                if (app.Configuration.GetValue<bool>("Hosting:AutoMigrate", true))
                {
                    using var scope = app.Services.CreateScope();
                    var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    await ctx.Database.MigrateAsync();
                }
            }

            await File.WriteAllTextAsync(file, currentMode);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error handling hosting mode switch");
        }
    }
}
