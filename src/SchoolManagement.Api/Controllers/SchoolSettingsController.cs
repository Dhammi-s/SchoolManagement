using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;
using SchoolManagement.Core.Models;

namespace SchoolManagement.Api.Controllers;

/// <summary>
/// School appearance / branding settings. Readable by any authenticated user;
/// editable by the Principal (login page + background are customisable per school).
/// </summary>
[ApiController]
[Route("api/school-settings")]
[Authorize]
public sealed class SchoolSettingsController : ControllerBase
{
    private readonly ISchoolSettingsRepository _settings;

    public SchoolSettingsController(ISchoolSettingsRepository settings) => _settings = settings;

    [HttpGet]
    [ProducesResponseType(typeof(SchoolSettingsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken ct)
        => Ok(await _settings.GetAsync(ct) ?? new SchoolSettingsDto());

    [HttpPut]
    [Authorize(Roles = Roles.Principal)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update([FromBody] SchoolSettingsDto settings, CancellationToken ct)
    {
        await _settings.UpdateAsync(settings, ct);
        return NoContent();
    }
}
