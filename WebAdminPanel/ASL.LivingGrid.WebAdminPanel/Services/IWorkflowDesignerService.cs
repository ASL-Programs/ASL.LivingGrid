using System.Collections.Concurrent;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IWorkflowDesignerService
{
    Task<Workflow> CreateWorkflowAsync(string name, string description);
    Task<FormField> AddFormFieldAsync(string workflowId, FormField field);
    Task<ApprovalStep> AddApprovalStepAsync(string workflowId, ApprovalStep step);
    Task AddValidationRuleAsync(string workflowId, string fieldName, ValidationRule rule);
    Task<IEnumerable<string>> ValidateAsync(string workflowId, Dictionary<string, object> formData);
    Task TriggerScriptAsync(string workflowId, WorkflowEvent @event, ScriptLanguage language, string code);
    Task<Workflow?> GetWorkflowAsync(string workflowId);
    Task<string> ExportWorkflowAsync(string workflowId);
    Task<Workflow?> ImportWorkflowAsync(string json);
    Task<string> ShareWorkflowAsync(string workflowId);
    Task EnableAutomationAsync(string workflowId, bool enabled);
}

public class Workflow
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<FormField> Fields { get; set; } = new();
    public List<ApprovalStep> Approval { get; set; } = new();
    public Dictionary<WorkflowEvent, Script> Scripts { get; set; } = new();
    public bool AutomationEnabled { get; set; } = true;
}

public class FormField
{
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = "text";
    public bool Required { get; set; }
    public List<ValidationRule> Rules { get; set; } = new();
}

public class ValidationRule
{
    public string Expression { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = "Invalid";
}

public class ApprovalStep
{
    public int Order { get; set; }
    public string Role { get; set; } = string.Empty;
    public string? ApproverId { get; set; }
}

public class Script
{
    public ScriptLanguage Language { get; set; }
    public string Code { get; set; } = string.Empty;
}

public enum ScriptLanguage
{
    CSharp,
    Python,
    JavaScript
}

public enum WorkflowEvent
{
    OnSubmit,
    OnChange
}
