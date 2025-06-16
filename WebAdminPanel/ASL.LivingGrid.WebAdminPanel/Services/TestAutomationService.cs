using System.Diagnostics;
using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class TestAutomationService : ITestAutomationService
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<TestAutomationService> _logger;
    private readonly List<TestRunResult> _history = new();

    public TestAutomationService(IWebHostEnvironment env, ILogger<TestAutomationService> logger)
    {
        _env = env;
        _logger = logger;
    }

    public async Task<TestRunResult> RunTestsAsync()
    {
        var solutionDir = Path.Combine(_env.ContentRootPath, "..");
        var psi = new ProcessStartInfo("dotnet", "test --no-build")
        {
            WorkingDirectory = solutionDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using var proc = Process.Start(psi)!;
        var output = await proc.StandardOutput.ReadToEndAsync();
        output += await proc.StandardError.ReadToEndAsync();
        await proc.WaitForExitAsync();

        var result = new TestRunResult
        {
            Timestamp = DateTime.Now,
            Success = proc.ExitCode == 0,
            Log = output
        };
        _history.Add(result);
        return result;
    }

    public IEnumerable<TestRunResult> GetHistory() => _history;

    public async Task CheckForUpdatesAsync()
    {
        var updateMarker = Path.Combine(_env.ContentRootPath, "tests.update");
        if (File.Exists(updateMarker))
        {
            _logger.LogInformation("Test update detected, executing tests...");
            await RunTestsAsync();
            File.Delete(updateMarker);
        }
    }
}
