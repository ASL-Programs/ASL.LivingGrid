namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface ISyncService
{
    Task SyncOnceAsync(CancellationToken cancellationToken = default);
}
