using System.Linq;
using System.Threading.Tasks;
using ASL.LivingGrid.WebAdminPanel.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ASL.LivingGrid.WebAdminPanel.Tests.Services;

public class ProcessMonitorServiceTests
{
    [Fact]
    public async Task ListAsync_ComputesCpuUsage()
    {
        var service = new ProcessMonitorService(new NullLogger<ProcessMonitorService>());
        // first call initializes samples
        await service.ListAsync();
        await Task.Delay(100);
        var second = await service.ListAsync();

        Assert.NotEmpty(second);
        Assert.All(second, p => Assert.True(p.CpuUsage >= 0));
    }
}
