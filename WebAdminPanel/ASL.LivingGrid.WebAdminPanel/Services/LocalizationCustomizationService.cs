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

    public async Task<CultureCustomization?> GetAsync(string culture)
    {
        return await _context.CultureCustomizations.FirstOrDefaultAsync(c => c.Culture == culture);
    }

    public async Task SetAsync(CultureCustomization customization)
    {
        var existing = await _context.CultureCustomizations.FirstOrDefaultAsync(c => c.Culture == customization.Culture);
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
}
