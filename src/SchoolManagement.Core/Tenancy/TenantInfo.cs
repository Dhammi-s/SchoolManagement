namespace SchoolManagement.Core.Tenancy;

/// <summary>
/// A resolved tenant (school). Produced from the master registry by matching
/// the incoming request domain.
/// </summary>
public sealed class TenantInfo
{
    public int Id { get; init; }
    public string SchoolName { get; init; } = string.Empty;
    public string Domain { get; init; } = string.Empty;
    public string? DatabaseName { get; init; }
    public string ConnectionString { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}
