using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface ITranslationWorkflowService
{
    Task<TranslationRequest> SubmitRequestAsync(string key, string culture, string proposedValue, string requestedBy);
    Task ApproveRequestAsync(Guid id, string approvedBy, bool apply);
    Task<string?> SuggestAsync(string text, string sourceCulture, string targetCulture);
    Task<IEnumerable<TranslationRequest>> GetPendingRequestsAsync();
    Task<IEnumerable<TranslationRequest>> GetRequestsByStatusAsync(TranslationRequestStatus status);
    Task UpdateStatusAsync(Guid id, TranslationRequestStatus status, string updatedBy);
    Task ReviewRequestAsync(Guid id, bool accept, string reviewer, string? comments, bool escalate);
}
