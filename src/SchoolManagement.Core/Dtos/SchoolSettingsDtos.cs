namespace SchoolManagement.Core.Dtos;

/// <summary>Full settings (Principal-editable).</summary>
public sealed class SchoolSettingsDto
{
    public string? SchoolName { get; set; }
    public string? LogoUrl { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public string? LoginBackgroundUrl { get; set; }
    public string? LoginTitle { get; set; }
    public string? LoginSubtitle { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Public, pre-login branding used to theme the login page. Safe subset of settings.
/// </summary>
public sealed class BrandingDto
{
    public string? SchoolName { get; set; }
    public string? LogoUrl { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public string? LoginBackgroundUrl { get; set; }
    public string? LoginTitle { get; set; }
    public string? LoginSubtitle { get; set; }
}
