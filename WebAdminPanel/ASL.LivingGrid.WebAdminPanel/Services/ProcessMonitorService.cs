using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class ProcessMonitorService : IProcessMonitorService
{
    private readonly ILogger<ProcessMonitorService> _logger;
    private readonly Dictionary<int, (TimeSpan Cpu, DateTime Timestamp)> _samples = new();
    private readonly int _processorCount = Environment.ProcessorCount;

    public ProcessMonitorService(ILogger<ProcessMonitorService> logger)
    {
        _logger = logger;
    }

    public Task<IEnumerable<ProcessInfo>> ListAsync()
    {
        var processes = Process.GetProcesses().Take(20);
        var now = DateTime.UtcNow;
        var list = new List<ProcessInfo>();

        foreach (var p in processes)
        {
            double memory = 0;
            double cpu = 0;
            DateTime startTime = DateTime.MinValue;
            try
            {
                memory = p.WorkingSet64 / 1024d / 1024d;
                startTime = p.StartTime;
                var totalCpu = p.TotalProcessorTime;

                if (_samples.TryGetValue(p.Id, out var sample))
                {
                    var deltaCpu = totalCpu - sample.Cpu;
                    var deltaTime = now - sample.Timestamp;
                    if (deltaTime.TotalMilliseconds > 0)
                    {
                        cpu = deltaCpu.TotalMilliseconds / (deltaTime.TotalMilliseconds * _processorCount) * 100;
                    }
                    _samples[p.Id] = (totalCpu, now);
                }
                else
                {
                    _samples[p.Id] = (totalCpu, now);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to read process info {Id}", p.Id);
            }

            list.Add(new ProcessInfo(p.Id, p.ProcessName, cpu, memory, startTime));
        }

        var activeIds = processes.Select(pr => pr.Id).ToHashSet();
        var remove = _samples.Keys.Where(id => !activeIds.Contains(id)).ToList();
        foreach (var id in remove)
        {
            _samples.Remove(id);
        }

        return Task.FromResult<IEnumerable<ProcessInfo>>(list);
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
