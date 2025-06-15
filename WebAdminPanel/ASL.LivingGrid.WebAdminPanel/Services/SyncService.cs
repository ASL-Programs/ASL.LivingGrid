using System.Text.Json;
using System.Text;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class SyncService : BackgroundService, ISyncService
{
    private readonly ILogger<SyncService> _logger;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IWebHostEnvironment _env;
    private List<string> _nodes = new();

    public SyncService(ILogger<SyncService> logger, IHttpClientFactory clientFactory, IWebHostEnvironment env)
    {
        _logger = logger;
        _clientFactory = clientFactory;
        _env = env;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await LoadNodesAsync();
        while (!stoppingToken.IsCancellationRequested)
        {
            await SyncOnceAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    public async Task SyncOnceAsync(CancellationToken cancellationToken = default)
    {
        foreach (var node in _nodes)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                var content = new StringContent("{}", Encoding.UTF8, "application/json");
                await client.PostAsync($"{node}/api/sync/ping", content, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing with node {Node}", node);
            }
        }
    }

    private async Task LoadNodesAsync()
    {
        var file = Path.Combine(_env.ContentRootPath, "sync_nodes.json");
        if (!File.Exists(file))
        {
            _logger.LogInformation("No sync nodes file found");
            return;
        }
        try
        {
            var json = await File.ReadAllTextAsync(file);
            _nodes = JsonSerializer.Deserialize<List<string>>(json) ?? new();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading sync nodes file");
        }
    }
}
