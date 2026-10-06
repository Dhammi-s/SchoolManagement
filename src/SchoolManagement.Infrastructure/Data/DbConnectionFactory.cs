using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Tenancy;

namespace SchoolManagement.Infrastructure.Data;

public sealed class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _masterConnectionString;
    private readonly ITenantContext _tenantContext;

    public DbConnectionFactory(IConfiguration configuration, ITenantContext tenantContext)
    {
        _masterConnectionString = configuration.GetConnectionString("Master")
            ?? throw new InvalidOperationException("ConnectionStrings:Master is not configured.");
        _tenantContext = tenantContext;
    }

    public Task<IDbConnection> CreateMasterConnectionAsync(CancellationToken ct = default)
        => CreateConnectionAsync(_masterConnectionString, ct);

    public Task<IDbConnection> CreateTenantConnectionAsync(CancellationToken ct = default)
    {
        if (!_tenantContext.HasTenant)
            throw new InvalidOperationException(
                "No tenant resolved for the current request. Ensure the tenant domain header is sent.");

        return CreateConnectionAsync(_tenantContext.TenantConnectionString, ct);
    }

    public async Task<IDbConnection> CreateConnectionAsync(string connectionString, CancellationToken ct = default)
    {
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(ct);
        return connection;
    }
}
