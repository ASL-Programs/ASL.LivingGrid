using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Models;
using Microsoft.EntityFrameworkCore;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class LocalizationCustomizationService : ILocalizationCustomizationService
{
    private readonly ApplicationDbContext _context;

    public LocalizationCustomizationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CultureCustomization?> GetAsync(string culture, Guid? companyId = null, Guid? tenantId = null, string? module = null)
    {
        var query = _context.CultureCustomizations.AsQueryable();
        query = query.Where(c => c.Culture == culture);
        if (companyId.HasValue)
            query = query.Where(c => c.CompanyId == companyId);
        else
            query = query.Where(c => c.CompanyId == null);
        if (tenantId.HasValue)
            query = query.Where(c => c.TenantId == tenantId);
        else
            query = query.Where(c => c.TenantId == null);
        if (!string.IsNullOrEmpty(module))
            query = query.Where(c => c.Module == module);
        else
            query = query.Where(c => c.Module == "General");

        return await query.FirstOrDefaultAsync();
    }

    public async Task SetAsync(CultureCustomization customization)
    {
        var existing = await _context.CultureCustomizations.FirstOrDefaultAsync(c => c.Culture == customization.Culture && c.CompanyId == customization.CompanyId && c.TenantId == customization.TenantId && c.Module == customization.Module);
        if (existing == null)
        {
            _context.CultureCustomizations.Add(customization);
        }
        else
        {
            existing.TextDirection = customization.TextDirection;
            existing.FontFamily = customization.FontFamily;
            existing.FontScale = customization.FontScale;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();
    }

    public async Task<TemplateOverride?> GetTemplateAsync(string culture, string module, Guid? companyId = null, Guid? tenantId = null)
    {
        return await _context.TemplateOverrides.FirstOrDefaultAsync(t => t.Culture == culture && t.Module == module && t.CompanyId == companyId && t.TenantId == tenantId);
    }

    public async Task SetTemplateAsync(TemplateOverride template)
    {
        var existing = await _context.TemplateOverrides.FirstOrDefaultAsync(t => t.Culture == template.Culture && t.Module == template.Module && t.CompanyId == template.CompanyId && t.TenantId == template.TenantId && t.TemplateKey == template.TemplateKey);
        if (existing == null)
        {
            _context.TemplateOverrides.Add(template);
        }
        else
        {
            existing.TemplateContent = template.TemplateContent;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();
    }

    public async Task<TerminologyOverride?> GetTerminologyAsync(string key, string culture, string module, Guid? companyId = null, Guid? tenantId = null)
    {
        return await _context.TerminologyOverrides.FirstOrDefaultAsync(t => t.Key == key && t.Culture == culture && t.Module == module && t.CompanyId == companyId && t.TenantId == tenantId);
    }

    public async Task SetTerminologyAsync(TerminologyOverride term)
    {
        var existing = await _context.TerminologyOverrides.FirstOrDefaultAsync(t => t.Key == term.Key && t.Culture == term.Culture && t.Module == term.Module && t.CompanyId == term.CompanyId && t.TenantId == term.TenantId);
        if (existing == null)
        {
            _context.TerminologyOverrides.Add(term);
        }
        else
        {
            existing.Value = term.Value;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();
    }
}
