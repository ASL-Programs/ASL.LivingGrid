namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IDisasterRecoveryService
{
    Task BackupAsync(CancellationToken cancellationToken = default);
    Task TriggerFailoverAsync(CancellationToken cancellationToken = default);
}
