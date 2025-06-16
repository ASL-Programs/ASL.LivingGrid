using ASL.LivingGrid.WebAdminPanel.Models;
using ASL.LivingGrid.WebAdminPanel.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using ASL.LivingGrid.WebAdminPanel.Tests;
using Moq;
using Xunit;

namespace ASL.LivingGrid.WebAdminPanel.Tests.Services;

public class PluginServiceTests
{
    [Fact]
    public async Task RemovePluginAsync_InvalidId_LogsError()
    {
        using var tempDir = new TemporaryDirectory();
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.SetupGet(e => e.ContentRootPath).Returns(tempDir.Path);
        var loggerMock = new Mock<ILogger<PluginService>>();
        var service = new PluginService(envMock.Object, loggerMock.Object);

        var plugin = new Plugin { Name = "Test", Version = "1.0" };
        await service.InstallPluginAsync(plugin);

        await service.RemovePluginAsync("not-a-guid");

        var installed = await service.GetInstalledPluginsAsync();
        Assert.Single(installed);
        loggerMock.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Invalid plugin id")),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
}
