using System.Net;
using System.Net.Http;
using ASL.LivingGrid.WebAdminPanel.Models;
using ASL.LivingGrid.WebAdminPanel.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace ASL.LivingGrid.WebAdminPanel.Tests.Services;

public class WidgetMarketplaceServiceTests
{
    [Fact]
    public async Task ListAsync_ReadsFromLocalFile()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var jsonFile = Path.Combine(tempDir, "widget_marketplace.json");
        var json = "[{\"Id\":\"w1\",\"Name\":\"Widget\",\"Description\":\"Desc\",\"DownloadUrl\":\"http://example.com/widget.json\",\"PreviewImage\":\"img\"}]";
        await File.WriteAllTextAsync(jsonFile, json);

        var envMock = new Mock<IWebHostEnvironment>();
        envMock.SetupGet(e => e.ContentRootPath).Returns(tempDir);
        envMock.SetupGet(e => e.WebRootPath).Returns(tempDir);
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
                Content = new StringContent("[{\\"Id\\":\\"w1\\",\\"Name\\":\\"Widget\\",\\"Description\\":\\"Desc\\",\\"DownloadUrl\\":\\"http://example.com/widget.json\\",\\"PreviewImage\\":\\"img\\"}]")
            });
        var httpClient = new HttpClient(handlerMock.Object);
        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.SetupGet(e => e.ContentRootPath).Returns(tempDir);
        envMock.SetupGet(e => e.WebRootPath).Returns(tempDir);
        var loggerMock = new Mock<ILogger<WidgetMarketplaceService>>();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["WidgetMarketplace:Source"] = "http://example.com/marketplace.json"
        }).Build();
        var service = new WidgetMarketplaceService(envMock.Object, factoryMock.Object, loggerMock.Object, configuration);

        var widgets = await service.ListAsync();
        var widget = Assert.Single(widgets);
        Assert.Equal("w1", widget.Id);
    }

    [Fact]
    public async Task ImportAsync_DownloadsAndSavesFile()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(tempDir, "www", "widgets"));
        var jsonFile = Path.Combine(tempDir, "widget_marketplace.json");
        var marketplaceJson = "[{\"Id\":\"w1\",\"Name\":\"Widget\",\"Description\":\"Desc\",\"DownloadUrl\":\"http://example.com/widget.json\",\"PreviewImage\":\"img\"}]";
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
        envMock.SetupGet(e => e.ContentRootPath).Returns(tempDir);
        envMock.SetupGet(e => e.WebRootPath).Returns(Path.Combine(tempDir, "www"));
        var loggerMock = new Mock<ILogger<WidgetMarketplaceService>>();
        var configuration = new ConfigurationBuilder().Build();
        var service = new WidgetMarketplaceService(envMock.Object, factoryMock.Object, loggerMock.Object, configuration);

        var result = await service.ImportAsync("w1");
        Assert.NotNull(result);
        var expectedFile = Path.Combine(tempDir, "www", "widgets", "w1.json");
        Assert.True(File.Exists(expectedFile));
    }

    [Fact]
    public async Task ExportAsync_ReadsFile()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var widgetsDir = Path.Combine(tempDir, "widgets");
        Directory.CreateDirectory(widgetsDir);
        var file = Path.Combine(widgetsDir, "w1.json");
        await File.WriteAllTextAsync(file, "content");

        var envMock = new Mock<IWebHostEnvironment>();
        envMock.SetupGet(e => e.WebRootPath).Returns(tempDir);
        envMock.SetupGet(e => e.ContentRootPath).Returns(tempDir);
        var factoryMock = new Mock<IHttpClientFactory>();
        var loggerMock = new Mock<ILogger<WidgetMarketplaceService>>();
        var configuration = new ConfigurationBuilder().Build();
        var service = new WidgetMarketplaceService(envMock.Object, factoryMock.Object, loggerMock.Object, configuration);

        var result = await service.ExportAsync("w1");
        Assert.Equal("content", result);
    }
}

