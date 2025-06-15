using Microsoft.AspNetCore.Http;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class SessionPersistenceService : ISessionPersistenceService
{
    private readonly IHttpContextAccessor _accessor;

    public SessionPersistenceService(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public Task SaveAsync(string key, string data)
    {
        _accessor.HttpContext?.Session.SetString(key, data);
        return Task.CompletedTask;
    }

    public Task<string?> LoadAsync(string key)
    {
        var value = _accessor.HttpContext?.Session.GetString(key);
        return Task.FromResult<string?>(value);
    }
}
