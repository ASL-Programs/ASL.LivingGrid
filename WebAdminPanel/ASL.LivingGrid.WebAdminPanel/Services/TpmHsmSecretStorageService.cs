using Microsoft.AspNetCore.DataProtection;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class TpmHsmSecretStorageService : ISecretStorageService
{
    private readonly IDataProtector _protector;

    public TpmHsmSecretStorageService(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("ASL.LivingGrid.SecretStorage");
    }

    public Task<string> EncryptAsync(string plainText)
    {
        var protectedData = _protector.Protect(plainText ?? string.Empty);
        return Task.FromResult(protectedData);
    }

    public Task<string> DecryptAsync(string cipherText)
    {
        var plain = string.IsNullOrEmpty(cipherText) ? string.Empty : _protector.Unprotect(cipherText);
        return Task.FromResult(plain);
    }
}
