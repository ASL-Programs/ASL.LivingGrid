using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Hosting;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class DisasterRecoveryService : BackgroundService, IDisasterRecoveryService
{
    private readonly ILogger<DisasterRecoveryService> _logger;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IWebHostEnvironment _env;
    private DisasterRecoveryConfig _config = new();

    public DisasterRecoveryService(ILogger<DisasterRecoveryService> logger, IHttpClientFactory clientFactory, IWebHostEnvironment env)
    {
        _logger = logger;
        _clientFactory = clientFactory;
        _env = env;
        LoadConfig();
    }

    private void LoadConfig()
    {
        var file = Path.Combine(_env.ContentRootPath, "disaster_recovery.json");
        if (!File.Exists(file))
        {
            _logger.LogWarning("disaster_recovery.json not found");
            return;
        }
        try
        {
            var json = File.ReadAllText(file);
            _config = JsonSerializer.Deserialize<DisasterRecoveryConfig>(json) ?? new();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading disaster recovery config");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await BackupAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    public async Task BackupAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var path = Path.Combine(_env.ContentRootPath, _config.BackupPath ?? "backups");
            Directory.CreateDirectory(path);
            var file = Path.Combine(path, $"backup-{DateTime.UtcNow:yyyyMMddHHmmss}.txt");
            await File.WriteAllTextAsync(file, "backup", cancellationToken);
            _logger.LogInformation("Created backup {File}", file);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Backup failed");
        }
    }

    public async Task TriggerFailoverAsync(CancellationToken cancellationToken = default)
    {
        if (_config.FailoverNodes is null) return;
        foreach (var node in _config.FailoverNodes)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                await client.PostAsync($"{node}/api/failover", new StringContent("{}", Encoding.UTF8, "application/json"), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failover to node {Node} failed", node);
            }
        }
    }
}

public class DisasterRecoveryConfig
{
    public string? BackupPath { get; set; }
    public List<string>? FailoverNodes { get; set; }
}
