using System.Text.Json;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class CloudFunctionService : ICloudFunctionService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<CloudFunctionService> _logger;
    private Dictionary<string, string> _functions = new();

    public CloudFunctionService(IHttpClientFactory clientFactory, IWebHostEnvironment env, ILogger<CloudFunctionService> logger)
    {
        _clientFactory = clientFactory;
        _env = env;
        _logger = logger;
        LoadConfig();
    }

    private void LoadConfig()
    {
        var file = Path.Combine(_env.ContentRootPath, "cloudFunctions.json");
        if (!File.Exists(file))
        {
            _logger.LogWarning("cloudFunctions.json not found");
            return;
        }
        try
        {
            var json = File.ReadAllText(file);
            _functions = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading cloud functions config");
        }
    }

    public async Task<string?> InvokeAsync(string name, object? payload = null, CancellationToken cancellationToken = default)
    {
        if (!_functions.TryGetValue(name, out var url))
        {
            _logger.LogWarning("Cloud function {Name} not configured", name);
            return null;
        }

        try
        {
            var client = _clientFactory.CreateClient();
            using var response = await client.PostAsync(url, new StringContent(JsonSerializer.Serialize(payload ?? new { } ), System.Text.Encoding.UTF8, "application/json"), cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invoking cloud function {Name}", name);
            return null;
        }
    }
}
