namespace SchoolManagement.Core.Configuration;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Issuer { get; set; } = "SchoolManagement";
    public string Audience { get; set; } = "SchoolManagement";
    public string Key { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 480;
}

public sealed class CloudinaryOptions
{
    public const string SectionName = "Cloudinary";
    public string CloudName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
}

public sealed class TenancyOptions
{
    public const string SectionName = "Tenancy";

    /// <summary>Request header the frontend sends with the tenant's domain.</summary>
    public string HeaderName { get; set; } = "X-Tenant-Domain";

    /// <summary>Allow a ?tenant= query override (useful for Swagger / local testing).</summary>
    public bool AllowQueryStringOverride { get; set; } = true;
}
