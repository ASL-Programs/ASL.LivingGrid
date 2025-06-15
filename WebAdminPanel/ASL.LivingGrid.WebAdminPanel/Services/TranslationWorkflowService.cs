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

    public async Task<TranslationRequest> SubmitRequestAsync(string key, string culture, string proposedValue, string requestedBy, TranslationRequestStatus status = TranslationRequestStatus.Machine)
    {
        var req = new TranslationRequest
        {
            Key = key,
            Culture = culture,
            ProposedValue = proposedValue,
            RequestedBy = requestedBy,
            Status = status,
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

        if (req.Status == TranslationRequestStatus.Machine)
            req.Status = TranslationRequestStatus.MachineApproved;
        else if (req.Status == TranslationRequestStatus.Human)
            req.Status = TranslationRequestStatus.HumanApproved;
        else
            req.Status = TranslationRequestStatus.Approved;
        req.ApprovedBy = approvedBy;
        req.ApprovedAt = DateTime.UtcNow;
        req.RejectedBy = null;
        req.RejectedAt = null;
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

    public async Task ReviewRequestAsync(Guid id, bool accept, string reviewer, string? comments, bool escalate)
    {
        var req = await _context.TranslationRequests.FindAsync(id);
        if (req == null) return;

        req.ReviewerComments = comments;
        req.Escalate = escalate;

        if (accept)
        {
            if (req.Status == TranslationRequestStatus.Machine)
                req.Status = TranslationRequestStatus.MachineApproved;
            else if (req.Status == TranslationRequestStatus.Human)
                req.Status = TranslationRequestStatus.HumanApproved;
            else
                req.Status = TranslationRequestStatus.Approved;
            req.ApprovedBy = reviewer;
            req.ApprovedAt = DateTime.UtcNow;
            req.RejectedBy = null;
            req.RejectedAt = null;

            if (req.ProposedValue is not null)
            {
                await _localizationService.SetStringAsync(req.Key, req.ProposedValue, req.Culture);
            }
        }
        else
        {
            req.Status = TranslationRequestStatus.Rejected;
            req.RejectedBy = reviewer;
            req.RejectedAt = DateTime.UtcNow;
            req.ApprovedBy = null;
            req.ApprovedAt = null;
        }

        await _context.SaveChangesAsync();
        await _audit.LogAsync("Review", nameof(TranslationRequest), id.ToString(), reviewer, reviewer, null, req);
    }

    public async Task RejectRequestAsync(Guid id, string reviewer, string? comments, bool escalate)
    {
        await ReviewRequestAsync(id, accept: false, reviewer, comments, escalate);
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
            else if (string.Equals(provider, "Azure", StringComparison.OrdinalIgnoreCase))
            {
                var endpoint = _configuration.GetValue<string>("Translation:Endpoint") ?? "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0";
                var apiKey = _configuration.GetValue<string>("Translation:ApiKey");
                var region = _configuration.GetValue<string>("Translation:Region");

                var url = $"{endpoint}&from={sourceCulture}&to={targetCulture}";
                using var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("Ocp-Apim-Subscription-Key", apiKey);
                if (!string.IsNullOrEmpty(region))
                    request.Headers.Add("Ocp-Apim-Subscription-Region", region);
                request.Content = new StringContent(JsonSerializer.Serialize(new[] { new { Text = text } }), Encoding.UTF8, "application/json");

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
                var result = doc.RootElement[0].GetProperty("translations")[0].GetProperty("text").GetString();
                return result?.Trim();
            }
            else if (string.Equals(provider, "Custom", StringComparison.OrdinalIgnoreCase))
            {
                var endpoint = _configuration.GetValue<string>("Translation:Endpoint");
                if (string.IsNullOrEmpty(endpoint))
                {
                    _logger.LogWarning("Custom translation provider endpoint not configured");
                    return null;
                }

                using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
                var payload = new { text, sourceCulture, targetCulture };
                var apiKey = _configuration.GetValue<string>("Translation:ApiKey");
                if (!string.IsNullOrEmpty(apiKey))
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
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
                if (doc.RootElement.TryGetProperty("translation", out var node))
                    return node.GetString()?.Trim();
                return null;
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
            .Where(r => r.Status == TranslationRequestStatus.Machine || r.Status == TranslationRequestStatus.Human || r.Status == TranslationRequestStatus.PendingReview)
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

    public async Task<TranslationRequest?> GetRequestAsync(Guid id)
    {
        return await _context.TranslationRequests.FindAsync(id);
    }
}
