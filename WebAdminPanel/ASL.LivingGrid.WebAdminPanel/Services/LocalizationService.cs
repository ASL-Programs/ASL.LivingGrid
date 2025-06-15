using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Text.RegularExpressions;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class LocalizationService : ILocalizationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<LocalizationService> _logger;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);

    /// <inheritdoc />
    public event Action<string, string>? MissingTranslation;

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
            string value;
            if (resource == null)
            {
                value = key; // Return key if translation not found
                MissingTranslation?.Invoke(key, culture);
            }
            else
            {
                value = resource.Value;
            }

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

    public async Task<Dictionary<string, double>> GetCoverageByCategoryAsync(string culture)
    {
        const string defaultCulture = "az";

        var total = await _context.LocalizationResources
            .Where(r => r.Culture == defaultCulture)
            .GroupBy(r => r.Category)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .ToListAsync();

        var translated = await _context.LocalizationResources
            .Where(r => r.Culture == culture)
            .GroupBy(r => r.Category)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Category, g => g.Count);

        var result = new Dictionary<string, double>();
        foreach (var cat in total)
        {
            translated.TryGetValue(cat.Category, out var c);
            var pct = cat.Count == 0 ? 100 : (double)c * 100 / cat.Count;
            result[cat.Category] = Math.Round(pct, 2);
        }

        return result;
    }

    public async Task<IEnumerable<string>> GetMissingKeysAsync(string culture)
    {
        const string defaultCulture = "az";

        var defaultKeys = await _context.LocalizationResources
            .Where(r => r.Culture == defaultCulture)
            .Select(r => r.Key)
            .ToListAsync();

        var targetKeys = await _context.LocalizationResources
            .Where(r => r.Culture == culture)
            .Select(r => r.Key)
            .ToListAsync();

        return defaultKeys.Except(targetKeys);
    }

    public async Task<IEnumerable<string>> ValidatePlaceholdersAsync(string culture)
    {
        try
        {
            const string defaultCulture = "az";
            var source = await GetAllStringsAsync(defaultCulture);
            var target = await GetAllStringsAsync(culture);
            var issues = new List<string>();
            foreach (var kv in target)
            {
                if (!source.TryGetValue(kv.Key, out var srcValue))
                    continue;

                var srcPh = ExtractPlaceholders(srcValue);
                var tgtPh = ExtractPlaceholders(kv.Value);
                if (!srcPh.SequenceEqual(tgtPh))
                    issues.Add(kv.Key);
            }
            return issues;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating placeholders for culture {Culture}", culture);
            return Enumerable.Empty<string>();
        }
    }

    public async Task<Dictionary<string, int>> GetOverflowStringsAsync(string culture, int maxLength = 60)
    {
        try
        {
            var strings = await GetAllStringsAsync(culture);
            var result = new Dictionary<string, int>();
            foreach (var kv in strings)
            {
                if (kv.Value != null && kv.Value.Length > maxLength)
                    result[kv.Key] = kv.Value.Length;
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking overflow strings for culture {Culture}", culture);
            return new Dictionary<string, int>();
        }
    }

    private static IEnumerable<string> ExtractPlaceholders(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Array.Empty<string>();

        var matches = System.Text.RegularExpressions.Regex.Matches(text, "{[^}]+}");
        return matches.Cast<System.Text.RegularExpressions.Match>().Select(m => m.Value);
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
