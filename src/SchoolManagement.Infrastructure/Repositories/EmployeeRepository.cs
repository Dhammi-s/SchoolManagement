using System.Data;
using Dapper;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;

namespace SchoolManagement.Infrastructure.Repositories;

public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public EmployeeRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<IReadOnlyList<EmployeeDto>> GetByRoleAsync(string? roleName, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var rows = await conn.QueryAsync<EmployeeDto>(
            new CommandDefinition(
                "dbo.usp_Employee_GetByRole",
                new { RoleName = roleName },
                commandType: CommandType.StoredProcedure,
                cancellationToken: ct));
        return rows.AsList();
    }

    public async Task<int> InsertAsync(CreateTeacherRequest r, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var p = new DynamicParameters();
        p.Add("@EmployeeCode", r.EmployeeCode);
        p.Add("@FirstName", r.FirstName);
        p.Add("@LastName", r.LastName);
        p.Add("@RoleName", string.IsNullOrWhiteSpace(r.RoleName) ? "Teacher" : r.RoleName);
        p.Add("@Designation", r.Designation);
        p.Add("@Email", r.Email);
        p.Add("@Phone", r.Phone);
        p.Add("@Gender", r.Gender);
        p.Add("@DateOfBirth", r.DateOfBirth);
        p.Add("@Qualification", r.Qualification);
        p.Add("@Address", r.Address);
        p.Add("@DateOfJoining", r.DateOfJoining);
        p.Add("@PhotoUrl", r.PhotoUrl);
        p.Add("@NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        await conn.ExecuteAsync(new CommandDefinition("dbo.usp_Employee_Insert", p, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return p.Get<int>("@NewId");
    }

    public async Task SetPhotoAsync(int employeeId, string photoUrl, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(
            "dbo.usp_Employee_SetPhoto", new { EmployeeId = employeeId, PhotoUrl = photoUrl },
            commandType: CommandType.StoredProcedure, cancellationToken: ct));
    }
}
