using SchoolManagement.Core.Tenancy;

namespace SchoolManagement.Infrastructure.Tenancy;

/// <summary>Scoped per request. Populated by the tenant-resolution middleware.</summary>
public sealed class TenantContext : ITenantContext
{
    public TenantInfo? Current { get; private set; }

    public bool HasTenant => Current is not null;

    public string TenantConnectionString =>
        Current?.ConnectionString
        ?? throw new InvalidOperationException("No tenant has been resolved for this request.");

    public void SetTenant(TenantInfo tenant) => Current = tenant;
}
