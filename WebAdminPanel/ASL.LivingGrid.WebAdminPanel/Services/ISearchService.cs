namespace ASL.LivingGrid.WebAdminPanel.Services;

using ASL.LivingGrid.WebAdminPanel.Models;

public interface ISearchService
{
    Task<IDictionary<string, List<SearchResultItem>>> SearchAsync(string query);
}
