using System.Net;
using System.Net.Http;
using ASL.LivingGrid.WebAdminPanel.Models;
using ASL.LivingGrid.WebAdminPanel.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ASL.LivingGrid.WebAdminPanel.Tests;
using Moq;
using Moq.Protected;
using Xunit;

namespace ASL.LivingGrid.WebAdminPanel.Tests.Services;

public class WidgetMarketplaceServiceTests
{
    [Fact]
    public async Task ListAsync_ReadsFromLocalFile()
    {
        using var tempDir = new TemporaryDirectory();
        var jsonFile = Path.Combine(tempDir.Path, "widget_marketplace.json");
        var json = "[{\"Id\":\"w1\",\"Name\":\"Widget\",\"Description\":\"Desc\",\"DownloadUrl\":\"../../Docs/samples/widgets/counter.json\",\"PreviewImage\":\"img\"}]";
        await File.WriteAllTextAsync(jsonFile, json);

        var envMock = new Mock<IWebHostEnvironment>();
        envMock.SetupGet(e => e.ContentRootPath).Returns(tempDir.Path);
        envMock.SetupGet(e => e.WebRootPath).Returns(tempDir.Path);
        var factoryMock = new Mock<IHttpClientFactory>();
        var loggerMock = new Mock<ILogger<WidgetMarketplaceService>>();
        var configuration = new ConfigurationBuilder().Build();
        var service = new WidgetMarketplaceService(envMock.Object, factoryMock.Object, loggerMock.Object, configuration);

        var widgets = await service.ListAsync();
        var widget = Assert.Single(widgets);
        Assert.Equal("w1", widget.Id);
    }

    [Fact]
    public async Task ListAsync_LoadsFromUrl()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[{\\"Id\\":\\"w1\\",\\"Name\\":\\"Widget\\",\\"Description\\":\\"Desc\\",\\"DownloadUrl\\":\\"../../Docs/samples/widgets/counter.json\\",\\"PreviewImage\\":\\"img\\"}]")
            });
        var httpClient = new HttpClient(handlerMock.Object);
        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        using var tempDir = new TemporaryDirectory();
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.SetupGet(e => e.ContentRootPath).Returns(tempDir.Path);
        envMock.SetupGet(e => e.WebRootPath).Returns(tempDir.Path);
        var loggerMock = new Mock<ILogger<WidgetMarketplaceService>>();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["WidgetMarketplace:Source"] = "../../Docs/samples/widgets/marketplace.json"
        }).Build();
        var service = new WidgetMarketplaceService(envMock.Object, factoryMock.Object, loggerMock.Object, configuration);

        var widgets = await service.ListAsync();
        var widget = Assert.Single(widgets);
        Assert.Equal("w1", widget.Id);
    }

    [Fact]
    public async Task ImportAsync_DownloadsAndSavesFile()
    {
        using var tempDir = new TemporaryDirectory();
        Directory.CreateDirectory(Path.Combine(tempDir.Path, "www", "widgets"));
        var jsonFile = Path.Combine(tempDir.Path, "widget_marketplace.json");
        var marketplaceJson = "[{\"Id\":\"w1\",\"Name\":\"Widget\",\"Description\":\"Desc\",\"DownloadUrl\":\"../../Docs/samples/widgets/counter.json\",\"PreviewImage\":\"img\"}]";
        await File.WriteAllTextAsync(jsonFile, marketplaceJson);

        var widgetJson = "{\"Id\":\"w1\"}";
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(widgetJson)
            });
        var httpClient = new HttpClient(handlerMock.Object);
        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var envMock = new Mock<IWebHostEnvironment>();
        envMock.SetupGet(e => e.ContentRootPath).Returns(tempDir.Path);
        envMock.SetupGet(e => e.WebRootPath).Returns(Path.Combine(tempDir.Path, "www"));
        var loggerMock = new Mock<ILogger<WidgetMarketplaceService>>();
        var configuration = new ConfigurationBuilder().Build();
        var service = new WidgetMarketplaceService(envMock.Object, factoryMock.Object, loggerMock.Object, configuration);

        var result = await service.ImportAsync("w1");
        Assert.NotNull(result);
        var expectedFile = Path.Combine(tempDir.Path, "www", "widgets", "w1.json");
        Assert.True(File.Exists(expectedFile));
    }

    [Fact]
    public async Task ExportAsync_ReadsFile()
    {
        using var tempDir = new TemporaryDirectory();
        var widgetsDir = Path.Combine(tempDir.Path, "widgets");
        Directory.CreateDirectory(widgetsDir);
        var file = Path.Combine(widgetsDir, "w1.json");
        await File.WriteAllTextAsync(file, "content");

        var envMock = new Mock<IWebHostEnvironment>();
        envMock.SetupGet(e => e.WebRootPath).Returns(tempDir.Path);
        envMock.SetupGet(e => e.ContentRootPath).Returns(tempDir.Path);
        var factoryMock = new Mock<IHttpClientFactory>();
        var loggerMock = new Mock<ILogger<WidgetMarketplaceService>>();
        var configuration = new ConfigurationBuilder().Build();
        var service = new WidgetMarketplaceService(envMock.Object, factoryMock.Object, loggerMock.Object, configuration);

        var result = await service.ExportAsync("w1");
        Assert.Equal("content", result);
    }
}

