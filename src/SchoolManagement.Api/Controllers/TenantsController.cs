using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Dtos;
using SchoolManagement.Core.Tenancy;

namespace SchoolManagement.Api.Controllers;

/// <summary>
/// Master-registry administration: register schools (tenants) and list them.
/// This is cross-tenant (operates on the master DB), so it is guarded by an
/// admin API key sent in the "X-Admin-Key" header rather than a tenant login.
/// Configure the key via "Admin:ApiKey".
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class TenantsController : ControllerBase
{
    private readonly ITenantStore _tenants;
    private readonly IConfiguration _configuration;

    public TenantsController(ITenantStore tenants, IConfiguration configuration)
    {
        _tenants = tenants;
        _configuration = configuration;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TenantDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool activeOnly, CancellationToken ct)
    {
        if (!IsAdmin())
            return Unauthorized(new { error = "Admin key required." });

        var tenants = await _tenants.GetAllAsync(activeOnly, ct);
        var dtos = tenants.Select(t => new TenantDto
        {
            Id = t.Id,
            SchoolName = t.SchoolName,
            Domain = t.Domain,
            DatabaseName = t.DatabaseName,
            IsActive = t.IsActive
        });
        return Ok(dtos);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateTenantRequest request, CancellationToken ct)
    {
        if (!IsAdmin())
            return Unauthorized(new { error = "Admin key required." });

        var id = await _tenants.CreateAsync(
            request.SchoolName,
            request.Domain.Trim().ToLowerInvariant(),
            request.DatabaseName,
            request.ConnectionString,
            ct);

        return CreatedAtAction(nameof(GetAll), new { id }, new { id });
    }

    private bool IsAdmin()
    {
        var configured = _configuration["Admin:ApiKey"];
        if (string.IsNullOrWhiteSpace(configured))
            return false;

        return Request.Headers.TryGetValue("X-Admin-Key", out var provided)
               && string.Equals(provided.ToString(), configured, StringComparison.Ordinal);
    }
}
