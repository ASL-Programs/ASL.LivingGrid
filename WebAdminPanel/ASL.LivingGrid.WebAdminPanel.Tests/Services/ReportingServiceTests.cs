using System.Security.Claims;
using System.Text;
using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Models;
using ASL.LivingGrid.WebAdminPanel.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ASL.LivingGrid.WebAdminPanel.Tests.Services;

public class ReportingServiceTests
{
    private static ApplicationDbContext GetContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;
        var context = new ApplicationDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task RunReportAsync_ReturnsResults()
    {
        using var ctx = GetContext();
        ctx.Companies.Add(new Company { Name = "Acme", Code = "A" });
        ctx.SaveChanges();

        var report = new Report
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Query = "SELECT Name FROM Companies WHERE Name LIKE '%' || @keyword || '%'"
        };
        ctx.Reports.Add(report);
        ctx.SaveChanges();

        var auditLogger = new Mock<ILogger<AuditService>>();
        var audit = new AuditService(ctx, auditLogger.Object);
        var logger = new Mock<ILogger<ReportingService>>();
        var service = new ReportingService(ctx, audit, logger.Object);

        var filter = new ReportFilter { Keyword = "Acme" };
        var user = new ClaimsPrincipal(new ClaimsIdentity());

        var results = (await service.RunReportAsync(report.Id, filter, user)).ToList();

        Assert.Single(results);
        Assert.Equal("Acme", results[0]["Name"]);
    }

    [Fact]
    public async Task ExportReportAsync_GeneratesCsv()
    {
        using var ctx = GetContext();
        ctx.Companies.Add(new Company { Name = "Acme", Code = "A" });
        ctx.SaveChanges();

        var report = new Report
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Query = "SELECT Name FROM Companies"
        };
        ctx.Reports.Add(report);
        ctx.SaveChanges();

        var auditLogger = new Mock<ILogger<AuditService>>();
        var audit = new AuditService(ctx, auditLogger.Object);
        var logger = new Mock<ILogger<ReportingService>>();
        var service = new ReportingService(ctx, audit, logger.Object);

        var bytes = await service.ExportReportAsync(report.Id, new ReportFilter(), "csv", new ClaimsPrincipal());
        var csv = Encoding.UTF8.GetString(bytes);

        Assert.Contains("Name", csv);
        Assert.Contains("Acme", csv);
    }
}
