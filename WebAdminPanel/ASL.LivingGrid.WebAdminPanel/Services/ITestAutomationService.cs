namespace ASL.LivingGrid.WebAdminPanel.Services;

using ASL.LivingGrid.WebAdminPanel.Models;

public interface ITestAutomationService
{
    Task<TestRunResult> RunTestsAsync();
    IEnumerable<TestRunResult> GetHistory();
    Task CheckForUpdatesAsync();
}
