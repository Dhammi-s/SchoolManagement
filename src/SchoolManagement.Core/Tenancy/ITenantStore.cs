namespace SchoolManagement.Core.Tenancy;

/// <summary>
/// Reads the master registry to resolve tenants by domain and to list/create them.
/// </summary>
public interface ITenantStore
{
    Task<TenantInfo?> GetByDomainAsync(string domain, CancellationToken ct = default);

    Task<IReadOnlyList<TenantInfo>> GetAllAsync(bool activeOnly = false, CancellationToken ct = default);

    Task<int> CreateAsync(string schoolName, string domain, string? databaseName, string connectionString, CancellationToken ct = default);
}
