using System.Data;
using Dapper;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Models;

namespace SchoolManagement.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<UserRecord?> GetByUsernameAsync(string username, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<UserRecord>(
            new CommandDefinition(
                "dbo.usp_User_GetByUsername",
                new { Username = username },
                commandType: CommandType.StoredProcedure,
                cancellationToken: ct));
    }

    public async Task UpdateLastLoginAsync(int userId, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        await conn.ExecuteAsync(
            new CommandDefinition(
                "dbo.usp_User_UpdateLastLogin",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure,
                cancellationToken: ct));
    }

    public async Task<int> InsertAsync(string username, string passwordHash, string roleName, int? employeeId, int? studentId, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        var p = new DynamicParameters();
        p.Add("@Username", username);
        p.Add("@PasswordHash", passwordHash);
        p.Add("@RoleName", roleName);
        p.Add("@EmployeeId", employeeId);
        p.Add("@StudentId", studentId);
        p.Add("@NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        await conn.ExecuteAsync(new CommandDefinition("dbo.usp_User_Insert", p, commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return p.Get<int>("@NewId");
    }
}
