using ASL.LivingGrid.WebAdminPanel.Data;
using ASL.LivingGrid.WebAdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public class WireframePageBuilderService : IWireframePageBuilderService
{
    private readonly ILogger<WireframePageBuilderService> _logger;
    private readonly IConfigurationService _configService;
    private readonly string _projectsPath;
    private readonly string _templatesPath;
    private readonly string _previewsPath;
    private Task _initTemplatesTask = Task.CompletedTask;

    public WireframePageBuilderService(
        ILogger<WireframePageBuilderService> logger,
        IConfigurationService configService)
    {
        _logger = logger;
        _configService = configService;
        
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        _projectsPath = Path.Combine(basePath, "Wireframes", "Projects");
        _templatesPath = Path.Combine(basePath, "Wireframes", "Templates");
        _previewsPath = Path.Combine(basePath, "Wireframes", "Previews");
        
        try
        {
            // Ensure directories exist
            Directory.CreateDirectory(_projectsPath);
            Directory.CreateDirectory(_templatesPath);
            Directory.CreateDirectory(_previewsPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating wireframe directories");
            throw;
        }

        // Initialize default templates in background
        _initTemplatesTask = Task.Run(InitializeDefaultTemplatesAsync);
        _initTemplatesTask.ContinueWith(
            t => _logger.LogError(t.Exception, "Error initializing wireframe templates"),
            TaskContinuationOptions.OnlyOnFaulted);
    }

    public async Task<WireframeProject> CreateProjectAsync(string name, string description)
    {
        try
        {
            var project = new WireframeProject
            {
                Name = name,
                Description = description,
                Settings = new ProjectSettings
                {
                    Canvas = new CanvasSettings(),
                    Grid = new GridSettings(),
                    Snap = new SnapSettings(),
                    Theme = new ThemeSettings(),
                    Export = new ExportSettings()
                }
            };

            await SaveProjectAsync(project);
            
            _logger.LogInformation("Wireframe project created: {ProjectName} ({ProjectId})", name, project.Id);
            return project;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating wireframe project: {ProjectName}", name);
            throw;
        }
    }

    public async Task<WireframeProject?> GetProjectAsync(string projectId)
    {
        try
        {
            var projectPath = Path.Combine(_projectsPath, $"{projectId}.json");
            if (!File.Exists(projectPath))
            {
                return null;
            }

            var json = await File.ReadAllTextAsync(projectPath);
            return JsonSerializer.Deserialize<WireframeProject>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting wireframe project: {ProjectId}", projectId);
            return null;
        }
    }

    public async Task<IEnumerable<WireframeProject>> GetAllProjectsAsync()
    {
        try
        {
            var projects = new List<WireframeProject>();
            
            if (!Directory.Exists(_projectsPath))
            {
                return projects;
            }

            var projectFiles = Directory.GetFiles(_projectsPath, "*.json");
            
            foreach (var file in projectFiles)
            {
                try
                {
                    var json = await File.ReadAllTextAsync(file);
                    var project = JsonSerializer.Deserialize<WireframeProject>(json);
                    if (project != null)
                    {
                        projects.Add(project);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error reading project file: {FilePath}", file);
                }
            }

            return projects.OrderByDescending(p => p.LastModified);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all wireframe projects");
            return new List<WireframeProject>();
        }
    }

    public async Task<bool> UpdateProjectAsync(WireframeProject project)
    {
        try
        {
            project.LastModified = DateTime.UtcNow;
            await SaveProjectAsync(project);
            
            _logger.LogInformation("Wireframe project updated: {ProjectId}", project.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating wireframe project: {ProjectId}", project.Id);
            return false;
        }
    }

    public async Task<bool> DeleteProjectAsync(string projectId)
    {
        try
        {
            var projectPath = Path.Combine(_projectsPath, $"{projectId}.json");
            if (File.Exists(projectPath))
            {
                File.Delete(projectPath);
            }

            // Delete associated pages and previews
            var previewDir = Path.Combine(_previewsPath, projectId);
            if (Directory.Exists(previewDir))
            {
                Directory.Delete(previewDir, true);
            }

            _logger.LogInformation("Wireframe project deleted: {ProjectId}", projectId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting wireframe project: {ProjectId}", projectId);
            return false;
        }
    }

    public async Task<WireframePage> CreatePageAsync(string projectId, string pageName, PageType pageType)
    {
        try
        {
            var page = new WireframePage
            {
                ProjectId = projectId,
                Name = pageName,
                PageType = pageType,
                Settings = new PageSettings
                {
                    Responsive = new ResponsiveSettings(),
                    Layout = new LayoutSettings(),
                    Accessibility = new AccessibilitySettings(),
                    SEO = new SEOSettings()
                }
            };

            // Add default elements based on page type
            await AddDefaultElementsForPageType(page, pageType);

            // Update project with new page
            var project = await GetProjectAsync(projectId);
            if (project != null)
            {
                project.Pages.Add(page);
                await UpdateProjectAsync(project);
            }

            _logger.LogInformation("Wireframe page created: {PageName} ({PageId}) in project {ProjectId}", 
                pageName, page.Id, projectId);
            return page;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating wireframe page: {PageName}", pageName);
            throw;
        }
    }

    public async Task<WireframePage?> GetPageAsync(string pageId)
    {
        try
        {
            var projects = await GetAllProjectsAsync();
            
            foreach (var project in projects)
            {
                var page = project.Pages.FirstOrDefault(p => p.Id == pageId);
                if (page != null)
                {
                    return page;
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting wireframe page: {PageId}", pageId);
            return null;
        }
    }

    public async Task<IEnumerable<WireframePage>> GetProjectPagesAsync(string projectId)
    {
        try
        {
            var project = await GetProjectAsync(projectId);
            return project?.Pages ?? new List<WireframePage>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting project pages: {ProjectId}", projectId);
            return new List<WireframePage>();
        }
    }

    public async Task<bool> UpdatePageAsync(WireframePage page)
    {
        try
        {
            var project = await GetProjectAsync(page.ProjectId);
            if (project == null)
            {
                return false;
            }

            var existingPageIndex = project.Pages.FindIndex(p => p.Id == page.Id);
            if (existingPageIndex >= 0)
            {
                page.LastModified = DateTime.UtcNow;
                project.Pages[existingPageIndex] = page;
                await UpdateProjectAsync(project);
                
                _logger.LogInformation("Wireframe page updated: {PageId}", page.Id);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating wireframe page: {PageId}", page.Id);
            return false;
        }
    }

    public async Task<bool> DeletePageAsync(string pageId)
    {
        try
        {
            var projects = await GetAllProjectsAsync();
            
            foreach (var project in projects)
            {
                var pageIndex = project.Pages.FindIndex(p => p.Id == pageId);
                if (pageIndex >= 0)
                {
                    project.Pages.RemoveAt(pageIndex);
                    await UpdateProjectAsync(project);
                    
                    _logger.LogInformation("Wireframe page deleted: {PageId}", pageId);
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting wireframe page: {PageId}", pageId);
            return false;
        }
    }

    public async Task<WireframeElement> AddElementAsync(string pageId, WireframeElement element)
    {
        try
        {
            element.PageId = pageId;
            element.CreatedAt = DateTime.UtcNow;
            element.LastModified = DateTime.UtcNow;

            var page = await GetPageAsync(pageId);
            if (page != null)
            {
                page.Elements.Add(element);
                await UpdatePageAsync(page);
                
                _logger.LogInformation("Element added to page: {ElementId} -> {PageId}", element.Id, pageId);
            }

            return element;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding element to page: {PageId}", pageId);
            throw;
        }
    }

    public async Task<bool> UpdateElementAsync(WireframeElement element)
    {
        try
        {
            var page = await GetPageAsync(element.PageId);
            if (page == null)
            {
                return false;
            }

            var existingElementIndex = page.Elements.FindIndex(e => e.Id == element.Id);
            if (existingElementIndex >= 0)
            {
                element.LastModified = DateTime.UtcNow;
                page.Elements[existingElementIndex] = element;
                await UpdatePageAsync(page);
                
                _logger.LogInformation("Element updated: {ElementId}", element.Id);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating element: {ElementId}", element.Id);
            return false;
        }
    }

    public async Task<bool> DeleteElementAsync(string elementId)
    {
        try
        {
            var projects = await GetAllProjectsAsync();
            
            foreach (var project in projects)
            {
                foreach (var page in project.Pages)
                {
                    var elementIndex = page.Elements.FindIndex(e => e.Id == elementId);
                    if (elementIndex >= 0)
                    {
                        page.Elements.RemoveAt(elementIndex);
                        await UpdatePageAsync(page);
                        
                        _logger.LogInformation("Element deleted: {ElementId}", elementId);
                        return true;
                    }
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting element: {ElementId}", elementId);
            return false;
        }
    }

    public async Task<bool> MoveElementAsync(string elementId, int newX, int newY)
    {
        try
        {
            var projects = await GetAllProjectsAsync();
            
            foreach (var project in projects)
            {
                foreach (var page in project.Pages)
                {
                    var element = page.Elements.FirstOrDefault(e => e.Id == elementId);
                    if (element != null)
                    {
                        element.Position.X = newX;
                        element.Position.Y = newY;
                        element.LastModified = DateTime.UtcNow;
                        
                        await UpdatePageAsync(page);
                        return true;
                    }
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving element: {ElementId}", elementId);
            return false;
        }
    }

    public async Task<bool> ResizeElementAsync(string elementId, int newWidth, int newHeight)
    {
        try
        {
            var projects = await GetAllProjectsAsync();
            
            foreach (var project in projects)
            {
                foreach (var page in project.Pages)
                {
                    var element = page.Elements.FirstOrDefault(e => e.Id == elementId);
                    if (element != null)
                    {
                        element.Size.Width = newWidth;
                        element.Size.Height = newHeight;
                        element.LastModified = DateTime.UtcNow;
                        
                        await UpdatePageAsync(page);
                        return true;
                    }
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resizing element: {ElementId}", elementId);
            return false;
        }
    }

    public async Task<IEnumerable<WireframeTemplate>> GetAvailableTemplatesAsync()
    {
        try
        {
            var templates = new List<WireframeTemplate>();
            
            if (!Directory.Exists(_templatesPath))
            {
                return templates;
            }

            var templateFiles = Directory.GetFiles(_templatesPath, "*.json");
            
            foreach (var file in templateFiles)
            {
                try
                {
                    var json = await File.ReadAllTextAsync(file);
                    var template = JsonSerializer.Deserialize<WireframeTemplate>(json);
                    if (template != null)
                    {
                        templates.Add(template);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error reading template file: {FilePath}", file);
                }
            }

            return templates.OrderBy(t => t.Category).ThenBy(t => t.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available templates");
            return new List<WireframeTemplate>();
        }
    }

    public async Task<WireframePage> CreatePageFromTemplateAsync(string projectId, string templateId, string pageName)
    {
        try
        {
            var templates = await GetAvailableTemplatesAsync();
            var template = templates.FirstOrDefault(t => t.Id == templateId);
            
            if (template == null)
            {
                throw new InvalidOperationException($"Template not found: {templateId}");
            }

            // Clone the template data
            var templateJson = JsonSerializer.Serialize(template.TemplateData);
            var page = JsonSerializer.Deserialize<WireframePage>(templateJson);
            
            if (page != null)
            {
                page.Id = Guid.NewGuid().ToString();
                page.ProjectId = projectId;
                page.Name = pageName;
                page.CreatedAt = DateTime.UtcNow;
                page.LastModified = DateTime.UtcNow;
                page.IsTemplate = false;

                // Generate new IDs for all elements
                RegenerateElementIds(page.Elements);

                // Update project with new page
                var project = await GetProjectAsync(projectId);
                if (project != null)
                {
                    project.Pages.Add(page);
                    await UpdateProjectAsync(project);
                }

                // Update template usage count
                template.UsageCount++;
                await SaveTemplateAsync(template);

                _logger.LogInformation("Page created from template: {PageName} from template {TemplateId}", 
                    pageName, templateId);
                return page;
            }

            throw new InvalidOperationException("Failed to deserialize template data");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating page from template: {TemplateId}", templateId);
            throw;
        }
    }

    public async Task<bool> SavePageAsTemplateAsync(string pageId, string templateName, string description)
    {
        try
        {
            var page = await GetPageAsync(pageId);
            if (page == null)
            {
                return false;
            }

            var template = new WireframeTemplate
            {
                Name = templateName,
                Description = description,
                Category = page.PageType.ToString(),
                PageType = page.PageType,
                TemplateData = page
            };

            await SaveTemplateAsync(template);
            
            _logger.LogInformation("Page saved as template: {PageId} -> {TemplateName}", pageId, templateName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving page as template: {PageId}", pageId);
            return false;
        }
    }

    public async Task<string> GenerateCodeAsync(string pageId, CodeGenerationType codeType)
    {
        try
        {
            var page = await GetPageAsync(pageId);
            if (page == null)
            {
                return string.Empty;
            }

            return codeType switch
            {
                CodeGenerationType.HTML => await GenerateHTMLAsync(page),
                CodeGenerationType.CSS => await GenerateCSSAsync(page),
                CodeGenerationType.JavaScript => await GenerateJavaScriptAsync(page),
                CodeGenerationType.Blazor => await GenerateBlazorAsync(page),
                CodeGenerationType.React => await GenerateReactAsync(page),
                CodeGenerationType.Bootstrap => await GenerateBootstrapAsync(page),
                CodeGenerationType.TailwindCSS => await GenerateTailwindAsync(page),
                _ => await GenerateHTMLAsync(page)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating code for page: {PageId}", pageId);
            return string.Empty;
        }
    }

    public async Task<string> ExportProjectAsync(string projectId, ExportFormat format)
    {
        try
        {
            var project = await GetProjectAsync(projectId);
            if (project == null)
            {
                return string.Empty;
            }

            var exportDir = Path.Combine(_previewsPath, "exports", projectId);
            Directory.CreateDirectory(exportDir);

            return format switch
            {
                ExportFormat.JSON => await ExportAsJSONAsync(project, exportDir),
                ExportFormat.XML => await ExportAsXMLAsync(project, exportDir),
                ExportFormat.PDF => await ExportAsPDFAsync(project, exportDir),
                ExportFormat.ZIP => await ExportAsZIPAsync(project, exportDir),
                _ => await ExportAsJSONAsync(project, exportDir)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting project: {ProjectId}", projectId);
            return string.Empty;
        }
    }

    public async Task<WireframeProject?> ImportProjectAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            var json = await File.ReadAllTextAsync(filePath);
            var project = JsonSerializer.Deserialize<WireframeProject>(json);
            
            if (project != null)
            {
                // Generate new ID to avoid conflicts
                project.Id = Guid.NewGuid().ToString();
                project.CreatedAt = DateTime.UtcNow;
                project.LastModified = DateTime.UtcNow;

                // Regenerate IDs for all pages and elements
                foreach (var page in project.Pages)
                {
                    page.Id = Guid.NewGuid().ToString();
                    page.ProjectId = project.Id;
                    RegenerateElementIds(page.Elements);
                }

                await SaveProjectAsync(project);
                
                _logger.LogInformation("Project imported: {ProjectName} ({ProjectId})", project.Name, project.Id);
                return project;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing project from: {FilePath}", filePath);
            return null;
        }
    }

    public async Task<bool> ValidatePageAsync(string pageId)
    {
        try
        {
            var issues = await GetPageValidationIssuesAsync(pageId);
            return !issues.Any(i => i.Severity == ValidationSeverity.Error || i.Severity == ValidationSeverity.Critical);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating page: {PageId}", pageId);
            return false;
        }
    }

    public async Task<IEnumerable<ValidationIssue>> GetPageValidationIssuesAsync(string pageId)
    {
        try
        {
            var page = await GetPageAsync(pageId);
            if (page == null)
            {
                return new List<ValidationIssue>
                {
                    new()
                    {
                        Severity = ValidationSeverity.Critical,
                        Message = "Page not found",
                        Description = $"Page with ID {pageId} could not be found"
                    }
                };
            }

            var issues = new List<ValidationIssue>();

            // Validate page structure
            if (string.IsNullOrWhiteSpace(page.Name))
            {
                issues.Add(new ValidationIssue
                {
                    Severity = ValidationSeverity.Error,
                    Message = "Page name is required",
                    Description = "Every page must have a descriptive name"
                });
            }

            // Validate elements
            foreach (var element in page.Elements)
            {
                await ValidateElementAsync(element, issues);
            }

            // Check for accessibility issues
            await ValidateAccessibilityAsync(page, issues);

            return issues;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting page validation issues: {PageId}", pageId);
            return new List<ValidationIssue>();
        }
    }

    public async Task<WireframePreview> GeneratePreviewAsync(string pageId, PreviewMode mode)
    {
        try
        {
            var preview = new WireframePreview
            {
                PageId = pageId,
                Mode = mode
            };

            var startTime = DateTime.UtcNow;

            // Generate HTML, CSS, and JS
            preview.GeneratedHTML = await GenerateCodeAsync(pageId, CodeGenerationType.HTML);
            preview.GeneratedCSS = await GenerateCodeAsync(pageId, CodeGenerationType.CSS);
            preview.GeneratedJS = await GenerateCodeAsync(pageId, CodeGenerationType.JavaScript);

            // Save preview files
            var previewDir = Path.Combine(_previewsPath, pageId);
            Directory.CreateDirectory(previewDir);

            var htmlPath = Path.Combine(previewDir, "preview.html");
            await File.WriteAllTextAsync(htmlPath, GenerateFullHTMLPreview(preview));

            preview.PreviewUrl = $"/wireframes/preview/{pageId}/preview.html";
            preview.GenerationTime = DateTime.UtcNow - startTime;

            _logger.LogInformation("Preview generated for page: {PageId}", pageId);
            return preview;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating preview for page: {PageId}", pageId);
            throw;
        }
    }

    public async Task<bool> PublishPageAsync(string pageId, PublishOptions options)
    {
        try
        {
            var page = await GetPageAsync(pageId);
            if (page == null)
            {
                return false;
            }

            // Generate preview
            var preview = await GeneratePreviewAsync(pageId, PreviewMode.Interactive);

            // Create published version based on options
            var publishDir = Path.Combine(_previewsPath, "published", pageId);
            Directory.CreateDirectory(publishDir);

            // Save published files
            var htmlContent = GenerateFullHTMLPreview(preview);
            if (options.PasswordProtected && !string.IsNullOrEmpty(options.Password))
            {
                htmlContent = AddPasswordProtection(htmlContent, options.Password);
            }

            await File.WriteAllTextAsync(Path.Combine(publishDir, "index.html"), htmlContent);

            // Update page status
            page.Status = PageStatus.Published;
            await UpdatePageAsync(page);

            _logger.LogInformation("Page published: {PageId}", pageId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing page: {PageId}", pageId);
            return false;
        }
    }

    // Private helper methods
    private async Task SaveProjectAsync(WireframeProject project)
    {
        var projectPath = Path.Combine(_projectsPath, $"{project.Id}.json");
        var json = JsonSerializer.Serialize(project, new JsonSerializerOptions 
        { 
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });
        await File.WriteAllTextAsync(projectPath, json);
    }

    private async Task SaveTemplateAsync(WireframeTemplate template)
    {
        var templatePath = Path.Combine(_templatesPath, $"{template.Id}.json");
        var json = JsonSerializer.Serialize(template, new JsonSerializerOptions 
        { 
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });
        await File.WriteAllTextAsync(templatePath, json);
    }

    private async Task InitializeDefaultTemplatesAsync()
    {
        try
        {
            var existingTemplates = await GetAvailableTemplatesAsync();
            if (existingTemplates.Any())
            {
                return; // Templates already exist
            }

            // Create default templates
            await CreateDefaultLandingPageTemplate();
            await CreateDefaultDashboardTemplate();
            await CreateDefaultFormTemplate();
            await CreateDefaultLoginTemplate();

            _logger.LogInformation("Default wireframe templates initialized");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing default templates");
        }
    }

    private async Task CreateDefaultLandingPageTemplate()
    {
        var template = new WireframeTemplate
        {
            Name = "Landing Page",
            Description = "A simple landing page template with header, hero section, and footer",
            Category = "Marketing",
            PageType = PageType.Landing,
            TemplateData = new WireframePage
            {
                Name = "Landing Page Template",
                PageType = PageType.Landing,
                Elements = new List<WireframeElement>
                {
                    new()
                    {
                        Type = ElementType.Header,
                        Name = "Main Header",
                        Position = new ElementPosition { X = 0, Y = 0 },
                        Size = new ElementSize { Width = 1200, Height = 80 },
                        Style = new ElementStyle { BackgroundColor = "#f8f9fa" }
                    },
                    new()
                    {
                        Type = ElementType.Section,
                        Name = "Hero Section",
                        Position = new ElementPosition { X = 0, Y = 80 },
                        Size = new ElementSize { Width = 1200, Height = 400 },
                        Style = new ElementStyle { BackgroundColor = "#007bff" }
                    },
                    new()
                    {
                        Type = ElementType.Footer,
                        Name = "Main Footer",
                        Position = new ElementPosition { X = 0, Y = 480 },
                        Size = new ElementSize { Width = 1200, Height = 60 },
                        Style = new ElementStyle { BackgroundColor = "#343a40" }
                    }
                }
            }
        };

        await SaveTemplateAsync(template);
    }

    private async Task CreateDefaultDashboardTemplate()
    {
        var template = new WireframeTemplate
        {
            Name = "Dashboard",
            Description = "A dashboard template with sidebar navigation and content area",
            Category = "Admin",
            PageType = PageType.Dashboard,
            TemplateData = new WireframePage
            {
                Name = "Dashboard Template",
                PageType = PageType.Dashboard,
                Elements = new List<WireframeElement>
                {
                    new()
                    {
                        Type = ElementType.Sidebar,
                        Name = "Navigation Sidebar",
                        Position = new ElementPosition { X = 0, Y = 0 },
                        Size = new ElementSize { Width = 250, Height = 800 },
                        Style = new ElementStyle { BackgroundColor = "#343a40" }
                    },
                    new()
                    {
                        Type = ElementType.Container,
                        Name = "Main Content",
                        Position = new ElementPosition { X = 250, Y = 0 },
                        Size = new ElementSize { Width = 950, Height = 800 },
                        Style = new ElementStyle { BackgroundColor = "#ffffff" }
                    }
                }
            }
        };

        await SaveTemplateAsync(template);
    }

    private async Task CreateDefaultFormTemplate()
    {
        var template = new WireframeTemplate
        {
            Name = "Contact Form",
            Description = "A contact form template with common form fields",
            Category = "Forms",
            PageType = PageType.Form,
            TemplateData = new WireframePage
            {
                Name = "Contact Form Template",
                PageType = PageType.Form,
                Elements = new List<WireframeElement>
                {
                    new()
                    {
                        Type = ElementType.Form,
                        Name = "Contact Form",
                        Position = new ElementPosition { X = 100, Y = 100 },
                        Size = new ElementSize { Width = 400, Height = 500 },
                        Style = new ElementStyle { BorderWidth = 1, BorderColor = "#dee2e6" }
                    }
                }
            }
        };

        await SaveTemplateAsync(template);
    }

    private async Task CreateDefaultLoginTemplate()
    {
        var template = new WireframeTemplate
        {
            Name = "Login Page",
            Description = "A simple login page template",
            Category = "Authentication",
            PageType = PageType.Login,
            TemplateData = new WireframePage
            {
                Name = "Login Page Template",
                PageType = PageType.Login,
                Elements = new List<WireframeElement>
                {
                    new()
                    {
                        Type = ElementType.Container,
                        Name = "Login Container",
                        Position = new ElementPosition { X = 400, Y = 200 },
                        Size = new ElementSize { Width = 400, Height = 300 },
                        Style = new ElementStyle { BorderWidth = 1, BorderColor = "#dee2e6" }
                    }
                }
            }
        };

        await SaveTemplateAsync(template);
    }

    private async Task AddDefaultElementsForPageType(WireframePage page, PageType pageType)
    {
        switch (pageType)
        {
            case PageType.Dashboard:
                page.Elements.Add(new WireframeElement
                {
                    PageId = page.Id,
                    Type = ElementType.Sidebar,
                    Name = "Navigation",
                    Position = new ElementPosition { X = 0, Y = 0 },
                    Size = new ElementSize { Width = 250, Height = 600 }
                });
                break;

            case PageType.Form:
                page.Elements.Add(new WireframeElement
                {
                    PageId = page.Id,
                    Type = ElementType.Form,
                    Name = "Main Form",
                    Position = new ElementPosition { X = 50, Y = 50 },
                    Size = new ElementSize { Width = 400, Height = 300 }
                });
                break;

            case PageType.Login:
                page.Elements.Add(new WireframeElement
                {
                    PageId = page.Id,
                    Type = ElementType.Container,
                    Name = "Login Container",
                    Position = new ElementPosition { X = 300, Y = 150 },
                    Size = new ElementSize { Width = 350, Height = 250 }
                });
                break;
        }

        await Task.Delay(1); // Make it async
    }

    private void RegenerateElementIds(List<WireframeElement> elements)
    {
        foreach (var element in elements)
        {
            element.Id = Guid.NewGuid().ToString();
            RegenerateElementIds(element.Children);
        }
    }

    private async Task<string> GenerateHTMLAsync(WireframePage page)
    {
        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html lang=\"en\">");
        html.AppendLine("<head>");
        html.AppendLine($"    <title>{page.Name}</title>");
        html.AppendLine("    <meta charset=\"UTF-8\">");
        html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        html.AppendLine("</head>");
        html.AppendLine("<body>");

        foreach (var element in page.Elements.OrderBy(e => e.ZIndex))
        {
            html.AppendLine(GenerateElementHTML(element));
        }

        html.AppendLine("</body>");
        html.AppendLine("</html>");

        await Task.Delay(1); // Make it async
        return html.ToString();
    }

    private string GenerateElementHTML(WireframeElement element)
    {
        var tagName = GetHTMLTagName(element.Type);
        var style = GenerateElementCSSStyle(element);
        var attributes = GenerateElementAttributes(element);

        var html = new StringBuilder();
        html.AppendLine($"    <{tagName} id=\"{element.Id}\" style=\"{style}\" {attributes}>");
        
        if (!string.IsNullOrEmpty(element.Properties.Text))
        {
            html.AppendLine($"        {element.Properties.Text}");
        }

        foreach (var child in element.Children)
        {
            html.AppendLine(GenerateElementHTML(child));
        }

        html.AppendLine($"    </{tagName}>");

        return html.ToString();
    }

    private string GetHTMLTagName(ElementType type)
    {
        return type switch
        {
            ElementType.Header => "header",
            ElementType.Footer => "footer",
            ElementType.Section => "section",
            ElementType.Container => "div",
            ElementType.Text => "span",
            ElementType.Heading => "h2",
            ElementType.Paragraph => "p",
            ElementType.Image => "img",
            ElementType.Button => "button",
            ElementType.Input => "input",
            ElementType.Textarea => "textarea",
            ElementType.Select => "select",
            ElementType.Form => "form",
            ElementType.Link => "a",
            _ => "div"
        };
    }

    private string GenerateElementCSSStyle(WireframeElement element)
    {
        var styles = new List<string>();

        styles.Add($"position: {element.Position.PositionType.ToString().ToLower()}");
        styles.Add($"left: {element.Position.X}px");
        styles.Add($"top: {element.Position.Y}px");
        styles.Add($"width: {element.Size.Width}px");
        styles.Add($"height: {element.Size.Height}px");
        
        if (element.Style.BackgroundColor != "transparent")
        {
            styles.Add($"background-color: {element.Style.BackgroundColor}");
        }
        
        if (element.Style.BorderWidth > 0)
        {
            styles.Add($"border: {element.Style.BorderWidth}px {element.Style.BorderStyle} {element.Style.BorderColor}");
        }

        return string.Join("; ", styles);
    }

    private string GenerateElementAttributes(WireframeElement element)
    {
        var attributes = new List<string>();

        foreach (var attr in element.Properties.Attributes)
        {
            attributes.Add($"{attr.Key}=\"{attr.Value}\"");
        }

        return string.Join(" ", attributes);
    }

    private async Task<string> GenerateCSSAsync(WireframePage page)
    {
        var css = new StringBuilder();
        css.AppendLine("/* Generated CSS for wireframe */");
        css.AppendLine("body { margin: 0; padding: 0; font-family: Arial, sans-serif; }");

        foreach (var element in page.Elements)
        {
            css.AppendLine(GenerateElementCSS(element));
        }

        await Task.Delay(1); // Make it async
        return css.ToString();
    }

    private string GenerateElementCSS(WireframeElement element)
    {
        var css = new StringBuilder();
        css.AppendLine($"#{element.Id} {{");
        css.AppendLine($"    position: {element.Position.PositionType.ToString().ToLower()};");
        css.AppendLine($"    left: {element.Position.X}px;");
        css.AppendLine($"    top: {element.Position.Y}px;");
        css.AppendLine($"    width: {element.Size.Width}px;");
        css.AppendLine($"    height: {element.Size.Height}px;");
        
        if (element.Style.BackgroundColor != "transparent")
        {
            css.AppendLine($"    background-color: {element.Style.BackgroundColor};");
        }
        
        css.AppendLine("}");

        return css.ToString();
    }

    private async Task<string> GenerateJavaScriptAsync(WireframePage page)
    {
        var js = new StringBuilder();
        js.AppendLine("// Generated JavaScript for wireframe");
        js.AppendLine("document.addEventListener('DOMContentLoaded', function() {");
        js.AppendLine("    console.log('Wireframe loaded');");
        js.AppendLine("});");

        await Task.Delay(1); // Make it async
        return js.ToString();
    }

    private async Task<string> GenerateBlazorAsync(WireframePage page)
    {
        var blazor = new StringBuilder();
        blazor.AppendLine("@page \"/generated-page\"");
        blazor.AppendLine($"<PageTitle>{page.Name}</PageTitle>");
        blazor.AppendLine();

        foreach (var element in page.Elements)
        {
            blazor.AppendLine(GenerateBlazorElement(element));
        }

        await Task.Delay(1); // Make it async
        return blazor.ToString();
    }

    private string GenerateBlazorElement(WireframeElement element)
    {
        return element.Type switch
        {
            ElementType.Container => $"<div class=\"container\">{element.Properties.Text}</div>",
            ElementType.Button => $"<button class=\"btn btn-primary\">{element.Properties.Text}</button>",
            ElementType.Input => $"<input class=\"form-control\" placeholder=\"{element.Properties.Placeholder}\" />",
            _ => $"<div>{element.Properties.Text}</div>"
        };
    }

    private async Task<string> GenerateReactAsync(WireframePage page)
    {
        var react = new StringBuilder();
        react.AppendLine("import React from 'react';");
        react.AppendLine();
        react.AppendLine($"const {page.Name.Replace(" ", "")}Page = () => {{");
        react.AppendLine("    return (");
        react.AppendLine("        <div>");

        foreach (var element in page.Elements)
        {
            react.AppendLine($"            {GenerateReactElement(element)}");
        }

        react.AppendLine("        </div>");
        react.AppendLine("    );");
        react.AppendLine("};");
        react.AppendLine();
        react.AppendLine($"export default {page.Name.Replace(" ", "")}Page;");

        await Task.Delay(1); // Make it async
        return react.ToString();
    }

    private string GenerateReactElement(WireframeElement element)
    {
        return element.Type switch
        {
            ElementType.Container => $"<div>{element.Properties.Text}</div>",
            ElementType.Button => $"<button>{element.Properties.Text}</button>",
            ElementType.Input => $"<input placeholder=\"{element.Properties.Placeholder}\" />",
            _ => $"<div>{element.Properties.Text}</div>"
        };
    }

    private async Task<string> GenerateBootstrapAsync(WireframePage page)
    {
        var html = await GenerateHTMLAsync(page);
        
        // Add Bootstrap classes
        html = html.Replace("<div", "<div class=\"container\"");
        html = html.Replace("<button", "<button class=\"btn btn-primary\"");
        html = html.Replace("<input", "<input class=\"form-control\"");

        return html;
    }

    private async Task<string> GenerateTailwindAsync(WireframePage page)
    {
        var html = await GenerateHTMLAsync(page);
        
        // Add Tailwind classes
        html = html.Replace("<div", "<div class=\"container mx-auto\"");
        html = html.Replace("<button", "<button class=\"bg-blue-500 text-white px-4 py-2 rounded\"");
        html = html.Replace("<input", "<input class=\"border border-gray-300 px-3 py-2 rounded\"");

        return html;
    }

    private async Task<string> ExportAsJSONAsync(WireframeProject project, string exportDir)
    {
        var filePath = Path.Combine(exportDir, $"{project.Name}_export.json");
        var json = JsonSerializer.Serialize(project, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
        return filePath;
    }

    private async Task<string> ExportAsXMLAsync(WireframeProject project, string exportDir)
    {
        // Simplified XML export
        var filePath = Path.Combine(exportDir, $"{project.Name}_export.xml");
        var xml = $"<?xml version=\"1.0\"?><project name=\"{project.Name}\" id=\"{project.Id}\"></project>";
        await File.WriteAllTextAsync(filePath, xml);
        return filePath;
    }

    private async Task<string> ExportAsPDFAsync(WireframeProject project, string exportDir)
    {
        // This would require a PDF generation library
        // For now, just create a placeholder
        var filePath = Path.Combine(exportDir, $"{project.Name}_export.pdf");
        await File.WriteAllTextAsync(filePath, "PDF export placeholder");
        return filePath;
    }

    private async Task<string> ExportAsZIPAsync(WireframeProject project, string exportDir)
    {
        // Create a ZIP file with all project files
        var filePath = Path.Combine(exportDir, $"{project.Name}_export.zip");
        
        // For now, just create a placeholder
        await File.WriteAllTextAsync(filePath, "ZIP export placeholder");
        return filePath;
    }

    private async Task ValidateElementAsync(WireframeElement element, List<ValidationIssue> issues)
    {
        // Validate element size
        if (element.Size.Width <= 0 || element.Size.Height <= 0)
        {
            issues.Add(new ValidationIssue
            {
                ElementId = element.Id,
                Severity = ValidationSeverity.Warning,
                Message = "Element has invalid size",
                Description = $"Element {element.Name} has zero or negative dimensions"
            });
        }

        // Validate required text for text elements
        if ((element.Type == ElementType.Text || element.Type == ElementType.Heading || element.Type == ElementType.Paragraph) 
            && string.IsNullOrWhiteSpace(element.Properties.Text))
        {
            issues.Add(new ValidationIssue
            {
                ElementId = element.Id,
                Severity = ValidationSeverity.Warning,
                Message = "Text element is empty",
                Description = $"Text element {element.Name} should have content"
            });
        }

        await Task.Delay(1); // Make it async
    }

    private async Task ValidateAccessibilityAsync(WireframePage page, List<ValidationIssue> issues)
    {
        // Check for images without alt text
        var images = page.Elements.Where(e => e.Type == ElementType.Image);
        foreach (var image in images)
        {
            if (string.IsNullOrWhiteSpace(image.Properties.Alt))
            {
                issues.Add(new ValidationIssue
                {
                    ElementId = image.Id,
                    Severity = ValidationSeverity.Warning,
                    Message = "Image missing alt text",
                    Description = "Images should have alternative text for accessibility"
                });
            }
        }

        await Task.Delay(1); // Make it async
    }

    private string GenerateFullHTMLPreview(WireframePreview preview)
    {
        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html lang=\"en\">");
        html.AppendLine("<head>");
        html.AppendLine("    <meta charset=\"UTF-8\">");
        html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        html.AppendLine("    <title>Wireframe Preview</title>");
        html.AppendLine("    <style>");
        html.AppendLine(preview.GeneratedCSS);
        html.AppendLine("    </style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");
        html.AppendLine(preview.GeneratedHTML);
        html.AppendLine("    <script>");
        html.AppendLine(preview.GeneratedJS);
        html.AppendLine("    </script>");
        html.AppendLine("</body>");
        html.AppendLine("</html>");

        return html.ToString();
    }

    private string AddPasswordProtection(string htmlContent, string password)
    {
        // Simple password protection (in real implementation, use proper authentication)
        var protection = $@"
<script>
var enteredPassword = prompt('Enter password to view this page:');
if (enteredPassword !== '{password}') {{
    document.body.innerHTML = '<h1>Access Denied</h1>';
}}
</script>";

        return htmlContent.Replace("<body>", $"<body>{protection}");
    }
}
