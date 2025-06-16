using System.Text.Json;
using Microsoft.Extensions.Hosting;
using ASL.LivingGrid.WebAdminPanel.Models;
using Microsoft.Extensions.Configuration;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class LocalizationUpdateService : BackgroundService
{
    private readonly ILogger<LocalizationUpdateService> _logger;
    private readonly IServiceProvider _provider;
    private readonly IWebHostEnvironment _env;
    private readonly TimeSpan _interval;
    private readonly IConfiguration _configuration;

    public LocalizationUpdateService(ILogger<LocalizationUpdateService> logger, IServiceProvider provider, IWebHostEnvironment env, IConfiguration configuration)
    {
        _logger = logger;
        _provider = provider;
        _env = env;
        _configuration = configuration;
        var minutes = _configuration.GetValue<int>("Localization:UpdateIntervalMinutes", 30);
        _interval = TimeSpan.FromMinutes(minutes);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessUpdatesAsync();
            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task ProcessUpdatesAsync()
    {
        try
        {
            var file = Path.Combine(_env.ContentRootPath, "pending_languagepack_updates.json");
            if (!File.Exists(file)) return;
            var json = await File.ReadAllTextAsync(file);
            var updates = JsonSerializer.Deserialize<List<PendingLanguagePackUpdate>>(json) ?? new();
            if (updates.Count == 0) return;

            using var scope = _provider.CreateScope();
            var locSvc = scope.ServiceProvider.GetRequiredService<ILocalizationService>();
            var notify = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var audit = scope.ServiceProvider.GetRequiredService<IAuditService>();
            var clientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();

            foreach (var upd in updates.Where(u => !u.Applied))
            {
                var client = clientFactory.CreateClient();
                var packJson = await client.GetStringAsync(upd.DownloadUrl);
                await locSvc.ImportAsync(packJson, upd.Culture);
                await audit.LogAsync("Update", "LanguagePack", upd.Culture);
                await notify.CreateAsync("Language pack updated", $"{upd.Culture} language pack applied", userId: null);
                upd.Applied = true;
            }

            await File.WriteAllTextAsync(file, JsonSerializer.Serialize(updates));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing localization updates");
        }
    }
}

public class PendingLanguagePackUpdate
{
    public string Culture { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;
    public bool Applied { get; set; }
}
