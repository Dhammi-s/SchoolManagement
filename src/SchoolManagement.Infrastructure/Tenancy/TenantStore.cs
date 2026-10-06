using Dapper;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Tenancy;

namespace SchoolManagement.Infrastructure.Tenancy;

public sealed class TenantStore : ITenantStore
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TenantStore(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<TenantInfo?> GetByDomainAsync(string domain, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateMasterConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<TenantInfo>(
            new CommandDefinition(
                "dbo.usp_Tenant_GetByDomain",
                new { Domain = domain },
                commandType: System.Data.CommandType.StoredProcedure,
                cancellationToken: ct));
    }

    public async Task<IReadOnlyList<TenantInfo>> GetAllAsync(bool activeOnly = false, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateMasterConnectionAsync(ct);
        var rows = await conn.QueryAsync<TenantInfo>(
            new CommandDefinition(
                "dbo.usp_Tenant_GetAll",
                new { ActiveOnly = activeOnly },
                commandType: System.Data.CommandType.StoredProcedure,
                cancellationToken: ct));
        return rows.AsList();
    }

    public async Task<int> CreateAsync(string schoolName, string domain, string? databaseName, string connectionString, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateMasterConnectionAsync(ct);
        var parameters = new DynamicParameters();
        parameters.Add("@SchoolName", schoolName);
        parameters.Add("@Domain", domain);
        parameters.Add("@DatabaseName", databaseName);
        parameters.Add("@ConnectionString", connectionString);
        parameters.Add("@NewId", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

        await conn.ExecuteAsync(
            new CommandDefinition(
                "dbo.usp_Tenant_Create",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure,
                cancellationToken: ct));

        return parameters.Get<int>("@NewId");
    }
}
