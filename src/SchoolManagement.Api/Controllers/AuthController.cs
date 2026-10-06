using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;
using SchoolManagement.Core.Tenancy;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITenantContext _tenant;

    public AuthController(IAuthService authService, ITenantContext tenant)
    {
        _authService = authService;
        _tenant = tenant;
    }

    /// <summary>Authenticate a user within the current tenant (school).</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        if (!_tenant.HasTenant)
            return BadRequest(new { error = "No school/tenant resolved. Send the tenant domain header." });

        var result = await _authService.LoginAsync(request, ct);
        return result is null
            ? Unauthorized(new { error = "Invalid username or password." })
            : Ok(result);
    }
}
