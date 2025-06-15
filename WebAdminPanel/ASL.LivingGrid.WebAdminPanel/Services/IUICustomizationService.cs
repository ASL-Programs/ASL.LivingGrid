using ASL.LivingGrid.WebAdminPanel.Models;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public interface IUICustomizationService
{
    // Translation Management
    Task<TranslationProject> CreateTranslationProjectAsync(string name, List<string> supportedLanguages);
    Task<TranslationProject?> GetTranslationProjectAsync(string projectId);
    Task<IEnumerable<TranslationProject>> GetAllTranslationProjectsAsync();
    Task<bool> UpdateTranslationProjectAsync(TranslationProject project);
    Task<bool> DeleteTranslationProjectAsync(string projectId);
    
    Task<TranslationKey> AddTranslationKeyAsync(string projectId, string key, Dictionary<string, string> translations);
    Task<bool> UpdateTranslationKeyAsync(TranslationKey translationKey);
    Task<bool> DeleteTranslationKeyAsync(string keyId);
    Task<IEnumerable<TranslationKey>> GetTranslationKeysAsync(string projectId, string? languageCode = null);
    Task<string> GetTranslationAsync(string projectId, string key, string languageCode);
    Task<Dictionary<string, string>> GetAllTranslationsAsync(string projectId, string languageCode);
    
    Task<bool> ImportTranslationsAsync(string projectId, string filePath, ImportFormat format);
    Task<string> ExportTranslationsAsync(string projectId, string languageCode, ExportFormat format);
    Task<TranslationCoverage> GetTranslationCoverageAsync(string projectId);
    Task<IEnumerable<TranslationIssue>> ValidateTranslationsAsync(string projectId);
    
    // Theme Management
    Task<UITheme> CreateThemeAsync(string name, string description);
    Task<UITheme?> GetThemeAsync(string themeId);
    Task<IEnumerable<UITheme>> GetAllThemesAsync();
    Task<bool> UpdateThemeAsync(UITheme theme);
    Task<bool> DeleteThemeAsync(string themeId);
    Task<bool> SetActiveThemeAsync(string themeId, string? tenantId = null);
    Task<UITheme?> GetActiveThemeAsync(string? tenantId = null);
    
    Task<UITheme> CloneThemeAsync(string sourceThemeId, string newName);
    Task<string> GenerateThemeCSSAsync(string themeId);
    Task<ThemePreview> GenerateThemePreviewAsync(string themeId);
    Task<bool> ImportThemeAsync(string filePath);
    Task<string> ExportThemeAsync(string themeId);
    
    // Color Management
    Task<ColorPalette> CreateColorPaletteAsync(string name, Dictionary<string, string> colors);
    Task<ColorPalette?> GetColorPaletteAsync(string paletteId);
    Task<IEnumerable<ColorPalette>> GetAllColorPalettesAsync();
    Task<bool> UpdateColorPaletteAsync(ColorPalette palette);
    Task<bool> DeleteColorPaletteAsync(string paletteId);
    Task<IEnumerable<ColorPalette>> GetPredefinedColorPalettesAsync();
    Task<ColorPalette> GenerateColorPaletteFromImageAsync(string imagePath);
    Task<bool> ValidateColorContrastAsync(string foregroundColor, string backgroundColor);
    
    // Logo Management
    Task<LogoAsset> UploadLogoAsync(string name, byte[] imageData, string contentType);
    Task<LogoAsset?> GetLogoAsync(string logoId);
    Task<IEnumerable<LogoAsset>> GetAllLogosAsync();
    Task<bool> UpdateLogoAsync(LogoAsset logo);
    Task<bool> DeleteLogoAsync(string logoId);
    Task<bool> SetActiveLogoAsync(string logoId, string? tenantId = null);
    Task<LogoAsset?> GetActiveLogoAsync(string? tenantId = null);
    
    Task<LogoAsset> ResizeLogoAsync(string logoId, int width, int height);
    Task<LogoAsset> ConvertLogoFormatAsync(string logoId, ImageFormat targetFormat);
    Task<IEnumerable<LogoVariant>> GenerateLogoVariantsAsync(string logoId);
    
    // Real-time Updates
    Task<bool> PublishUIChangesAsync(string changesetId);
    Task<UIChangeset> CreateChangesetAsync(string name, string description);
    Task<bool> PreviewChangesAsync(string changesetId);
    Task<bool> RevertChangesAsync(string changesetId);
    Task<IEnumerable<UIChangeset>> GetChangeHistoryAsync();
    
    // Brand Management
    Task<BrandProfile> CreateBrandProfileAsync(string name, BrandSettings settings);
    Task<BrandProfile?> GetBrandProfileAsync(string profileId);
    Task<IEnumerable<BrandProfile>> GetAllBrandProfilesAsync();
    Task<bool> UpdateBrandProfileAsync(BrandProfile profile);
    Task<bool> ApplyBrandProfileAsync(string profileId, string? tenantId = null);
    
    // Accessibility
    Task<AccessibilityReport> CheckAccessibilityAsync(string themeId);
    Task<UITheme> ApplyAccessibilityFixesAsync(string themeId, AccessibilityLevel level);
    Task<bool> ValidateWCAGComplianceAsync(string themeId, WCAGLevel level);
}

public class TranslationProject
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> SupportedLanguages { get; set; } = new();
    public string DefaultLanguage { get; set; } = "en";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = Environment.UserName;
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;
    public List<TranslationKey> TranslationKeys { get; set; } = new();
    public TranslationSettings Settings { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public enum ProjectStatus
{
    Active,
    Inactive,
    Archived,
    Draft
}

public class TranslationSettings
{
    public bool EnableAutoTranslation { get; set; } = false;
    public string AutoTranslationProvider { get; set; } = "Google";
    public bool EnablePluralForms { get; set; } = true;
    public bool EnableContextualTranslations { get; set; } = true;
    public bool RequireReviewForAutoTranslations { get; set; } = true;
    public List<string> RestrictedKeys { get; set; } = new();
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

public class TranslationKey
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProjectId { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, string> Translations { get; set; } = new();
    public Dictionary<string, TranslationMetadata> TranslationMetadata { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = Environment.UserName;
    public bool IsRequired { get; set; } = true;
    public List<string> Tags { get; set; } = new();
    public string Context { get; set; } = string.Empty;
    public int MaxLength { get; set; } = 0;
    public bool SupportsPluralForms { get; set; } = false;
}

public class TranslationMetadata
{
    public string TranslatedBy { get; set; } = string.Empty;
    public DateTime TranslatedAt { get; set; } = DateTime.UtcNow;
    public TranslationStatus Status { get; set; } = TranslationStatus.Draft;
    public string ReviewedBy { get; set; } = string.Empty;
    public DateTime? ReviewedAt { get; set; }
    public bool IsAutoTranslated { get; set; } = false;
    public double ConfidenceScore { get; set; } = 0.0;
    public string Notes { get; set; } = string.Empty;
}

public enum TranslationStatus
{
    Draft,
    Translated,
    Review,
    Approved,
    Outdated
}

public enum ImportFormat
{
    JSON,
    CSV,
    Excel,
    XML,
    RESX,
    PO,
    XLIFF
}

public enum ExportFormat
{
    JSON,
    CSV,
    Excel,
    XML,
    RESX,
    PO,
    XLIFF
}

public class TranslationCoverage
{
    public string ProjectId { get; set; } = string.Empty;
    public Dictionary<string, LanguageCoverage> Languages { get; set; } = new();
    public int TotalKeys { get; set; }
    public double OverallCoverage { get; set; }
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
    public List<string> MissingKeys { get; set; } = new();
    public List<string> OutdatedKeys { get; set; } = new();
}

public class LanguageCoverage
{
    public string LanguageCode { get; set; } = string.Empty;
    public string LanguageName { get; set; } = string.Empty;
    public int TranslatedKeys { get; set; }
    public int TotalKeys { get; set; }
    public double CoveragePercentage { get; set; }
    public int ReviewedKeys { get; set; }
    public int ApprovedKeys { get; set; }
    public List<string> MissingKeys { get; set; } = new();
}

public class TranslationIssue
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string KeyId { get; set; } = string.Empty;
    public string LanguageCode { get; set; } = string.Empty;
    public IssueType Type { get; set; }
    public IssueSeverity Severity { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SuggestedFix { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
}

public enum IssueType
{
    MissingTranslation,
    EmptyTranslation,
    TooLong,
    InvalidPlaceholder,
    InconsistentTerminology,
    PotentialTypo,
    FormattingIssue
}

public enum IssueSeverity
{
    Info,
    Warning,
    Error,
    Critical
}

public class UITheme
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = Environment.UserName;
    public bool IsSystemTheme { get; set; } = false;
    public bool IsActive { get; set; } = false;
    public ThemeColors Colors { get; set; } = new();
    public ThemeTypography Typography { get; set; } = new();
    public ThemeSpacing Spacing { get; set; } = new();
    public ThemeBorders Borders { get; set; } = new();
    public ThemeShadows Shadows { get; set; } = new();
    public ThemeAnimations Animations { get; set; } = new();
    public Dictionary<string, string> CustomProperties { get; set; } = new();
    public string Version { get; set; } = "1.0.0";
    public List<string> Tags { get; set; } = new();
    public ThemeMetadata Metadata { get; set; } = new();
}

public class ThemeColors
{
    public string Primary { get; set; } = "#007bff";
    public string Secondary { get; set; } = "#6c757d";
    public string Success { get; set; } = "#28a745";
    public string Danger { get; set; } = "#dc3545";
    public string Warning { get; set; } = "#ffc107";
    public string Info { get; set; } = "#17a2b8";
    public string Light { get; set; } = "#f8f9fa";
    public string Dark { get; set; } = "#343a40";
    public string Background { get; set; } = "#ffffff";
    public string Surface { get; set; } = "#f8f9fa";
    public string OnPrimary { get; set; } = "#ffffff";
    public string OnSecondary { get; set; } = "#ffffff";
    public string OnBackground { get; set; } = "#000000";
    public string OnSurface { get; set; } = "#000000";
    public Dictionary<string, string> CustomColors { get; set; } = new();
}

public class ThemeTypography
{
    public string FontFamily { get; set; } = "Arial, sans-serif";
    public string HeadingFontFamily { get; set; } = "Arial, sans-serif";
    public string MonoFontFamily { get; set; } = "monospace";
    public double BaseFontSize { get; set; } = 16.0;
    public double LineHeight { get; set; } = 1.5;
    public Dictionary<string, FontStyle> FontStyles { get; set; } = new();
}

public class FontStyle
{
    public double FontSize { get; set; }
    public string FontWeight { get; set; } = "normal";
    public double LineHeight { get; set; }
    public double LetterSpacing { get; set; }
    public string TextTransform { get; set; } = "none";
}

public class ThemeSpacing
{
    public double BaseUnit { get; set; } = 8.0;
    public Dictionary<string, double> Sizes { get; set; } = new()
    {
        ["xs"] = 4.0,
        ["sm"] = 8.0,
        ["md"] = 16.0,
        ["lg"] = 24.0,
        ["xl"] = 32.0
    };
}

public class ThemeBorders
{
    public double DefaultWidth { get; set; } = 1.0;
    public string DefaultStyle { get; set; } = "solid";
    public string DefaultColor { get; set; } = "#dee2e6";
    public double DefaultRadius { get; set; } = 4.0;
    public Dictionary<string, double> RadiusSizes { get; set; } = new()
    {
        ["none"] = 0.0,
        ["sm"] = 2.0,
        ["md"] = 4.0,
        ["lg"] = 8.0,
        ["xl"] = 12.0,
        ["full"] = 9999.0
    };
}

public class ThemeShadows
{
    public Dictionary<string, string> Elevations { get; set; } = new()
    {
        ["none"] = "none",
        ["sm"] = "0 1px 2px rgba(0,0,0,0.05)",
        ["md"] = "0 4px 6px rgba(0,0,0,0.1)",
        ["lg"] = "0 10px 15px rgba(0,0,0,0.1)",
        ["xl"] = "0 20px 25px rgba(0,0,0,0.1)"
    };
}

public class ThemeAnimations
{
    public Dictionary<string, string> Durations { get; set; } = new()
    {
        ["fast"] = "150ms",
        ["normal"] = "300ms",
        ["slow"] = "500ms"
    };
    public Dictionary<string, string> Easings { get; set; } = new()
    {
        ["ease"] = "ease",
        ["ease-in"] = "ease-in",
        ["ease-out"] = "ease-out",
        ["ease-in-out"] = "ease-in-out"
    };
}

public class ThemeMetadata
{
    public string PreviewImage { get; set; } = string.Empty;
    public string ThumbnailImage { get; set; } = string.Empty;
    public string Category { get; set; } = "Custom";
    public string Author { get; set; } = string.Empty;
    public string License { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public bool IsDarkMode { get; set; } = false;
    public List<string> SupportedScreenSizes { get; set; } = new();
}

public class ThemePreview
{
    public string ThemeId { get; set; } = string.Empty;
    public string PreviewHTML { get; set; } = string.Empty;
    public string PreviewCSS { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public TimeSpan GenerationTime { get; set; }
}

public class ColorPalette
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, string> Colors { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = Environment.UserName;
    public bool IsPredefined { get; set; } = false;
    public string Category { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public ColorHarmony Harmony { get; set; } = ColorHarmony.Custom;
    public string BaseColor { get; set; } = string.Empty;
}

public enum ColorHarmony
{
    Custom,
    Monochromatic,
    Analogous,
    Complementary,
    Triadic,
    Tetradic,
    SplitComplementary
}

public class LogoAsset
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string ThumbnailPath { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = Environment.UserName;
    public bool IsActive { get; set; } = false;
    public List<LogoVariant> Variants { get; set; } = new();
    public LogoMetadata Metadata { get; set; } = new();
    public List<string> Tags { get; set; } = new();
}

public class LogoVariant
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ParentLogoId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
    public ImageFormat Format { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public LogoVariantType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum ImageFormat
{
    PNG,
    JPG,
    SVG,
    WebP,
    GIF,
    ICO
}

public enum LogoVariantType
{
    Original,
    Thumbnail,
    Small,
    Medium,
    Large,
    Icon,
    Favicon,
    Banner
}

public class LogoMetadata
{
    public string OriginalFileName { get; set; } = string.Empty;
    public bool HasTransparentBackground { get; set; } = false;
    public string DominantColor { get; set; } = string.Empty;
    public List<string> ExtractedColors { get; set; } = new();
    public string License { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public Dictionary<string, object> ExifData { get; set; } = new();
}

public class UIChangeset
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = Environment.UserName;
    public ChangesetStatus Status { get; set; } = ChangesetStatus.Draft;
    public List<UIChange> Changes { get; set; } = new();
    public DateTime? PublishedAt { get; set; }
    public string? PublishedBy { get; set; }
    public DateTime? RevertedAt { get; set; }
    public string? RevertedBy { get; set; }
    public string? TenantId { get; set; }
}

public enum ChangesetStatus
{
    Draft,
    Preview,
    Published,
    Reverted
}

public class UIChange
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public UIChangeType Type { get; set; }
    public string ComponentType { get; set; } = string.Empty;
    public string ComponentId { get; set; } = string.Empty;
    public string PropertyName { get; set; } = string.Empty;
    public string OldValue { get; set; } = string.Empty;
    public string NewValue { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public string ChangedBy { get; set; } = Environment.UserName;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public enum UIChangeType
{
    ThemeChange,
    ColorChange,
    LogoChange,
    TranslationChange,
    LayoutChange,
    ComponentChange
}

public class BrandProfile
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = Environment.UserName;
    public BrandSettings Settings { get; set; } = new();
    public bool IsActive { get; set; } = false;
    public string? TenantId { get; set; }
}

public class BrandSettings
{
    public string PrimaryLogoId { get; set; } = string.Empty;
    public string SecondaryLogoId { get; set; } = string.Empty;
    public string FaviconId { get; set; } = string.Empty;
    public string ThemeId { get; set; } = string.Empty;
    public string ColorPaletteId { get; set; } = string.Empty;
    public BrandColors BrandColors { get; set; } = new();
    public BrandTypography Typography { get; set; } = new();
    public Dictionary<string, string> CustomCSS { get; set; } = new();
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

public class BrandColors
{
    public string Primary { get; set; } = string.Empty;
    public string Secondary { get; set; } = string.Empty;
    public string Accent { get; set; } = string.Empty;
    public string Background { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}

public class BrandTypography
{
    public string PrimaryFont { get; set; } = string.Empty;
    public string SecondaryFont { get; set; } = string.Empty;
    public string FontWeights { get; set; } = string.Empty;
}

public class AccessibilityReport
{
    public string ThemeId { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public WCAGLevel ComplianceLevel { get; set; }
    public List<AccessibilityIssue> Issues { get; set; } = new();
    public AccessibilityMetrics Metrics { get; set; } = new();
    public List<AccessibilityRecommendation> Recommendations { get; set; } = new();
}

public enum WCAGLevel
{
    A,
    AA,
    AAA
}

public enum AccessibilityLevel
{
    Basic,
    Enhanced,
    Maximum
}

public class AccessibilityIssue
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public AccessibilityIssueType Type { get; set; }
    public WCAGLevel Level { get; set; }
    public string Component { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SuggestedFix { get; set; } = string.Empty;
    public IssueSeverity Severity { get; set; }
}

public enum AccessibilityIssueType
{
    ColorContrast,
    FontSize,
    FocusIndicator,
    AlternativeText,
    KeyboardNavigation,
    ScreenReader
}

public class AccessibilityMetrics
{
    public double ContrastRatio { get; set; }
    public bool MeetsWCAGAA { get; set; }
    public bool MeetsWCAGAAA { get; set; }
    public int TotalIssues { get; set; }
    public int CriticalIssues { get; set; }
    public double AccessibilityScore { get; set; }
}

public class AccessibilityRecommendation
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public AccessibilityImpact Impact { get; set; }
    public string Implementation { get; set; } = string.Empty;
    public bool CanAutoFix { get; set; } = false;
}

public enum AccessibilityImpact
{
    Low,
    Medium,
    High,
    Critical
}
