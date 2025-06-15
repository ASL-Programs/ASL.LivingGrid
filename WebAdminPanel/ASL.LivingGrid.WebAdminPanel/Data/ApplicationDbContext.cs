using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Core entities
    public DbSet<Company> Companies { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Configuration> Configurations { get; set; }
    public DbSet<LocalizationResource> LocalizationResources { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<Plugin> Plugins { get; set; }
    public DbSet<LocalizationResourceVersion> LocalizationResourceVersions { get; set; }
    public DbSet<TranslationProject> TranslationProjects { get; set; }
    public DbSet<TranslationKey> TranslationKeys { get; set; }
    public DbSet<TranslationRequest> TranslationRequests { get; set; }
    public DbSet<CultureCustomization> CultureCustomizations { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure entity relationships and constraints
        builder.Entity<Company>()
            .HasIndex(c => c.Code)
            .IsUnique();

        builder.Entity<Tenant>()
            .HasOne(t => t.Company)
            .WithMany(c => c.Tenants)
            .HasForeignKey(t => t.CompanyId);

        builder.Entity<AppUser>()
            .HasOne(u => u.Company)
            .WithMany(c => c.Users)
            .HasForeignKey(u => u.CompanyId);

        builder.Entity<Configuration>()
            .HasIndex(c => new { c.Key, c.TenantId })
            .IsUnique();

        builder.Entity<LocalizationResource>()
            .HasIndex(l => new { l.Key, l.Culture })
            .IsUnique();

        builder.Entity<LocalizationResourceVersion>()
            .HasIndex(v => new { v.ResourceId, v.Version });

        builder.Entity<TranslationProject>()
            .HasIndex(p => p.Name);

        builder.Entity<TranslationKey>()
            .HasIndex(k => new { k.ProjectId, k.Key })
            .IsUnique();

        builder.Entity<TranslationRequest>()
            .HasIndex(r => new { r.Key, r.Culture, r.Status });
        builder.Entity<AuditLog>()
            .HasIndex(a => a.Timestamp);

        builder.Entity<AuditLog>()
            .HasIndex(a => a.UserId);

        // Seed initial data
        SeedInitialData(builder);
    }

    private void SeedInitialData(ModelBuilder builder)
    {
        // Seed default company
        var defaultCompanyId = Guid.NewGuid();
        builder.Entity<Company>().HasData(new Company
        {
            Id = defaultCompanyId,
            Name = "ASL LivingGrid",
            Code = "ASL",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        // Seed default configurations
        var configs = new[]
        {
            new Configuration 
            { 
                Id = Guid.NewGuid(), 
                Key = "App:Title", 
                Value = "ASL LivingGrid - Web Admin Panel",
                Description = "Application title",
                CreatedAt = DateTime.UtcNow
            },
            new Configuration 
            { 
                Id = Guid.NewGuid(), 
                Key = "App:DefaultLanguage", 
                Value = "az",
                Description = "Default application language",
                CreatedAt = DateTime.UtcNow
            },
            new Configuration 
            { 
                Id = Guid.NewGuid(), 
                Key = "App:SupportedLanguages", 
                Value = "az,en,tr,ru",
                Description = "Supported application languages",
                CreatedAt = DateTime.UtcNow
            },
            new Configuration 
            { 
                Id = Guid.NewGuid(), 
                Key = "Hosting:Mode", 
                Value = "Auto",
                Description = "Hosting mode: Auto, Standalone, Web",
                CreatedAt = DateTime.UtcNow
            }
        };

        builder.Entity<Configuration>().HasData(configs);

        // Seed basic localization resources
        var localizationResources = new[]
        {
            // Azerbaijani
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Common.Welcome", Value = "Xoş gəlmisiniz", Culture = "az", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Common.Login", Value = "Daxil ol", Culture = "az", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Common.Logout", Value = "Çıxış", Culture = "az", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Navigation.Dashboard", Value = "İdarə paneli", Culture = "az", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Navigation.LocalizationCoverage", Value = "Tərcümə Örtüyü", Culture = "az", CreatedAt = DateTime.UtcNow },
            
            // English
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Common.Welcome", Value = "Welcome", Culture = "en", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Common.Login", Value = "Login", Culture = "en", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Common.Logout", Value = "Logout", Culture = "en", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Navigation.Dashboard", Value = "Dashboard", Culture = "en", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Navigation.LocalizationCoverage", Value = "Translation Coverage", Culture = "en", CreatedAt = DateTime.UtcNow },
            
            // Turkish
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Common.Welcome", Value = "Hoş geldiniz", Culture = "tr", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Common.Login", Value = "Giriş", Culture = "tr", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Common.Logout", Value = "Çıkış", Culture = "tr", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Navigation.Dashboard", Value = "Kontrol Paneli", Culture = "tr", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Navigation.LocalizationCoverage", Value = "Çeviri Kapsamı", Culture = "tr", CreatedAt = DateTime.UtcNow },
            
            // Russian
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Common.Welcome", Value = "Добро пожаловать", Culture = "ru", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Common.Login", Value = "Войти", Culture = "ru", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Common.Logout", Value = "Выйти", Culture = "ru", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Navigation.Dashboard", Value = "Панель управления", Culture = "ru", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Navigation.LocalizationCoverage", Value = "Покрытие переводов", Culture = "ru", CreatedAt = DateTime.UtcNow }
        };

        builder.Entity<LocalizationResource>().HasData(localizationResources);

        var customizations = new[]
        {
            new CultureCustomization
            {
                Id = Guid.NewGuid(),
                Culture = "az",
                TextDirection = "ltr",
                FontFamily = "Arial",
                FontScale = 1.0,
                CreatedAt = DateTime.UtcNow
            },
            new CultureCustomization
            {
                Id = Guid.NewGuid(),
                Culture = "en",
                TextDirection = "ltr",
                FontFamily = "Helvetica",
                FontScale = 1.0,
                CreatedAt = DateTime.UtcNow
            },
            new CultureCustomization
            {
                Id = Guid.NewGuid(),
                Culture = "tr",
                TextDirection = "ltr",
                FontFamily = "Arial",
                FontScale = 1.0,
                CreatedAt = DateTime.UtcNow
            },
            new CultureCustomization
            {
                Id = Guid.NewGuid(),
                Culture = "ru",
                TextDirection = "ltr",
                FontFamily = "Tahoma",
                FontScale = 1.0,
                CreatedAt = DateTime.UtcNow
            }
        };

        builder.Entity<CultureCustomization>().HasData(customizations);

    }
}
