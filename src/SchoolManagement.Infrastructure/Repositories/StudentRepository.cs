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

    public async Task<IReadOnlyList<StudentListItemDto>> GetAllAsync(int? classId, string? search, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var rows = await conn.QueryAsync<StudentListItemDto>(
            new CommandDefinition("dbo.usp_Student_GetAll", new { ClassId = classId, Search = search }, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return rows.AsList();
    }

    public async Task<StudentDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<StudentDetailDto>(
            new CommandDefinition("dbo.usp_Student_GetById", new { Id = id }, commandType: CommandType.StoredProcedure, cancellationToken: ct));
    }

    public async Task<int> InsertAsync(CreateStudentRequest r, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var p = new DynamicParameters();
        p.Add("@AdmissionNumber", r.AdmissionNumber);
        p.Add("@FirstName", r.FirstName);
        p.Add("@LastName", r.LastName);
        p.Add("@Gender", r.Gender);
        p.Add("@DateOfBirth", r.DateOfBirth);
        p.Add("@ClassId", r.ClassId);
        p.Add("@SectionId", r.SectionId);
        p.Add("@RollNumber", r.RollNumber);
        p.Add("@Address", r.Address);
        p.Add("@GuardianName", r.GuardianName);
        p.Add("@GuardianPhone", r.GuardianPhone);
        p.Add("@PreviousSchoolName", r.PreviousSchoolName);
        p.Add("@PreviousSchoolDetails", r.PreviousSchoolDetails);
        p.Add("@UsesBusService", r.UsesBusService);
        p.Add("@BusRouteId", r.BusRouteId);
        p.Add("@AdmissionDate", r.AdmissionDate);
        p.Add("@PhotoUrl", r.PhotoUrl);
        p.Add("@NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        await conn.ExecuteAsync(new CommandDefinition("dbo.usp_Student_Insert", p, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return p.Get<int>("@NewId");
    }

    public async Task SetPhotoAsync(int studentId, string photoUrl, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(
            "dbo.usp_Student_SetPhoto", new { StudentId = studentId, PhotoUrl = photoUrl },
            commandType: CommandType.StoredProcedure, cancellationToken: ct));
    }
}
