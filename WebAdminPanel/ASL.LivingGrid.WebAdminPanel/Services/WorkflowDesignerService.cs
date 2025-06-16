using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class WorkflowDesignerService : IWorkflowDesignerService
{
    private readonly ILogger<WorkflowDesignerService> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly string _templatesDir;
    private readonly ConcurrentDictionary<string, Workflow> _workflows = new();
    public WorkflowDesignerService(ILogger<WorkflowDesignerService> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
        _templatesDir = Path.Combine(_env.ContentRootPath, "workflow_templates");
        Directory.CreateDirectory(_templatesDir);
    }

    public Task<Workflow> CreateWorkflowAsync(string name, string description)
    {
        var wf = new Workflow { Name = name, Description = description };
        _workflows[wf.Id] = wf;
        return Task.FromResult(wf);
    }

    public Task<FormField> AddFormFieldAsync(string workflowId, FormField field)
    {
        if (_workflows.TryGetValue(workflowId, out var wf))
        {
            wf.Fields.Add(field);
            return Task.FromResult(field);
        }
        throw new KeyNotFoundException($"Workflow {workflowId} not found");
    }

    public Task<ApprovalStep> AddApprovalStepAsync(string workflowId, ApprovalStep step)
    {
        if (_workflows.TryGetValue(workflowId, out var wf))
        {
            wf.Approval.Add(step);
            return Task.FromResult(step);
        }
        throw new KeyNotFoundException($"Workflow {workflowId} not found");
    }

    public Task AddValidationRuleAsync(string workflowId, string fieldName, ValidationRule rule)
    {
        if (_workflows.TryGetValue(workflowId, out var wf))
        {
            var field = wf.Fields.FirstOrDefault(f => f.Name == fieldName);
            if (field == null)
                throw new ArgumentException($"Field {fieldName} not found");
            field.Rules.Add(rule);
            return Task.CompletedTask;
        }
        throw new KeyNotFoundException($"Workflow {workflowId} not found");
    }

    public Task<IEnumerable<string>> ValidateAsync(string workflowId, Dictionary<string, object> formData)
    {
        if (!_workflows.TryGetValue(workflowId, out var wf))
            throw new KeyNotFoundException($"Workflow {workflowId} not found");

        var errors = new List<string>();
        foreach (var field in wf.Fields)
        {
            if (field.Required && (!formData.ContainsKey(field.Name) || formData[field.Name] == null))
            {
                errors.Add($"{field.Label} tələb olunur");
                continue;
            }
            if (formData.TryGetValue(field.Name, out var value))
            {
                var str = value?.ToString() ?? string.Empty;
                foreach (var rule in field.Rules)
                {
                    if (!Regex.IsMatch(str, rule.Expression))
                        errors.Add(rule.ErrorMessage);
                }
            }
        }
        return Task.FromResult<IEnumerable<string>>(errors);
    }

    public Task TriggerScriptAsync(string workflowId, WorkflowEvent @event, ScriptLanguage language, string code)
    {
        if (!_workflows.TryGetValue(workflowId, out var wf))
            throw new KeyNotFoundException($"Workflow {workflowId} not found");

        wf.Scripts[@event] = new Script { Language = language, Code = code };
        _logger.LogInformation("Script registered for {Workflow} event {Event} in {Language}", wf.Name, @event, language);
        return Task.CompletedTask;
    }

    public Task<Workflow?> GetWorkflowAsync(string workflowId)
    {
        _workflows.TryGetValue(workflowId, out var wf);
        return Task.FromResult(wf);
    }

    public Task<string> ExportWorkflowAsync(string workflowId)
    {
        if (_workflows.TryGetValue(workflowId, out var wf))
        {
            return Task.FromResult(JsonSerializer.Serialize(wf));
        }
        return Task.FromResult(string.Empty);
    }

    public Task<Workflow?> ImportWorkflowAsync(string json)
    {
        var wf = JsonSerializer.Deserialize<Workflow>(json);
        if (wf != null)
        {
            _workflows[wf.Id] = wf;
        }
        return Task.FromResult(wf);
    }

    public async Task<string> ShareWorkflowAsync(string workflowId)
    {
        var json = await ExportWorkflowAsync(workflowId);
        if (string.IsNullOrEmpty(json))
            return string.Empty;
        var file = Path.Combine(_templatesDir, $"{workflowId}.json");
        await File.WriteAllTextAsync(file, json);
        return file;
    }

    public Task EnableAutomationAsync(string workflowId, bool enabled)
    {
        if (_workflows.TryGetValue(workflowId, out var wf))
        {
            wf.AutomationEnabled = enabled;
            _logger.LogInformation("Automation {State} for workflow {Id}", enabled ? "enabled" : "disabled", workflowId);
            return Task.CompletedTask;
        }
        throw new KeyNotFoundException($"Workflow {workflowId} not found");
    }
}
