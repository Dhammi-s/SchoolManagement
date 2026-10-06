namespace SchoolManagement.Core.Tenancy;

/// <summary>
/// Per-request holder of the current tenant. Registered as scoped and populated
/// by the tenant-resolution middleware before any tenant data access occurs.
/// </summary>
public interface ITenantContext
{
    TenantInfo? Current { get; }

    bool HasTenant { get; }

    /// <summary>Connection string for the current tenant's database.</summary>
    string TenantConnectionString { get; }

    void SetTenant(TenantInfo tenant);
}
