using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class LocalizationService : ILocalizationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger&lt;LocalizationService&gt; _logger;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);

    public LocalizationService(
        ApplicationDbContext context, 
        ILogger&lt;LocalizationService&gt; logger,
        IMemoryCache cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task&lt;string&gt; GetStringAsync(string key, string culture = "az", Guid? companyId = null, Guid? tenantId = null)
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

    public async Task&lt;IEnumerable&lt;LocalizationResource&gt;&gt; GetAllAsync(string culture, Guid? companyId = null, Guid? tenantId = null)
    {
        try
        {
            var query = _context.LocalizationResources.Where(r =&gt; r.Culture == culture);
            
            if (companyId.HasValue)
                query = query.Where(r =&gt; r.CompanyId == companyId);
            else
                query = query.Where(r =&gt; r.CompanyId == null);

            if (tenantId.HasValue)
                query = query.Where(r =&gt; r.TenantId == tenantId);
            else
                query = query.Where(r =&gt; r.TenantId == null);

            return await query.OrderBy(r =&gt; r.Category).ThenBy(r =&gt; r.Key).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all localization resources for culture: {Culture}", culture);
            return Enumerable.Empty&lt;LocalizationResource&gt;();
        }
    }

    public async Task&lt;IEnumerable&lt;LocalizationResource&gt;&gt; GetByCategoryAsync(string category, string culture, Guid? companyId = null, Guid? tenantId = null)
    {
        try
        {
            var query = _context.LocalizationResources
                .Where(r =&gt; r.Category == category &amp;&amp; r.Culture == culture);
            
            if (companyId.HasValue)
                query = query.Where(r =&gt; r.CompanyId == companyId);
            else
                query = query.Where(r =&gt; r.CompanyId == null);

            if (tenantId.HasValue)
                query = query.Where(r =&gt; r.TenantId == tenantId);
            else
                query = query.Where(r =&gt; r.TenantId == null);

            return await query.OrderBy(r =&gt; r.Key).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting localization resources by category: {Category}, culture: {Culture}", category, culture);
            return Enumerable.Empty&lt;LocalizationResource&gt;();
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

    public async Task&lt;IEnumerable&lt;string&gt;&gt; GetSupportedCulturesAsync()
    {
        try
        {
            var cultures = await _context.LocalizationResources
                .Select(r =&gt; r.Culture)
                .Distinct()
                .OrderBy(c =&gt; c)
                .ToListAsync();

            return cultures.Any() ? cultures : new[] { "az", "en", "tr", "ru" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting supported cultures");
            return new[] { "az", "en", "tr", "ru" };
        }
    }

    public async Task&lt;Dictionary&lt;string, string&gt;&gt; GetAllStringsAsync(string culture, Guid? companyId = null, Guid? tenantId = null)
    {
        try
        {
            var resources = await GetAllAsync(culture, companyId, tenantId);
            return resources.ToDictionary(r =&gt; r.Key, r =&gt; r.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all strings for culture: {Culture}", culture);
            return new Dictionary&lt;string, string&gt;();
        }
    }

    private async Task&lt;LocalizationResource?&gt; GetLocalizationResourceAsync(string key, string culture, Guid? companyId, Guid? tenantId)
    {
        var query = _context.LocalizationResources
            .Where(r =&gt; r.Key == key &amp;&amp; r.Culture == culture);
        
        if (companyId.HasValue)
            query = query.Where(r =&gt; r.CompanyId == companyId);
        else
            query = query.Where(r =&gt; r.CompanyId == null);

        if (tenantId.HasValue)
            query = query.Where(r =&gt; r.TenantId == tenantId);
        else
            query = query.Where(r =&gt; r.TenantId == null);

        return await query.FirstOrDefaultAsync();
    }
}
