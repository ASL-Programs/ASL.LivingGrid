using System.Security.Claims;
using ASL.LivingGrid.WebAdminPanel.Models;
using ASL.LivingGrid.WebAdminPanel.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using ASL.LivingGrid.WebAdminPanel.Tests;
using Moq;
using Xunit;

namespace ASL.LivingGrid.WebAdminPanel.Tests.Services;

public class WidgetPermissionServiceTests
{
    private static WidgetPermissionService CreateService(string json, TemporaryDirectory tempDir)
    {
        File.WriteAllText(Path.Combine(tempDir.Path, "widget_permissions.json"), json);
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.SetupGet(e => e.ContentRootPath).Returns(tempDir.Path);
        var loggerMock = new Mock<ILogger<WidgetPermissionService>>();
        var service = new WidgetPermissionService(envMock.Object, loggerMock.Object);
        return service;
    }

    private static ClaimsPrincipal CreateUser(string id)
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, id) });
        return new ClaimsPrincipal(identity);
    }

    [Fact]
    public void HasAccess_DeniesByModule()
    {
        var json = "{\"w1\":{\"Modules\":[\"m1\"]}}";
        using var tempDir = new TemporaryDirectory();
        var service = CreateService(json, tempDir);
        var user = CreateUser("u1");
        var result = service.HasAccess("w1", user, module: "m2");
        Assert.False(result);
    }

    [Fact]
    public void HasAccess_DeniesByTenant()
    {
        var json = "{\"w1\":{\"Tenants\":[\"t1\"]}}";
        using var tempDir = new TemporaryDirectory();
        var service = CreateService(json, tempDir);
        var user = CreateUser("u1");
        var result = service.HasAccess("w1", user, tenantId: "t2");
        Assert.False(result);
    }

    [Fact]
    public void HasAccess_DeniesByUser()
    {
        var json = "{\"w1\":{\"Users\":[\"u1\"]}}";
        using var tempDir = new TemporaryDirectory();
        var service = CreateService(json, tempDir);
        var user = CreateUser("u2");
        var result = service.HasAccess("w1", user);
        Assert.False(result);
    }

    [Fact]
    public void HasAccess_AllowsWhenNoEntry()
    {
        using var tempDir = new TemporaryDirectory();
        var service = CreateService("{}", tempDir);
        var user = CreateUser("u1");
        var result = service.HasAccess("w1", user);
        Assert.True(result);
    }

    [Fact]
    public void HasAccess_AllowsWhenMatches()
    {
        var json = "{\"w1\":{\"Modules\":[\"m1\"],\"Tenants\":[\"t1\"],\"Users\":[\"u1\"]}}";
        using var tempDir = new TemporaryDirectory();
        var service = CreateService(json, tempDir);
        var user = CreateUser("u1");
        var result = service.HasAccess("w1", user, "t1", "m1");
        Assert.True(result);
    }
}

