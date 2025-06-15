namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface ICloudFunctionService
{
    Task<string?> InvokeAsync(string name, object? payload = null, CancellationToken cancellationToken = default);
}
