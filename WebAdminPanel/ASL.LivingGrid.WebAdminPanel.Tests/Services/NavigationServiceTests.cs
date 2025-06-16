using System.Security.Claims;
using ASL.LivingGrid.WebAdminPanel.Models;
using ASL.LivingGrid.WebAdminPanel.Services;
using Microsoft.Extensions.Logging;
using ASL.LivingGrid.WebAdminPanel.Tests;
using Moq;
using Xunit;

namespace ASL.LivingGrid.WebAdminPanel.Tests.Services;

public class NavigationServiceTests
{
    [Fact]
    public async Task GetMenuItemsAsync_FiltersByRoleService()
    {
        using var tempDir = new TemporaryDirectory();
        var dir = tempDir.Path;
        var json = "[{\"Key\":\"Dashboard\",\"Url\":\"/\",\"Icon\":\"home\"},{\"Key\":\"Admin\",\"Url\":\"/admin\",\"Icon\":\"admin\"}]";
        await File.WriteAllTextAsync(Path.Combine(dir, "menuitems.json"), json);

        var envMock = new Mock<IWebHostEnvironment>();
        envMock.SetupGet(e => e.ContentRootPath).Returns(dir);

        var roleMock = new Mock<IRoleBasedUiService>();
        roleMock.Setup(r => r.HasAccessAsync("Dashboard", It.IsAny<ClaimsPrincipal>())).ReturnsAsync(true);
        roleMock.Setup(r => r.HasAccessAsync("Admin", It.IsAny<ClaimsPrincipal>())).ReturnsAsync(false);

        var loggerMock = new Mock<ILogger<NavigationService>>();
        var service = new NavigationService(loggerMock.Object, envMock.Object, roleMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity());
        var items = (await service.GetMenuItemsAsync(user)).ToList();

        Assert.Single(items);
        Assert.Equal("Dashboard", items[0].Key);
    }
}
