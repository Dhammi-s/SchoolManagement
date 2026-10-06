using System.Data;
using Dapper;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;

namespace SchoolManagement.Infrastructure.Repositories;

public sealed class BusRouteRepository : IBusRouteRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public BusRouteRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<IReadOnlyList<BusRouteDto>> GetAllAsync(CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var rows = await conn.QueryAsync<BusRouteDto>(
            new CommandDefinition("dbo.usp_BusRoute_GetAll", commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return rows.AsList();
    }

    public async Task<int> InsertAsync(CreateBusRouteRequest r, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var p = new DynamicParameters();
        p.Add("@RouteName", r.RouteName);
        p.Add("@VehicleNumber", r.VehicleNumber);
        p.Add("@DriverName", r.DriverName);
        p.Add("@DriverPhone", r.DriverPhone);
        p.Add("@Fee", r.Fee);
        p.Add("@NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        await conn.ExecuteAsync(new CommandDefinition("dbo.usp_BusRoute_Insert", p, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return p.Get<int>("@NewId");
    }
}
