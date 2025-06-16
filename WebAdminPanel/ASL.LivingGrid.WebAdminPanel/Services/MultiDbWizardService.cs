using ASL.LivingGrid.WebAdminPanel.Data;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class MultiDbWizardService : IMultiDbWizardService
{
    private readonly IConfigurationService _config;
    private readonly ILogger<MultiDbWizardService> _logger;

    public MultiDbWizardService(IConfigurationService config, ILogger<MultiDbWizardService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public Task<IEnumerable<string>> GetSupportedTypesAsync()
    {
        IEnumerable<string> types = new[] { "SQLServer", "PostgreSQL", "SQLite" };
        return Task.FromResult(types);
    }

    public async Task<bool> ApplyConfigurationAsync(string type, string connectionString)
    {
        try
        {
            await _config.SetValueAsync("Database:Type", type);
            await _config.SetValueAsync("ConnectionStrings:DefaultConnection", connectionString);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply DB config");
            return false;
        }
    }
}
