using System.Data;
using Dapper;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;

namespace SchoolManagement.Infrastructure.Repositories;

public sealed class PerformanceRepository : IPerformanceRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PerformanceRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<int> CreateTestAsync(CreatePerformanceTestRequest r, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var p = new DynamicParameters();
        p.Add("@ClassId", r.ClassId);
        p.Add("@SectionId", r.SectionId);
        p.Add("@SubjectId", r.SubjectId);
        p.Add("@TeacherEmployeeId", r.TeacherEmployeeId);
        p.Add("@Title", r.Title);
        p.Add("@TestDate", r.TestDate);
        p.Add("@MaxMarks", r.MaxMarks);
        p.Add("@NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        await conn.ExecuteAsync(new CommandDefinition("dbo.usp_PerformanceTest_Insert", p, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return p.Get<int>("@NewId");
    }

    public async Task<IReadOnlyList<PerformanceTestDto>> GetTestsAsync(int? classId, int? teacherEmployeeId, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var rows = await conn.QueryAsync<PerformanceTestDto>(
            new CommandDefinition("dbo.usp_PerformanceTest_GetByClass",
                new { ClassId = classId, TeacherEmployeeId = teacherEmployeeId },
                commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return rows.AsList();
    }

    public async Task SaveResultAsync(SaveTestResultRequest r, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(
            "dbo.usp_TestResult_Save",
            new { r.PerformanceTestId, r.StudentId, r.MarksObtained, r.Grade, r.Remarks },
            commandType: CommandType.StoredProcedure, cancellationToken: ct));
    }

    public async Task<IReadOnlyList<TestResultRowDto>> GetResultsByTestAsync(int performanceTestId, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var rows = await conn.QueryAsync<TestResultRowDto>(
            new CommandDefinition("dbo.usp_TestResult_GetByTest", new { PerformanceTestId = performanceTestId }, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return rows.AsList();
    }

    public async Task<IReadOnlyList<StudentResultDto>> GetResultsByStudentAsync(int studentId, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var rows = await conn.QueryAsync<StudentResultDto>(
            new CommandDefinition("dbo.usp_TestResult_GetByStudent", new { StudentId = studentId }, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return rows.AsList();
    }
}
