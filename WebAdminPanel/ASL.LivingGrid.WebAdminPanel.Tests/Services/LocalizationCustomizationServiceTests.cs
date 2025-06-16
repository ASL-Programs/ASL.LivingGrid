using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Models;
using ASL.LivingGrid.WebAdminPanel.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ASL.LivingGrid.WebAdminPanel.Tests.Services;

public class LocalizationCustomizationServiceTests
{
    private static ApplicationDbContext GetDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetAsync_ReturnsCustomization_WhenExists()
    {
        using var context = GetDbContext(nameof(GetAsync_ReturnsCustomization_WhenExists));
        var customization = new CultureCustomization { Culture = "en", Module = "General", TextDirection = "ltr" };
        context.CultureCustomizations.Add(customization);
        context.SaveChanges();
        var service = new LocalizationCustomizationService(context);

        var result = await service.GetAsync("en");

        Assert.NotNull(result);
        Assert.Equal("en", result!.Culture);
    }

    [Fact]
    public async Task SetAsync_InsertsOrUpdatesCustomization()
    {
        using var context = GetDbContext(nameof(SetAsync_InsertsOrUpdatesCustomization));
        var service = new LocalizationCustomizationService(context);
        var customization = new CultureCustomization { Culture = "en", Module = "General", TextDirection = "ltr" };

        await service.SetAsync(customization);
        Assert.Equal(1, context.CultureCustomizations.Count());

        customization.TextDirection = "rtl";
        await service.SetAsync(customization);

        Assert.Equal(1, context.CultureCustomizations.Count());
        Assert.Equal("rtl", context.CultureCustomizations.First().TextDirection);
    }

    [Fact]
    public async Task GetTemplateAsync_ReturnsTemplate_WhenExists()
    {
        using var context = GetDbContext(nameof(GetTemplateAsync_ReturnsTemplate_WhenExists));
        var template = new TemplateOverride { Culture = "en", Module = "General", TemplateKey = "Key", TemplateContent = "Content" };
        context.TemplateOverrides.Add(template);
        context.SaveChanges();
        var service = new LocalizationCustomizationService(context);

        var result = await service.GetTemplateAsync("en", "General");

        Assert.NotNull(result);
        Assert.Equal("Key", result!.TemplateKey);
    }

    [Fact]
    public async Task SetTemplateAsync_InsertsOrUpdatesTemplate()
    {
        using var context = GetDbContext(nameof(SetTemplateAsync_InsertsOrUpdatesTemplate));
        var service = new LocalizationCustomizationService(context);
        var template = new TemplateOverride { Culture = "en", Module = "General", TemplateKey = "Key", TemplateContent = "Content" };

        await service.SetTemplateAsync(template);
        Assert.Equal(1, context.TemplateOverrides.Count());

        template.TemplateContent = "Updated";
        await service.SetTemplateAsync(template);

        Assert.Equal(1, context.TemplateOverrides.Count());
        Assert.Equal("Updated", context.TemplateOverrides.First().TemplateContent);
    }

    [Fact]
    public async Task GetTerminologyAsync_ReturnsTerm_WhenExists()
    {
        using var context = GetDbContext(nameof(GetTerminologyAsync_ReturnsTerm_WhenExists));
        var term = new TerminologyOverride { Culture = "en", Module = "General", Key = "Hello", Value = "World" };
        context.TerminologyOverrides.Add(term);
        context.SaveChanges();
        var service = new LocalizationCustomizationService(context);

        var result = await service.GetTerminologyAsync("Hello", "en", "General");

        Assert.NotNull(result);
        Assert.Equal("World", result!.Value);
    }

    [Fact]
    public async Task SetTerminologyAsync_InsertsOrUpdatesTerm()
    {
        using var context = GetDbContext(nameof(SetTerminologyAsync_InsertsOrUpdatesTerm));
        var service = new LocalizationCustomizationService(context);
        var term = new TerminologyOverride { Culture = "en", Module = "General", Key = "Hello", Value = "World" };

        await service.SetTerminologyAsync(term);
        Assert.Equal(1, context.TerminologyOverrides.Count());

        term.Value = "Universe";
        await service.SetTerminologyAsync(term);

        Assert.Equal(1, context.TerminologyOverrides.Count());
        Assert.Equal("Universe", context.TerminologyOverrides.First().Value);
    }

    [Fact]
    public async Task GetAsync_FallsBackToGeneralModule_WhenModuleNull()
    {
        using var context = GetDbContext(nameof(GetAsync_FallsBackToGeneralModule_WhenModuleNull));
        var general = new CultureCustomization { Culture = "en", Module = "General", TextDirection = "ltr" };
        var other = new CultureCustomization { Culture = "en", Module = "Other", TextDirection = "rtl" };
        context.CultureCustomizations.AddRange(general, other);
        context.SaveChanges();
        var service = new LocalizationCustomizationService(context);

        var result = await service.GetAsync("en", module: null);

        Assert.NotNull(result);
        Assert.Equal("General", result!.Module);
    }

    [Fact]
    public async Task GetAsync_FallsBackToGlobalCompany_WhenCompanyIdNull()
    {
        using var context = GetDbContext(nameof(GetAsync_FallsBackToGlobalCompany_WhenCompanyIdNull));
        var global = new CultureCustomization { Culture = "en", Module = "General", TextDirection = "ltr" };
        var company = new CultureCustomization { Culture = "en", Module = "General", CompanyId = Guid.NewGuid(), TextDirection = "rtl" };
        context.CultureCustomizations.AddRange(global, company);
        context.SaveChanges();
        var service = new LocalizationCustomizationService(context);

        var result = await service.GetAsync("en", companyId: null);

        Assert.NotNull(result);
        Assert.Null(result!.CompanyId);
    }

    [Fact]
    public async Task GetAsync_FallsBackToGlobalTenant_WhenTenantIdNull()
    {
        using var context = GetDbContext(nameof(GetAsync_FallsBackToGlobalTenant_WhenTenantIdNull));
        var global = new CultureCustomization { Culture = "en", Module = "General", CompanyId = Guid.NewGuid(), TextDirection = "ltr" };
        var tenant = new CultureCustomization { Culture = "en", Module = "General", CompanyId = global.CompanyId, TenantId = Guid.NewGuid(), TextDirection = "rtl" };
        context.CultureCustomizations.AddRange(global, tenant);
        context.SaveChanges();
        var service = new LocalizationCustomizationService(context);

        var result = await service.GetAsync("en", global.CompanyId, tenantId: null);

        Assert.NotNull(result);
        Assert.Null(result!.TenantId);
    }

    [Fact]
    public async Task GetAsync_ReturnsCustomization_ScopedByCompanyAndTenant()
    {
        using var context = GetDbContext(nameof(GetAsync_ReturnsCustomization_ScopedByCompanyAndTenant));
        var companyId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var scoped = new CultureCustomization { Culture = "en", Module = "General", CompanyId = companyId, TenantId = tenantId, TextDirection = "ltr" };
        var other = new CultureCustomization { Culture = "en", Module = "General", TextDirection = "rtl" };
        context.CultureCustomizations.AddRange(scoped, other);
        context.SaveChanges();
        var service = new LocalizationCustomizationService(context);

        var result = await service.GetAsync("en", companyId, tenantId, "General");

        Assert.NotNull(result);
        Assert.Equal(companyId, result!.CompanyId);
        Assert.Equal(tenantId, result.TenantId);
    }

    [Fact]
    public async Task SetAsync_UpsertsCustomization_PerCompanyAndTenant()
    {
        using var context = GetDbContext(nameof(SetAsync_UpsertsCustomization_PerCompanyAndTenant));
        var service = new LocalizationCustomizationService(context);
        var companyId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var customization = new CultureCustomization { Culture = "en", Module = "General", CompanyId = companyId, TenantId = tenantId, TextDirection = "ltr" };

        await service.SetAsync(customization);
        Assert.Equal(1, context.CultureCustomizations.Count());

        customization.TextDirection = "rtl";
        await service.SetAsync(customization);

        Assert.Equal(1, context.CultureCustomizations.Count());
        var updated = context.CultureCustomizations.First();
        Assert.Equal("rtl", updated.TextDirection);
        Assert.Equal(companyId, updated.CompanyId);
        Assert.Equal(tenantId, updated.TenantId);
    }
}
