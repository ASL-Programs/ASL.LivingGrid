namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface ISecretStorageService
{
    Task<string> EncryptAsync(string plainText);
    Task<string> DecryptAsync(string cipherText);
}
