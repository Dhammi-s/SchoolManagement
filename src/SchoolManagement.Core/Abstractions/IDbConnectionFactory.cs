using System.Data;

namespace SchoolManagement.Core.Abstractions;

/// <summary>
/// Creates open ADO.NET connections. Master = the registry DB; Tenant = the
/// current school's DB (resolved per request from <see cref="Tenancy.ITenantContext"/>).
/// </summary>
public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateMasterConnectionAsync(CancellationToken ct = default);

    Task<IDbConnection> CreateTenantConnectionAsync(CancellationToken ct = default);

    Task<IDbConnection> CreateConnectionAsync(string connectionString, CancellationToken ct = default);
}
