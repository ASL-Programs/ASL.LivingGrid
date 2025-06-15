using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class LocalizationService : ILocalizationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<LocalizationService> _logger;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);

    public LocalizationService(
        ApplicationDbContext context, 
        ILogger<LocalizationService> logger,
        IMemoryCache cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<string> GetStringAsync(string key, string culture = "az", Guid? companyId = null, Guid? tenantId = null)
    {
        try
        {
            var cacheKey = $"localization_{key}_{culture}_{companyId}_{tenantId}";
            
            if (_cache.TryGetValue(cacheKey, out string? cachedValue) && !string.IsNullOrEmpty(cachedValue))
            {
                return cachedValue;
            }

            var resource = await GetLocalizationResourceAsync(key, culture, companyId, tenantId);
            var value = resource?.Value ?? key; // Return key if translation not found

            _cache.Set(cacheKey, value, _cacheExpiration);
            return value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting localization string for key: {Key}, culture: {Culture}", key, culture);
            return key; // Return key as fallback
        }
    }

    public async Task<IEnumerable<LocalizationResource>> GetAllAsync(string culture, Guid? companyId = null, Guid? tenantId = null)
    {
        try
        {
            var query = _context.LocalizationResources.Where(r => r.Culture == culture);
            
            if (companyId.HasValue)
                query = query.Where(r => r.CompanyId == companyId);
            else
                query = query.Where(r => r.CompanyId == null);

            if (tenantId.HasValue)
                query = query.Where(r => r.TenantId == tenantId);
            else
                query = query.Where(r => r.TenantId == null);

            return await query.OrderBy(r => r.Category).ThenBy(r => r.Key).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all localization resources for culture: {Culture}", culture);
            return Enumerable.Empty<LocalizationResource>();
        }
    }

    public async Task<IEnumerable<LocalizationResource>> GetByCategoryAsync(string category, string culture, Guid? companyId = null, Guid? tenantId = null)
    {
        try
        {
            var query = _context.LocalizationResources
                .Where(r => r.Category == category && r.Culture == culture);
            
            if (companyId.HasValue)
                query = query.Where(r => r.CompanyId == companyId);
            else
                query = query.Where(r => r.CompanyId == null);

            if (tenantId.HasValue)
                query = query.Where(r => r.TenantId == tenantId);
            else
                query = query.Where(r => r.TenantId == null);

            return await query.OrderBy(r => r.Key).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting localization resources by category: {Category}, culture: {Culture}", category, culture);
            return Enumerable.Empty<LocalizationResource>();
        }
    }

    public async Task SetStringAsync(string key, string value, string culture, Guid? companyId = null, Guid? tenantId = null)
    {
        try
        {
            var resource = await GetLocalizationResourceAsync(key, culture, companyId, tenantId);
            
            if (resource == null)
            {
                resource = new LocalizationResource
                {
                    Key = key,
                    Value = value,
                    Culture = culture,
                    CompanyId = companyId,
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.LocalizationResources.Add(resource);
            }
            else
            {
                var version = new LocalizationResourceVersion
                {
                    ResourceId = resource.Id,
                    Value = resource.Value,
                    Version = resource.Versions.Count + 1,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUser = resource.UpdatedBy
                };
                _context.LocalizationResourceVersions.Add(version);

                resource.Value = value;
                resource.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            // Clear cache
            var cacheKey = $"localization_{key}_{culture}_{companyId}_{tenantId}";
            _cache.Remove(cacheKey);

            _logger.LogInformation("Localization resource updated: {Key} = {Value} ({Culture})", key, value, culture);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting localization string for key: {Key}, culture: {Culture}", key, culture);
            throw;
        }
    }

    public async Task DeleteAsync(string key, string culture, Guid? companyId = null, Guid? tenantId = null)
    {
        try
        {
            var resource = await GetLocalizationResourceAsync(key, culture, companyId, tenantId);
            if (resource != null)
            {
                var version = new LocalizationResourceVersion
                {
                    ResourceId = resource.Id,
                    Value = resource.Value,
                    Version = resource.Versions.Count + 1,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUser = resource.UpdatedBy
                };
                _context.LocalizationResourceVersions.Add(version);

                _context.LocalizationResources.Remove(resource);
                await _context.SaveChangesAsync();

                // Clear cache
                var cacheKey = $"localization_{key}_{culture}_{companyId}_{tenantId}";
                _cache.Remove(cacheKey);

                _logger.LogInformation("Localization resource deleted: {Key} ({Culture})", key, culture);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting localization resource: {Key}, culture: {Culture}", key, culture);
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetSupportedCulturesAsync()
    {
        try
        {
            var cultures = await _context.LocalizationResources
                .Select(r => r.Culture)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            return cultures.Any() ? cultures : new[] { "az", "en", "tr", "ru" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting supported cultures");
            return new[] { "az", "en", "tr", "ru" };
        }
    }

    public async Task<Dictionary<string, string>> GetAllStringsAsync(string culture, Guid? companyId = null, Guid? tenantId = null)
    {
        try
        {
            var resources = await GetAllAsync(culture, companyId, tenantId);
            return resources.ToDictionary(r => r.Key, r => r.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all strings for culture: {Culture}", culture);
            return new Dictionary<string, string>();
        }
    }

    public async Task BulkSetAsync(IEnumerable<LocalizationResource> resources)
    {
        foreach (var r in resources)
        {
            await SetStringAsync(r.Key, r.Value, r.Culture, r.CompanyId, r.TenantId);
        }
    }

    public async Task<string> ExportAsync(string culture, Guid? companyId = null, Guid? tenantId = null)
    {
        var strings = await GetAllStringsAsync(culture, companyId, tenantId);
        return System.Text.Json.JsonSerializer.Serialize(strings);
    }

    public async Task ImportAsync(string jsonContent, string culture, Guid? companyId = null, Guid? tenantId = null)
    {
        var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent) ?? new();
        foreach (var kv in data)
        {
            await SetStringAsync(kv.Key, kv.Value, culture, companyId, tenantId);
        }
    }

    public async Task<IEnumerable<LocalizationResourceVersion>> GetHistoryAsync(Guid resourceId)
    {
        return await _context.LocalizationResourceVersions
            .Where(v => v.ResourceId == resourceId)
            .OrderByDescending(v => v.Version)
            .ToListAsync();
    }

    public async Task ApproveAsync(Guid resourceId, string approvedBy)
    {
        var resource = await _context.LocalizationResources.FindAsync(resourceId);
        if (resource == null) return;

        resource.IsApproved = true;
        resource.ApprovedBy = approvedBy;
        resource.ApprovedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    private async Task<LocalizationResource?> GetLocalizationResourceAsync(string key, string culture, Guid? companyId, Guid? tenantId)
    {
        var query = _context.LocalizationResources
            .Where(r => r.Key == key && r.Culture == culture);
        
        if (companyId.HasValue)
            query = query.Where(r => r.CompanyId == companyId);
        else
            query = query.Where(r => r.CompanyId == null);

        if (tenantId.HasValue)
            query = query.Where(r => r.TenantId == tenantId);
        else
            query = query.Where(r => r.TenantId == null);

        return await query.FirstOrDefaultAsync();
    }
}
