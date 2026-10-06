using System.Data;
using Dapper;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;

namespace SchoolManagement.Infrastructure.Repositories;

public sealed class SchoolSettingsRepository : ISchoolSettingsRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SchoolSettingsRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<SchoolSettingsDto?> GetAsync(CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<SchoolSettingsDto>(
            new CommandDefinition(
                "dbo.usp_SchoolSettings_Get",
                commandType: CommandType.StoredProcedure,
                cancellationToken: ct));
    }

    public async Task UpdateAsync(SchoolSettingsDto settings, CancellationToken ct = default)
    {
        using var conn = await _connectionFactory.CreateTenantConnectionAsync(ct);
        await conn.ExecuteAsync(
            new CommandDefinition(
                "dbo.usp_SchoolSettings_Update",
                new
                {
                    settings.SchoolName,
                    settings.LogoUrl,
                    settings.PrimaryColor,
                    settings.SecondaryColor,
                    settings.LoginBackgroundUrl,
                    settings.LoginTitle,
                    settings.LoginSubtitle
                },
                commandType: CommandType.StoredProcedure,
                cancellationToken: ct));
    }
}
