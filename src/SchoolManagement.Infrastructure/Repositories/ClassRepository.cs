using System.Data;
using Dapper;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;

namespace SchoolManagement.Infrastructure.Repositories;

public sealed class ClassRepository : IClassRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ClassRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<IReadOnlyList<ClassDto>> GetAllAsync(CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var rows = await conn.QueryAsync<ClassDto>(
            new CommandDefinition(
                "dbo.usp_Class_GetAll",
                commandType: CommandType.StoredProcedure,
                cancellationToken: ct));
        return rows.AsList();
    }

    public async Task<int> CreateAsync(CreateClassRequest request, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var parameters = new DynamicParameters();
        parameters.Add("@Name", request.Name);
        parameters.Add("@AcademicYear", request.AcademicYear);
        parameters.Add("@ClassInchargeEmployeeId", request.ClassInchargeEmployeeId);
        parameters.Add("@NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);

        await conn.ExecuteAsync(
            new CommandDefinition(
                "dbo.usp_Class_Insert",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: ct));

        return parameters.Get<int>("@NewId");
    }

    public async Task AssignInchargeAsync(int classId, int? employeeId, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        await conn.ExecuteAsync(
            new CommandDefinition(
                "dbo.usp_Class_AssignIncharge",
                new { ClassId = classId, EmployeeId = employeeId },
                commandType: CommandType.StoredProcedure,
                cancellationToken: ct));
    }
}
