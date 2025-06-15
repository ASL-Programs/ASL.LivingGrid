using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface ITranslationWorkflowService
{
    Task<TranslationRequest> SubmitRequestAsync(string key, string culture, string proposedValue, string requestedBy);
    Task ApproveRequestAsync(Guid id, string approvedBy, bool apply);
    Task<string?> SuggestAsync(string text, string sourceCulture, string targetCulture);
    Task<IEnumerable<TranslationRequest>> GetPendingRequestsAsync();
}
