namespace ASL.LivingGrid.WebAdminPanel.Services;

public class EnvironmentConfigManager : IEnvironmentConfigManager
{
    private readonly IConfigurationService _config;

    public EnvironmentConfigManager(IConfigurationService config)
    {
        _config = config;
    }

    public async Task<IDictionary<string, string?>> LoadAsync()
    {
        var keys = new[] { "ApiUrl", "AuthUrl" };
        var dict = new Dictionary<string, string?>();
        foreach (var k in keys)
        {
            dict[k] = await _config.GetValueAsync<string>($"Environment:{k}") ?? string.Empty;
        }
        return dict;
    }

    public async Task SaveAsync(IDictionary<string, string?> values)
    {
        foreach (var kv in values)
        {
            await _config.SetValueAsync($"Environment:{kv.Key}", kv.Value);
        }
    }
}
