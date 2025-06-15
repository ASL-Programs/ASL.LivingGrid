using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationService(ApplicationDbContext context, ILogger<ConfigurationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<string?> GetValueAsync(string key, Guid? companyId = null, Guid? tenantId = null)
    {
        try
        {
            var config = await GetConfigurationAsync(key, companyId, tenantId);
            return config?.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting configuration value for key: {Key}", key);
            return null;
        }
    }

    public async Task<T?> GetValueAsync<T>(string key, Guid? companyId = null, Guid? tenantId = null)
    {
        try
        {
            var value = await GetValueAsync(key, companyId, tenantId);
            if (string.IsNullOrEmpty(value))
                return default;

            if (typeof(T) == typeof(string))
                return (T)(object)value;

            if (typeof(T) == typeof(bool))
                return (T)(object)bool.Parse(value);

            if (typeof(T) == typeof(int))
                return (T)(object)int.Parse(value);

            if (typeof(T) == typeof(decimal))
                return (T)(object)decimal.Parse(value);

            // Try to deserialize as JSON for complex types
            return JsonSerializer.Deserialize<T>(value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing configuration value for key: {Key}", key);
            return default;
        }
    }

    public async Task SetValueAsync(string key, string? value, Guid? companyId = null, Guid? tenantId = null)
    {
        try
        {
            var config = await GetConfigurationAsync(key, companyId, tenantId);
            
            if (config == null)
            {
                config = new Configuration
                {
                    Key = key,
                    Value = value,
                    CompanyId = companyId,
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Configurations.Add(config);
            }
            else
            {
                config.Value = value;
                config.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Configuration updated: {Key} = {Value}", key, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting configuration value for key: {Key}", key);
            throw;
        }
    }

    public async Task<IEnumerable<Configuration>> GetAllAsync(Guid? companyId = null, Guid? tenantId = null)
    {
        try
        {
            var query = _context.Configurations.AsQueryable();
            
            if (companyId.HasValue)
                query = query.Where(c => c.CompanyId == companyId);
            else
                query = query.Where(c => c.CompanyId == null);

            if (tenantId.HasValue)
                query = query.Where(c => c.TenantId == tenantId);
            else
                query = query.Where(c => c.TenantId == null);

            return await query.OrderBy(c => c.Category).ThenBy(c => c.Key).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all configurations");
            return Enumerable.Empty<Configuration>();
        }
    }

    public async Task<IEnumerable<Configuration>> GetByCategoryAsync(string category, Guid? companyId = null, Guid? tenantId = null)
    {
        try
        {
            var query = _context.Configurations.Where(c => c.Category == category);
            
            if (companyId.HasValue)
                query = query.Where(c => c.CompanyId == companyId);
            else
                query = query.Where(c => c.CompanyId == null);

            if (tenantId.HasValue)
                query = query.Where(c => c.TenantId == tenantId);
            else
                query = query.Where(c => c.TenantId == null);

            return await query.OrderBy(c => c.Key).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting configurations by category: {Category}", category);
            return Enumerable.Empty<Configuration>();
        }
    }

    public async Task DeleteAsync(string key, Guid? companyId = null, Guid? tenantId = null)
    {
        try
        {
            var config = await GetConfigurationAsync(key, companyId, tenantId);
            if (config != null)
            {
                _context.Configurations.Remove(config);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Configuration deleted: {Key}", key);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting configuration: {Key}", key);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string key, Guid? companyId = null, Guid? tenantId = null)
    {
        try
        {
            var config = await GetConfigurationAsync(key, companyId, tenantId);
            return config != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if configuration exists: {Key}", key);
            return false;
        }
    }

    private async Task<Configuration?> GetConfigurationAsync(string key, Guid? companyId, Guid? tenantId)
    {
        var query = _context.Configurations.Where(c => c.Key == key);
        
        if (companyId.HasValue)
            query = query.Where(c => c.CompanyId == companyId);
        else
            query = query.Where(c => c.CompanyId == null);

        if (tenantId.HasValue)
            query = query.Where(c => c.TenantId == tenantId);
        else
            query = query.Where(c => c.TenantId == null);

        return await query.FirstOrDefaultAsync();
    }
}
