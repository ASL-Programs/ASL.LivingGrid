using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Models;
using Microsoft.EntityFrameworkCore;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class SearchService : ISearchService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SearchService> _logger;

    public SearchService(ApplicationDbContext context, ILogger<SearchService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IDictionary<string, List<SearchResultItem>>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new Dictionary<string, List<SearchResultItem>>();

        try
        {
            query = query.Trim();

            var configResults = await _context.Configurations
                .Where(c => EF.Functions.Like(c.Key, $"%{query}%"))
                .Select(c => new SearchResultItem(c.Id.ToString(), c.Key, "Configurations"))
                .ToListAsync();

            var userResults = await _context.AppUsers
                .Where(u => EF.Functions.Like(u.FirstName + " " + u.LastName, $"%{query}%") || EF.Functions.Like(u.Email, $"%{query}%"))
                .Select(u => new SearchResultItem(u.Id.ToString(), u.FirstName + " " + u.LastName, "Users"))
                .ToListAsync();

            var moduleResults = await _context.Plugins
                .Where(p => EF.Functions.Like(p.Name, $"%{query}%"))
                .Select(p => new SearchResultItem(p.Id.ToString(), p.Name, "Modules"))
                .ToListAsync();

            var logResults = await _context.AuditLogs
                .Where(a => EF.Functions.Like(a.Action, $"%{query}%") || EF.Functions.Like(a.UserName ?? string.Empty, $"%{query}%"))
                .OrderByDescending(a => a.Timestamp)
                .Take(20)
                .Select(a => new SearchResultItem(a.Id.ToString(), a.Action + " - " + (a.UserName ?? string.Empty), "Logs"))
                .ToListAsync();

            return new Dictionary<string, List<SearchResultItem>>
            {
                { "Configurations", configResults },
                { "Users", userResults },
                { "Modules", moduleResults },
                { "Logs", logResults }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during search for query: {Query}", query);
            return new Dictionary<string, List<SearchResultItem>>();
        }
    }
}
