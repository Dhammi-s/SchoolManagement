using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Core.Dtos;

public sealed class TenantDto
{
    public int Id { get; set; }
    public string SchoolName { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string? DatabaseName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class CreateTenantRequest
{
    [Required, MaxLength(200)]
    public string SchoolName { get; set; } = string.Empty;

    [Required, MaxLength(256)]
    public string Domain { get; set; } = string.Empty;

    [MaxLength(128)]
    public string? DatabaseName { get; set; }

    [Required, MaxLength(1000)]
    public string ConnectionString { get; set; } = string.Empty;
}

public sealed class MediaUploadResponse
{
    public string Url { get; set; } = string.Empty;
    public string PublicId { get; set; } = string.Empty;
}
