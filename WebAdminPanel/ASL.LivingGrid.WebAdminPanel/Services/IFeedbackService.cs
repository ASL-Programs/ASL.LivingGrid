namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IFeedbackService
{
    Task SubmitAsync(string page, int rating, string comments);
}
