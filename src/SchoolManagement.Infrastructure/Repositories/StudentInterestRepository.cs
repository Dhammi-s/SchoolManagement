using System.Data;
using Dapper;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;

namespace SchoolManagement.Infrastructure.Repositories;

public sealed class StudentInterestRepository : IStudentInterestRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public StudentInterestRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<IReadOnlyList<StudentInterestDto>> GetByStudentAsync(int studentId, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var rows = await conn.QueryAsync<StudentInterestDto>(
            new CommandDefinition("dbo.usp_StudentInterest_GetByStudent", new { StudentId = studentId }, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return rows.AsList();
    }

    public async Task<int> AddAsync(int studentId, AddInterestRequest r, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var p = new DynamicParameters();
        p.Add("@StudentId", studentId);
        p.Add("@InterestType", r.InterestType);
        p.Add("@InterestName", r.InterestName);
        p.Add("@NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        await conn.ExecuteAsync(new CommandDefinition("dbo.usp_StudentInterest_Add", p, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return p.Get<int>("@NewId");
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(
            "dbo.usp_StudentInterest_Delete", new { Id = id },
            commandType: CommandType.StoredProcedure, cancellationToken: ct));
    }
}
