using System.Data;
using Dapper;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;

namespace SchoolManagement.Infrastructure.Repositories;

public sealed class FeeRepository : IFeeRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public FeeRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<IReadOnlyList<FeeStructureDto>> GetStructuresAsync(int? classId, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var rows = await conn.QueryAsync<FeeStructureDto>(
            new CommandDefinition("dbo.usp_FeeStructure_GetAll", new { ClassId = classId }, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return rows.AsList();
    }

    public async Task<int> CreateStructureAsync(CreateFeeStructureRequest r, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var p = new DynamicParameters();
        p.Add("@ClassId", r.ClassId);
        p.Add("@AcademicYear", r.AcademicYear);
        p.Add("@Title", r.Title);
        p.Add("@Amount", r.Amount);
        p.Add("@DueDate", r.DueDate);
        p.Add("@NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        await conn.ExecuteAsync(new CommandDefinition("dbo.usp_FeeStructure_Insert", p, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return p.Get<int>("@NewId");
    }

    public async Task<int> AssignAsync(AssignFeeRequest r, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var p = new DynamicParameters();
        p.Add("@StudentId", r.StudentId);
        p.Add("@FeeStructureId", r.FeeStructureId);
        p.Add("@NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        await conn.ExecuteAsync(new CommandDefinition("dbo.usp_StudentFee_Assign", p, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return p.Get<int>("@NewId");
    }

    public async Task<IReadOnlyList<StudentFeeDto>> GetByStudentAsync(int studentId, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var rows = await conn.QueryAsync<StudentFeeDto>(
            new CommandDefinition("dbo.usp_StudentFee_GetByStudent", new { StudentId = studentId }, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return rows.AsList();
    }

    public async Task RecordPaymentAsync(int studentFeeId, decimal amount, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(
            "dbo.usp_StudentFee_RecordPayment", new { StudentFeeId = studentFeeId, Amount = amount },
            commandType: CommandType.StoredProcedure, cancellationToken: ct));
    }

    public async Task<IReadOnlyList<PendingFeeDto>> GetPendingAsync(CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var rows = await conn.QueryAsync<PendingFeeDto>(
            new CommandDefinition("dbo.usp_StudentFee_GetPending", commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return rows.AsList();
    }
}
