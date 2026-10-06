using System.Data;
using Dapper;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;

namespace SchoolManagement.Infrastructure.Repositories;

public sealed class TimetableRepository : ITimetableRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TimetableRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<int> InsertAsync(CreateTimetablePeriodRequest r, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var p = new DynamicParameters();
        p.Add("@ClassId", r.ClassId);
        p.Add("@SectionId", r.SectionId);
        p.Add("@SubjectId", r.SubjectId);
        p.Add("@TeacherEmployeeId", r.TeacherEmployeeId);
        p.Add("@DayOfWeek", r.DayOfWeek);
        p.Add("@PeriodNumber", r.PeriodNumber);
        p.Add("@StartTime", r.StartTime);
        p.Add("@EndTime", r.EndTime);
        p.Add("@NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        await conn.ExecuteAsync(new CommandDefinition("dbo.usp_Timetable_Insert", p, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return p.Get<int>("@NewId");
    }

    public async Task<IReadOnlyList<TimetablePeriodDto>> GetByTeacherAsync(int teacherEmployeeId, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var rows = await conn.QueryAsync<TimetablePeriodDto>(
            new CommandDefinition("dbo.usp_Timetable_GetByTeacher", new { TeacherEmployeeId = teacherEmployeeId }, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return rows.AsList();
    }

    public async Task<IReadOnlyList<TimetablePeriodDto>> GetByClassAsync(int classId, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var rows = await conn.QueryAsync<TimetablePeriodDto>(
            new CommandDefinition("dbo.usp_Timetable_GetByClass", new { ClassId = classId }, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return rows.AsList();
    }
}
