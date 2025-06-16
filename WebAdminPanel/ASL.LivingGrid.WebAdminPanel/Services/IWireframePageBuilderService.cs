using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IWireframePageBuilderService
{
    Task<WireframeProject> CreateProjectAsync(string name, string description);
    Task<WireframeProject?> GetProjectAsync(string projectId);
    Task<IEnumerable<WireframeProject>> GetAllProjectsAsync();
    Task<bool> UpdateProjectAsync(WireframeProject project);
    Task<bool> DeleteProjectAsync(string projectId);
    
    Task<WireframePage> CreatePageAsync(string projectId, string pageName, PageType pageType);
    Task<WireframePage?> GetPageAsync(string pageId);
    Task<IEnumerable<WireframePage>> GetProjectPagesAsync(string projectId);
    Task<bool> UpdatePageAsync(WireframePage page);
    Task<bool> DeletePageAsync(string pageId);
    
    Task<WireframeElement> AddElementAsync(string pageId, WireframeElement element);
    Task<bool> UpdateElementAsync(WireframeElement element);
    Task<bool> DeleteElementAsync(string elementId);
    Task<bool> MoveElementAsync(string elementId, int newX, int newY);
    Task<bool> ResizeElementAsync(string elementId, int newWidth, int newHeight);
    
    Task<IEnumerable<WireframeTemplate>> GetAvailableTemplatesAsync();
    Task<WireframePage> CreatePageFromTemplateAsync(string projectId, string templateId, string pageName);
    Task<bool> SavePageAsTemplateAsync(string pageId, string templateName, string description);
    
    Task<string> GenerateCodeAsync(string pageId, CodeGenerationType codeType);
    Task<string> ExportProjectAsync(string projectId, ExportFormat format);
    Task<WireframeProject?> ImportProjectAsync(string filePath);
    
    Task<bool> ValidatePageAsync(string pageId);
    Task<IEnumerable<ValidationIssue>> GetPageValidationIssuesAsync(string pageId);
    
    Task<WireframePreview> GeneratePreviewAsync(string pageId, PreviewMode mode);
    Task<bool> PublishPageAsync(string pageId, PublishOptions options);
    Task<string> GetPreviewTokenAsync(string pageId, string? password = null);
}

public class WireframeProject
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = Environment.UserName;
    public ProjectStatus Status { get; set; } = ProjectStatus.Draft;
    public ProjectSettings Settings { get; set; } = new();
    public List<WireframePage> Pages { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
    public string Version { get; set; } = "1.0.0";
    public List<string> Tags { get; set; } = new();
    public bool IsPublic { get; set; } = false;
}

public enum ProjectStatus
{
    Draft,
    InProgress,
    Review,
    Approved,
    Published,
    Archived
}

public class ProjectSettings
{
    public CanvasSettings Canvas { get; set; } = new();
    public GridSettings Grid { get; set; } = new();
    public SnapSettings Snap { get; set; } = new();
    public ThemeSettings Theme { get; set; } = new();
    public ExportSettings Export { get; set; } = new();
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

public class CanvasSettings
{
    public int Width { get; set; } = 1200;
    public int Height { get; set; } = 800;
    public string BackgroundColor { get; set; } = "#ffffff";
    public string BackgroundImage { get; set; } = string.Empty;
    public double ZoomLevel { get; set; } = 1.0;
    public bool ShowRulers { get; set; } = true;
    public bool ShowGuides { get; set; } = true;
}

public class GridSettings
{
    public bool ShowGrid { get; set; } = true;
    public int GridSize { get; set; } = 10;
    public string GridColor { get; set; } = "#e0e0e0";
    public GridType GridType { get; set; } = GridType.Square;
    public bool SnapToGrid { get; set; } = true;
}

public enum GridType
{
    Square,
    Dots,
    Lines
}

public class SnapSettings
{
    public bool SnapToGrid { get; set; } = true;
    public bool SnapToElements { get; set; } = true;
    public bool SnapToGuides { get; set; } = true;
    public int SnapTolerance { get; set; } = 5;
}

public class ThemeSettings
{
    public string PrimaryColor { get; set; } = "#007bff";
    public string SecondaryColor { get; set; } = "#6c757d";
    public string FontFamily { get; set; } = "Arial, sans-serif";
    public int BaseFontSize { get; set; } = 14;
    public Dictionary<string, string> ColorPalette { get; set; } = new();
}

public class ExportSettings
{
    public bool IncludeComments { get; set; } = true;
    public bool IncludeAnnotations { get; set; } = true;
    public bool GenerateCSS { get; set; } = true;
    public bool GenerateHTML { get; set; } = true;
    public CodeFramework TargetFramework { get; set; } = CodeFramework.Bootstrap;
}

public enum CodeFramework
{
    Bootstrap,
    Tailwind,
    MaterialUI,
    Blazor,
    React,
    Vue,
    Angular
}

public class WireframePage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProjectId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public PageType PageType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
    public PageSettings Settings { get; set; } = new();
    public List<WireframeElement> Elements { get; set; } = new();
    public List<PageAnnotation> Annotations { get; set; } = new();
    public Dictionary<string, object> PageData { get; set; } = new();
    public int SortOrder { get; set; }
    public bool IsTemplate { get; set; } = false;
    public string? ParentPageId { get; set; }
    public PageStatus Status { get; set; } = PageStatus.Draft;
}

public enum PageType
{
    Landing,
    Dashboard,
    Form,
    List,
    Detail,
    Login,
    Settings,
    Report,
    Modal,
    Sidebar,
    Navigation,
    Footer,
    Custom
}

public enum PageStatus
{
    Draft,
    InReview,
    Approved,
    Published
}

public class PageSettings
{
    public ResponsiveSettings Responsive { get; set; } = new();
    public LayoutSettings Layout { get; set; } = new();
    public AccessibilitySettings Accessibility { get; set; } = new();
    public SEOSettings SEO { get; set; } = new();
}

public class ResponsiveSettings
{
    public List<Breakpoint> Breakpoints { get; set; } = new()
    {
        new() { Name = "Mobile", Width = 576, IsActive = true },
        new() { Name = "Tablet", Width = 768, IsActive = true },
        new() { Name = "Desktop", Width = 992, IsActive = true },
        new() { Name = "Large", Width = 1200, IsActive = true }
    };
    public string DefaultBreakpoint { get; set; } = "Desktop";
}

public class Breakpoint
{
    public string Name { get; set; } = string.Empty;
    public int Width { get; set; }
    public bool IsActive { get; set; } = true;
}

public class LayoutSettings
{
    public LayoutType LayoutType { get; set; } = LayoutType.Grid;
    public int Columns { get; set; } = 12;
    public int Padding { get; set; } = 15;
    public int Margin { get; set; } = 15;
    public string ContainerMaxWidth { get; set; } = "1200px";
}

public enum LayoutType
{
    Grid,
    Flexbox,
    Float,
    Absolute,
    CSS_Grid
}

public class AccessibilitySettings
{
    public bool EnableScreenReader { get; set; } = true;
    public bool EnableKeyboardNavigation { get; set; } = true;
    public bool HighContrast { get; set; } = false;
    public bool ShowFocusIndicators { get; set; } = true;
    public string AriaLabels { get; set; } = string.Empty;
}

public class SEOSettings
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
    public string OpenGraphImage { get; set; } = string.Empty;
}

public class WireframeElement
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PageId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ElementType Type { get; set; }
    public ElementPosition Position { get; set; } = new();
    public ElementSize Size { get; set; } = new();
    public ElementStyle Style { get; set; } = new();
    public ElementProperties Properties { get; set; } = new();
    public List<WireframeElement> Children { get; set; } = new();
    public string? ParentId { get; set; }
    public int ZIndex { get; set; } = 1;
    public bool IsLocked { get; set; } = false;
    public bool IsVisible { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> CustomData { get; set; } = new();
}

public enum ElementType
{
    // Layout Elements
    Container,
    Row,
    Column,
    Section,
    Header,
    Footer,
    Sidebar,
    
    // Content Elements
    Text,
    Heading,
    Paragraph,
    Image,
    Video,
    Icon,
    
    // Form Elements
    Input,
    Textarea,
    Select,
    Checkbox,
    Radio,
    Button,
    Form,
    
    // Navigation Elements
    Navbar,
    Menu,
    Breadcrumb,
    Pagination,
    Tabs,
    
    // Display Elements
    Table,
    List,
    Card,
    Modal,
    Alert,
    Badge,
    Progress,
    
    // Interactive Elements
    Link,
    Dropdown,
    Accordion,
    Carousel,
    Tooltip,
    
    // Custom Elements
    Widget,
    Component,
    Custom
}

public class ElementPosition
{
    public int X { get; set; }
    public int Y { get; set; }
    public PositionType PositionType { get; set; } = PositionType.Relative;
    public string? AnchorTo { get; set; }
}

public enum PositionType
{
    Static,
    Relative,
    Absolute,
    Fixed,
    Sticky
}

public class ElementSize
{
    public int Width { get; set; } = 100;
    public int Height { get; set; } = 50;
    public SizeUnit WidthUnit { get; set; } = SizeUnit.Pixels;
    public SizeUnit HeightUnit { get; set; } = SizeUnit.Pixels;
    public int MinWidth { get; set; } = 0;
    public int MinHeight { get; set; } = 0;
    public int MaxWidth { get; set; } = 0;
    public int MaxHeight { get; set; } = 0;
}

public enum SizeUnit
{
    Pixels,
    Percentage,
    Em,
    Rem,
    ViewportWidth,
    ViewportHeight,
    Auto
}

public class ElementStyle
{
    public string BackgroundColor { get; set; } = "transparent";
    public string BorderColor { get; set; } = "transparent";
    public int BorderWidth { get; set; } = 0;
    public string BorderStyle { get; set; } = "solid";
    public int BorderRadius { get; set; } = 0;
    public string TextColor { get; set; } = "#000000";
    public string FontFamily { get; set; } = "Arial, sans-serif";
    public int FontSize { get; set; } = 14;
    public FontWeight FontWeight { get; set; } = FontWeight.Normal;
    public TextAlign TextAlign { get; set; } = TextAlign.Left;
    public Padding Padding { get; set; } = new();
    public Margin Margin { get; set; } = new();
    public Shadow Shadow { get; set; } = new();
    public double Opacity { get; set; } = 1.0;
    public string Transform { get; set; } = string.Empty;
    public Dictionary<string, object> CustomStyles { get; set; } = new();
}

public enum FontWeight
{
    Thin,
    Light,
    Normal,
    Medium,
    SemiBold,
    Bold,
    ExtraBold,
    Black
}

public enum TextAlign
{
    Left,
    Center,
    Right,
    Justify
}

public class Padding
{
    public int Top { get; set; } = 0;
    public int Right { get; set; } = 0;
    public int Bottom { get; set; } = 0;
    public int Left { get; set; } = 0;
}

public class Margin
{
    public int Top { get; set; } = 0;
    public int Right { get; set; } = 0;
    public int Bottom { get; set; } = 0;
    public int Left { get; set; } = 0;
}

public class Shadow
{
    public bool Enabled { get; set; } = false;
    public int OffsetX { get; set; } = 0;
    public int OffsetY { get; set; } = 2;
    public int BlurRadius { get; set; } = 4;
    public string Color { get; set; } = "rgba(0,0,0,0.1)";
}

public class ElementProperties
{
    public string Text { get; set; } = string.Empty;
    public string Placeholder { get; set; } = string.Empty;
    public string Src { get; set; } = string.Empty;
    public string Alt { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public string Target { get; set; } = "_self";
    public bool Required { get; set; } = false;
    public bool Disabled { get; set; } = false;
    public bool ReadOnly { get; set; } = false;
    public string Value { get; set; } = string.Empty;
    public List<SelectOption> Options { get; set; } = new();
    public Dictionary<string, object> Attributes { get; set; } = new();
    public List<ElementEvent> Events { get; set; } = new();
    public ValidationRules Validation { get; set; } = new();
}

public class SelectOption
{
    public string Text { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool Selected { get; set; } = false;
    public bool Disabled { get; set; } = false;
}

public class ElementEvent
{
    public string EventType { get; set; } = string.Empty; // click, hover, focus, etc.
    public string Action { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
}

public class ValidationRules
{
    public bool Required { get; set; } = false;
    public int MinLength { get; set; } = 0;
    public int MaxLength { get; set; } = 0;
    public string Pattern { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}

public class PageAnnotation
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PageId { get; set; } = string.Empty;
    public AnnotationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public AnnotationPosition Position { get; set; } = new();
    public string Color { get; set; } = "#ff6b6b";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = Environment.UserName;
    public bool IsResolved { get; set; } = false;
    public List<AnnotationComment> Comments { get; set; } = new();
}

public enum AnnotationType
{
    Note,
    Question,
    Issue,
    Improvement,
    Requirement,
    Change
}

public class AnnotationPosition
{
    public int X { get; set; }
    public int Y { get; set; }
    public string? AttachedToElementId { get; set; }
}

public class AnnotationComment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = Environment.UserName;
}

public class WireframeTemplate
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public PageType PageType { get; set; }
    public string ThumbnailUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = Environment.UserName;
    public bool IsPublic { get; set; } = true;
    public int UsageCount { get; set; } = 0;
    public double Rating { get; set; } = 0.0;
    public List<string> Tags { get; set; } = new();
    public WireframePage TemplateData { get; set; } = new();
}

public enum CodeGenerationType
{
    HTML,
    CSS,
    JavaScript,
    Blazor,
    React,
    Vue,
    Angular,
    Bootstrap,
    TailwindCSS
}

public enum ExportFormat
{
    JSON,
    XML,
    PDF,
    PNG,
    SVG,
    ZIP
}

public class ValidationIssue
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ElementId { get; set; } = string.Empty;
    public ValidationSeverity Severity { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SuggestedFix { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
}

public enum ValidationSeverity
{
    Info,
    Warning,
    Error,
    Critical
}

public class WireframePreview
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PageId { get; set; } = string.Empty;
    public PreviewMode Mode { get; set; }
    public string GeneratedHTML { get; set; } = string.Empty;
    public string GeneratedCSS { get; set; } = string.Empty;
    public string GeneratedJS { get; set; } = string.Empty;
    public string PreviewUrl { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public TimeSpan GenerationTime { get; set; }
    public List<string> Assets { get; set; } = new();
}

public enum PreviewMode
{
    Desktop,
    Tablet,
    Mobile,
    Interactive,
    Static
}

public class PublishOptions
{
    public PublishTarget Target { get; set; } = PublishTarget.Internal;
    public string PublishUrl { get; set; } = string.Empty;
    public bool GenerateAssets { get; set; } = true;
    public bool EnableComments { get; set; } = true;
    public bool EnableAnnotations { get; set; } = true;
    public bool PasswordProtected { get; set; } = false;
    public string? Password { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public Dictionary<string, object> CustomOptions { get; set; } = new();
}

public enum PublishTarget
{
    Internal,
    External,
    Client,
    Team,
    Public
}
