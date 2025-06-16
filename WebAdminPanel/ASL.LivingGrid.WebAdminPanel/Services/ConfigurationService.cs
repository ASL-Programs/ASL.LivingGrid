using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Models;
using ASL.LivingGrid.WebAdminPanel.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ConfigurationService> _logger;
    private readonly IAuditService _audit;
    private readonly ISecretStorageService _secret;

    public ConfigurationService(
        ApplicationDbContext context,
        ILogger<ConfigurationService> logger,
        IAuditService audit,
        ISecretStorageService secret)
    {
        _context = context;
        _logger = logger;
        _audit = audit;
        _secret = secret;
    }

    public async Task<string?> GetValueAsync(string key, Guid? companyId = null, Guid? tenantId = null)
    {
        try
        {
            var config = await GetConfigurationAsync(key, companyId, tenantId);
            if (config == null)
                return null;

            var val = config.Value;
            if (config.IsEncrypted && val != null)
            {
                val = await _secret.DecryptAsync(val);
            }
            return val;
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

    public async Task SetValueAsync(string key, string? value, Guid? companyId = null, Guid? tenantId = null, bool isSecret = false)
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
                config.IsEncrypted = isSecret;
                _context.Configurations.Add(config);
                await _context.SaveChangesAsync();
                await _audit.LogAsync("Create", nameof(Configuration), config.Id.ToString(), null, null, null, new { Value = value });
            }
            else
            {
                var old = config.Value;
                if (config.IsEncrypted)
                {
                    old = string.IsNullOrEmpty(old) ? null : await _secret.DecryptAsync(old);
                }

                if (config.IsEncrypted || isSecret)
                {
                    config.IsEncrypted = true;
                    config.Value = await _secret.EncryptAsync(value ?? string.Empty);
                }
                else
                {
                    config.Value = value;
                }
                config.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                await _audit.LogAsync("Update", nameof(Configuration), config.Id.ToString(), null, null, new { Value = old }, new { Value = value });
            }

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

    public async Task<bool> RollbackValueAsync(string key, Guid? companyId = null, Guid? tenantId = null)
    {
        var config = await GetConfigurationAsync(key, companyId, tenantId);
        if (config == null)
            return false;

        var lastAudit = await _context.AuditLogs
            .Where(a => a.EntityType == nameof(Configuration) && a.EntityId == config.Id.ToString())
            .OrderByDescending(a => a.Timestamp)
            .FirstOrDefaultAsync();

        if (lastAudit == null || string.IsNullOrEmpty(lastAudit.OldValues))
            return false;

        try
        {
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(lastAudit.OldValues!);
            if (dict == null || !dict.TryGetValue("Value", out var oldValue))
                return false;

            if (config.IsEncrypted)
                config.Value = await _secret.EncryptAsync(oldValue);
            else
                config.Value = oldValue;

            config.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await _audit.LogAsync("Rollback", nameof(Configuration), config.Id.ToString(), null, null, null, new { Value = oldValue });
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling back configuration: {Key}", key);
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
