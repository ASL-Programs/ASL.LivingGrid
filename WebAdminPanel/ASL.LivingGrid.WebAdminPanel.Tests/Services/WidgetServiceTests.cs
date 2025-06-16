using System.Security.Claims;
using ASL.LivingGrid.WebAdminPanel.Models;
using ASL.LivingGrid.WebAdminPanel.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Moq;
using Xunit;

namespace ASL.LivingGrid.WebAdminPanel.Tests.Services;

public class WidgetServiceTests
{
    [Fact]
    public async Task InstallAndRemoveWidget_PersistsChanges()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.SetupGet(e => e.ContentRootPath).Returns(tempDir);
        var jsMock = new Mock<IJSRuntime>();
        var loggerMock = new Mock<ILogger<WidgetService>>();
        var service = new WidgetService(envMock.Object, jsMock.Object, loggerMock.Object);

        var widget = new WidgetDefinition { Id = "w1", Name = "Widget" };
        await service.InstallWidgetAsync(widget);
        var installed = await service.GetInstalledWidgetsAsync();
        Assert.Single(installed);

        await service.RemoveWidgetAsync("w1");
        installed = await service.GetInstalledWidgetsAsync();
        Assert.Empty(installed);
    }

    [Fact]
    public async Task GetAndSaveUserWidgets_UsesJsRuntime()
    {
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.SetupGet(e => e.ContentRootPath).Returns(Path.GetTempPath());
        var jsMock = new Mock<IJSRuntime>();
        jsMock.Setup(j => j.InvokeAsync<string?>("aslWidgets.getWidgets", It.IsAny<object?[]>()))
            .ReturnsAsync("[\"w1\"]");
        var loggerMock = new Mock<ILogger<WidgetService>>();
        var service = new WidgetService(envMock.Object, jsMock.Object, loggerMock.Object);

        var widgets = await service.GetUserWidgetsAsync("c", "u");
        Assert.Single(widgets);
        Assert.Equal("w1", widgets[0]);

        await service.SaveUserWidgetsAsync("c", "u", new List<string> { "w1", "w2" });
        jsMock.Verify(j => j.InvokeVoidAsync("aslWidgets.saveWidgets", It.Is<object[]>(o =>
            o[0]!.Equals("c") && o[1]!.Equals("u") && o[2]!.ToString()!.Contains("w2"))), Times.Once);
    }

    [Fact]
    public async Task UsageTracking_CallsJsRuntime()
    {
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.SetupGet(e => e.ContentRootPath).Returns(Path.GetTempPath());
        var jsMock = new Mock<IJSRuntime>();
        jsMock.Setup(j => j.InvokeAsync<int>("aslWidgets.getUsage", It.IsAny<object?[]>()))
            .ReturnsAsync(3);
        var loggerMock = new Mock<ILogger<WidgetService>>();
        var service = new WidgetService(envMock.Object, jsMock.Object, loggerMock.Object);

        await service.IncrementUsageAsync("w1");
        jsMock.Verify(j => j.InvokeVoidAsync("aslWidgets.incrementUsage", It.Is<object[]>(o => o[0]!.Equals("w1"))), Times.Once);

        var usage = await service.GetUsageAsync("w1");
        Assert.Equal(3, usage);
    }
}

