using System.Net;
using System.Net.Http;
using ASL.LivingGrid.WebAdminPanel.Models;
using ASL.LivingGrid.WebAdminPanel.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ASL.LivingGrid.WebAdminPanel.Tests;
using Moq;
using Moq.Protected;
using Xunit;

namespace ASL.LivingGrid.WebAdminPanel.Tests.Services;

public class ThemeMarketplaceServiceTests
{
    [Fact]
    public async Task ListAvailableThemesAsync_ReadsFromLocalFile()
    {
        using var tempDir = new TemporaryDirectory();
        var jsonFile = Path.Combine(tempDir.Path, "theme_marketplace.json");
        var json = "[{"Id":"dark","Name":"Dark","Description":"Desc","DownloadUrl":"http://example.com/dark.css","PreviewImage":"img"}]";
        await File.WriteAllTextAsync(jsonFile, json);

        var envMock = new Mock<IWebHostEnvironment>();
        envMock.SetupGet(e => e.ContentRootPath).Returns(tempDir.Path);
        envMock.SetupGet(e => e.WebRootPath).Returns(tempDir.Path);

        var clientFactoryMock = new Mock<IHttpClientFactory>();
        var loggerMock = new Mock<ILogger<ThemeMarketplaceService>>();
        var configuration = new ConfigurationBuilder().Build();

        var service = new ThemeMarketplaceService(envMock.Object, clientFactoryMock.Object, loggerMock.Object, configuration);

        var themes = await service.ListAvailableThemesAsync();

        var theme = Assert.Single(themes);
        Assert.Equal("dark", theme.Id);
    }

    [Fact]
    public async Task ImportThemeAsync_DownloadsCssAndSavesFile()
    {
        using var tempDir = new TemporaryDirectory();
        Directory.CreateDirectory(Path.Combine(tempDir.Path, "css", "themes"));
        var jsonFile = Path.Combine(tempDir.Path, "theme_marketplace.json");
        var json = "[{\"Id\":\"dark\",\"Name\":\"Dark\",\"Description\":\"Desc\",\"DownloadUrl\":\"http://example.com/dark.css\",\"PreviewImage\":\"img\"}]";
        await File.WriteAllTextAsync(jsonFile, json);

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("body{}")
            });
        var httpClient = new HttpClient(handlerMock.Object);
        var httpFactoryMock = new Mock<IHttpClientFactory>();
        httpFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var envMock = new Mock<IWebHostEnvironment>();
        envMock.SetupGet(e => e.ContentRootPath).Returns(tempDir.Path);
        envMock.SetupGet(e => e.WebRootPath).Returns(tempDir.Path);

        var loggerMock = new Mock<ILogger<ThemeMarketplaceService>>();
        var configuration = new ConfigurationBuilder().Build();
        var service = new ThemeMarketplaceService(envMock.Object, httpFactoryMock.Object, loggerMock.Object, configuration);

        var theme = await service.ImportThemeAsync("dark");

        Assert.NotNull(theme);
        var expectedFile = Path.Combine(tempDir.Path, "css", "themes", "dark.css");
        Assert.True(File.Exists(expectedFile));
        var css = await File.ReadAllTextAsync(expectedFile);
        Assert.Equal("body{}", css);
    }
}
