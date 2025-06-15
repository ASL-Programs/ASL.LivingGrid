using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class TranslationWorkflowService : ITranslationWorkflowService
{
    private readonly ApplicationDbContext _context;
    private readonly ILocalizationService _localizationService;
    private readonly IAuditService _audit;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TranslationWorkflowService> _logger;

    public TranslationWorkflowService(
        ApplicationDbContext context,
        ILocalizationService localizationService,
        IAuditService audit,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<TranslationWorkflowService> logger)
    {
        _context = context;
        _localizationService = localizationService;
        _audit = audit;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
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
            Status = TranslationRequestStatus.PendingReview,
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

    public async Task UpdateStatusAsync(Guid id, TranslationRequestStatus status, string updatedBy)
    {
        var req = await _context.TranslationRequests.FindAsync(id);
        if (req == null) return;

        req.Status = status;
        await _context.SaveChangesAsync();

        await _audit.LogAsync("StatusUpdate", nameof(TranslationRequest), id.ToString(), updatedBy, updatedBy, null, req);
    }

    public async Task<string?> SuggestAsync(string text, string sourceCulture, string targetCulture)
    {
        var provider = _configuration.GetValue<string>("Translation:Provider") ?? "OpenAI";
        try
        {
            var client = _httpClientFactory.CreateClient();
            if (string.Equals(provider, "OpenAI", StringComparison.OrdinalIgnoreCase))
            {
                var endpoint = _configuration.GetValue<string>("Translation:Endpoint") ?? "https://api.openai.com/v1/chat/completions";
                var apiKey = _configuration.GetValue<string>("Translation:ApiKey");
                var model = _configuration.GetValue<string>("Translation:Model") ?? "gpt-3.5-turbo";

                using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                var payload = new
                {
                    model,
                    messages = new[]
                    {
                        new { role = "system", content = $"Translate the following text from {sourceCulture} to {targetCulture}." },
                        new { role = "user", content = text }
                    }
                };
                request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                using var response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == (System.Net.HttpStatusCode)429)
                        _logger.LogWarning("Translation provider rate limited");
                    else
                        _logger.LogError("Translation provider error: {StatusCode}", response.StatusCode);
                    return null;
                }
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var result = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                return result?.Trim();
            }
            else if (string.Equals(provider, "DeepL", StringComparison.OrdinalIgnoreCase))
            {
                var endpoint = _configuration.GetValue<string>("Translation:Endpoint") ?? "https://api.deepl.com/v2/translate";
                var apiKey = _configuration.GetValue<string>("Translation:ApiKey");
                var query = new Dictionary<string, string>
                {
                    ["auth_key"] = apiKey ?? string.Empty,
                    ["text"] = text,
                    ["source_lang"] = sourceCulture.ToUpperInvariant(),
                    ["target_lang"] = targetCulture.ToUpperInvariant()
                };
                using var content = new FormUrlEncodedContent(query);
                using var response = await client.PostAsync(endpoint, content);
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == (System.Net.HttpStatusCode)429)
                        _logger.LogWarning("Translation provider rate limited");
                    else
                        _logger.LogError("Translation provider error: {StatusCode}", response.StatusCode);
                    return null;
                }
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var result = doc.RootElement.GetProperty("translations")[0].GetProperty("text").GetString();
                return result?.Trim();
            }
            else if (string.Equals(provider, "Google", StringComparison.OrdinalIgnoreCase))
            {
                var endpoint = _configuration.GetValue<string>("Translation:Endpoint") ?? "https://translation.googleapis.com/language/translate/v2";
                var apiKey = _configuration.GetValue<string>("Translation:ApiKey");
                var query = new Dictionary<string, string>
                {
                    ["q"] = text,
                    ["source"] = sourceCulture,
                    ["target"] = targetCulture,
                    ["format"] = "text",
                    ["key"] = apiKey ?? string.Empty
                };
                using var content = new FormUrlEncodedContent(query);
                using var response = await client.PostAsync(endpoint, content);
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == (System.Net.HttpStatusCode)429)
                        _logger.LogWarning("Translation provider rate limited");
                    else
                        _logger.LogError("Translation provider error: {StatusCode}", response.StatusCode);
                    return null;
                }
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var result = doc.RootElement.GetProperty("data").GetProperty("translations")[0].GetProperty("translatedText").GetString();
                return WebUtility.HtmlDecode(result ?? string.Empty).Trim();
            }

            _logger.LogWarning("Unknown translation provider: {Provider}", provider);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requesting translation from {Provider}", provider);
            return null;
        }
    }

    public async Task<IEnumerable<TranslationRequest>> GetPendingRequestsAsync()
    {
        return await _context.TranslationRequests
            .Where(r => r.Status == TranslationRequestStatus.PendingReview)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TranslationRequest>> GetRequestsByStatusAsync(TranslationRequestStatus status)
    {
        return await _context.TranslationRequests
            .Where(r => r.Status == status)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();
    }
}
