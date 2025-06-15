using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions&lt;ApplicationDbContext&gt; options)
        : base(options)
    {
    }

    // Core entities
    public DbSet&lt;Company&gt; Companies { get; set; }
    public DbSet&lt;Tenant&gt; Tenants { get; set; }
    public DbSet&lt;AppUser&gt; AppUsers { get; set; }
    public DbSet&lt;Role&gt; Roles { get; set; }
    public DbSet&lt;Permission&gt; Permissions { get; set; }
    public DbSet&lt;AuditLog&gt; AuditLogs { get; set; }
    public DbSet&lt;Configuration&gt; Configurations { get; set; }
    public DbSet&lt;LocalizationResource&gt; LocalizationResources { get; set; }
    public DbSet&lt;Notification&gt; Notifications { get; set; }
    public DbSet&lt;Device&gt; Devices { get; set; }
    public DbSet&lt;Plugin&gt; Plugins { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure entity relationships and constraints
        builder.Entity&lt;Company&gt;()
            .HasIndex(c =&gt; c.Code)
            .IsUnique();

        builder.Entity&lt;Tenant&gt;()
            .HasOne(t =&gt; t.Company)
            .WithMany(c =&gt; c.Tenants)
            .HasForeignKey(t =&gt; t.CompanyId);

        builder.Entity&lt;AppUser&gt;()
            .HasOne(u =&gt; u.Company)
            .WithMany(c =&gt; c.Users)
            .HasForeignKey(u =&gt; u.CompanyId);

        builder.Entity&lt;Configuration&gt;()
            .HasIndex(c =&gt; new { c.Key, c.TenantId })
            .IsUnique();

        builder.Entity&lt;LocalizationResource&gt;()
            .HasIndex(l =&gt; new { l.Key, l.Culture })
            .IsUnique();

        builder.Entity&lt;AuditLog&gt;()
            .HasIndex(a =&gt; a.Timestamp);

        builder.Entity&lt;AuditLog&gt;()
            .HasIndex(a =&gt; a.UserId);

        // Seed initial data
        SeedInitialData(builder);
    }

    private void SeedInitialData(ModelBuilder builder)
    {
        // Seed default company
        var defaultCompanyId = Guid.NewGuid();
        builder.Entity&lt;Company&gt;().HasData(new Company
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

        builder.Entity&lt;Configuration&gt;().HasData(configs);

        // Seed basic localization resources
        var localizationResources = new[]
        {
            // Azerbaijani
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Common.Welcome", Value = "Xoş gəlmisiniz", Culture = "az", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Common.Login", Value = "Daxil ol", Culture = "az", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Common.Logout", Value = "Çıxış", Culture = "az", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Navigation.Dashboard", Value = "İdarə paneli", Culture = "az", CreatedAt = DateTime.UtcNow },
            
            // English
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Common.Welcome", Value = "Welcome", Culture = "en", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Common.Login", Value = "Login", Culture = "en", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Common.Logout", Value = "Logout", Culture = "en", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Navigation.Dashboard", Value = "Dashboard", Culture = "en", CreatedAt = DateTime.UtcNow },
            
            // Turkish
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Common.Welcome", Value = "Hoş geldiniz", Culture = "tr", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Common.Login", Value = "Giriş", Culture = "tr", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Common.Logout", Value = "Çıkış", Culture = "tr", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Navigation.Dashboard", Value = "Kontrol Paneli", Culture = "tr", CreatedAt = DateTime.UtcNow },
            
            // Russian
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Common.Welcome", Value = "Добро пожаловать", Culture = "ru", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Common.Login", Value = "Войти", Culture = "ru", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Common.Logout", Value = "Выйти", Culture = "ru", CreatedAt = DateTime.UtcNow },
            new LocalizationResource { Id = Guid.NewGuid(), Key = "Navigation.Dashboard", Value = "Панель управления", Culture = "ru", CreatedAt = DateTime.UtcNow }
        };

        builder.Entity&lt;LocalizationResource&gt;().HasData(localizationResources);
    }
}
