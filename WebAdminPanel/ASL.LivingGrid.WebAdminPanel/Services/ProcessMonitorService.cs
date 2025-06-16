using System.Diagnostics;
using System.Linq;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class ProcessMonitorService : IProcessMonitorService
{
    private readonly ILogger<ProcessMonitorService> _logger;

    public ProcessMonitorService(ILogger<ProcessMonitorService> logger)
    {
        _logger = logger;
    }

    public Task<IEnumerable<ProcessInfo>> ListAsync()
    {
        var list = Process.GetProcesses().Take(20).Select(p =>
        {
            double memory = p.WorkingSet64 / 1024d / 1024d;
            double cpu = 0; // placeholder
            return new ProcessInfo(p.Id, p.ProcessName, cpu, memory);
        });
        return Task.FromResult(list);
    }

    public Task<bool> KillAsync(int id)
    {
        try
        {
            var proc = Process.GetProcessById(id);
            proc.Kill();
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to kill process {Id}", id);
            return Task.FromResult(false);
        }
    }
}
