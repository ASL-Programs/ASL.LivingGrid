using System;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public record ProcessInfo(int Id, string Name, double CpuUsage, double MemoryMb, DateTime StartTime);

public interface IProcessMonitorService
{
    Task<IEnumerable<ProcessInfo>> ListAsync();
    Task<bool> KillAsync(int id);
}
