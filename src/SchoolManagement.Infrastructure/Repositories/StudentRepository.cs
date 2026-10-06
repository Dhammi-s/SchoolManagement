using System.Data;
using Dapper;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;

namespace SchoolManagement.Infrastructure.Repositories;

public sealed class StudentRepository : IStudentRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public StudentRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<IReadOnlyList<StudentSummaryDto>> GetByClassAsync(int classId, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var rows = await conn.QueryAsync<StudentSummaryDto>(
            new CommandDefinition("dbo.usp_Student_GetByClass", new { ClassId = classId }, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return rows.AsList();
    }
}
