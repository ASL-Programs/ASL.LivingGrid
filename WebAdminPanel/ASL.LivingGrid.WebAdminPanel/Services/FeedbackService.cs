using System.Text.Json;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class FeedbackService : IFeedbackService
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<FeedbackService> _logger;
    private readonly List<FeedbackItem> _items = new();

    public FeedbackService(IWebHostEnvironment env, ILogger<FeedbackService> logger)
    {
        _env = env;
        _logger = logger;
    }

    public async Task SubmitAsync(string page, int rating, string comments)
    {
        var item = new FeedbackItem
        {
            Page = page,
            Rating = rating,
            Comments = comments,
            Timestamp = DateTime.UtcNow
        };
        _items.Add(item);
        try
        {
            var file = Path.Combine(_env.ContentRootPath, "feedback.json");
            var json = JsonSerializer.Serialize(_items, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(file, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving feedback");
        }
    }

    private record FeedbackItem
    {
        public string Page { get; init; } = string.Empty;
        public int Rating { get; init; }
        public string Comments { get; init; } = string.Empty;
        public DateTime Timestamp { get; init; }
    }
}
