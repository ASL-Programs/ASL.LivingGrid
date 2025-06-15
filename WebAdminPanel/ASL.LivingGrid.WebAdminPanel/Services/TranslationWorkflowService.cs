using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class TranslationWorkflowService : ITranslationWorkflowService
{
    private readonly ApplicationDbContext _context;
    private readonly ILocalizationService _localizationService;
    private readonly IAuditService _audit;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TranslationWorkflowService> _logger;

    public TranslationWorkflowService(ApplicationDbContext context,
        ILocalizationService localizationService,
        IAuditService audit,
        IHttpClientFactory httpClientFactory,
        ILogger<TranslationWorkflowService> logger)
    {
        _context = context;
        _localizationService = localizationService;
        _audit = audit;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<TranslationRequest> SubmitRequestAsync(string key, string culture, string proposedValue, string requestedBy)
    {
        var req = new TranslationRequest
        {
            Key = key,
            Culture = culture,
            ProposedValue = proposedValue,
            RequestedBy = requestedBy,
            CreatedAt = DateTime.UtcNow
        };
        _context.TranslationRequests.Add(req);
        await _context.SaveChangesAsync();

        await _audit.LogAsync("Create", nameof(TranslationRequest), req.Id.ToString(), requestedBy, requestedBy, null, req);
        return req;
    }

    public async Task ApproveRequestAsync(Guid id, string approvedBy, bool apply)
    {
        var req = await _context.TranslationRequests.FindAsync(id);
        if (req == null) return;

        req.Status = TranslationRequestStatus.Approved;
        req.ApprovedBy = approvedBy;
        req.ApprovedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        if (apply && req.ProposedValue != null)
        {
            await _localizationService.SetStringAsync(req.Key, req.ProposedValue, req.Culture);
        }

        await _audit.LogAsync("Approve", nameof(TranslationRequest), id.ToString(), approvedBy, approvedBy, null, req);
    }

    public async Task<IEnumerable<TranslationRequest>> GetPendingRequestsAsync()
    {
        return await _context.TranslationRequests
            .Where(r => r.Status == TranslationRequestStatus.Pending)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<string?> SuggestAsync(string text, string targetCulture, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"https://api.mymemory.translated.net/get?q={Uri.EscapeDataString(text)}&langpair=en|{Uri.EscapeDataString(targetCulture)}";
            using var response = await client.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
            var translated = doc.RootElement.GetProperty("responseData").GetProperty("translatedText").GetString();
            return translated;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Translation suggestion failed");
            return null;
        }
    }
}
