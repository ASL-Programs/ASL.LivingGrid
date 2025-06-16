namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IMultiDbWizardService
{
    Task<IEnumerable<string>> GetSupportedTypesAsync();
    Task<bool> ApplyConfigurationAsync(string type, string connectionString);
}
