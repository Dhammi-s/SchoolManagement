using System.Data;
using Dapper;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;

namespace SchoolManagement.Infrastructure.Repositories;

public sealed class SubjectRepository : ISubjectRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SubjectRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<IReadOnlyList<SubjectDto>> GetAllAsync(CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var rows = await conn.QueryAsync<SubjectDto>(
            new CommandDefinition("dbo.usp_Subject_GetAll", commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return rows.AsList();
    }

    public async Task<int> InsertAsync(CreateSubjectRequest request, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var p = new DynamicParameters();
        p.Add("@Name", request.Name);
        p.Add("@Code", request.Code);
        p.Add("@NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        await conn.ExecuteAsync(new CommandDefinition("dbo.usp_Subject_Insert", p, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return p.Get<int>("@NewId");
    }

    public async Task AssignToClassAsync(AssignClassSubjectRequest request, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(
            "dbo.usp_ClassSubject_Assign",
            new { request.ClassId, request.SubjectId, request.TeacherEmployeeId },
            commandType: CommandType.StoredProcedure,
            cancellationToken: ct));
    }
}
