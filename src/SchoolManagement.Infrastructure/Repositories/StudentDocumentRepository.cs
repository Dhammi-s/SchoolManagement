using System.Data;
using Dapper;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;

namespace SchoolManagement.Infrastructure.Repositories;

public sealed class StudentDocumentRepository : IStudentDocumentRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public StudentDocumentRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<IReadOnlyList<StudentDocumentDto>> GetByStudentAsync(int studentId, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var rows = await conn.QueryAsync<StudentDocumentDto>(
            new CommandDefinition("dbo.usp_StudentDocument_GetByStudent", new { StudentId = studentId }, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return rows.AsList();
    }

    public async Task<int> AddAsync(int studentId, AddDocumentRequest r, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var p = new DynamicParameters();
        p.Add("@StudentId", studentId);
        p.Add("@DocumentType", r.DocumentType);
        p.Add("@FileName", r.FileName);
        p.Add("@FileUrl", r.FileUrl);
        p.Add("@PublicId", r.PublicId);
        p.Add("@NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        await conn.ExecuteAsync(new CommandDefinition("dbo.usp_StudentDocument_Add", p, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return p.Get<int>("@NewId");
    }
}
