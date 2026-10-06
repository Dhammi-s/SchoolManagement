using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;
using SchoolManagement.Core.Tenancy;

namespace SchoolManagement.Api.Controllers;

/// <summary>
/// Public, pre-login branding for the current tenant. Used by the frontend to
/// theme the login page per school (logo, colours, background, titles).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class BrandingController : ControllerBase
{
    private readonly ISchoolSettingsRepository _settings;
    private readonly ITenantContext _tenant;

    public BrandingController(ISchoolSettingsRepository settings, ITenantContext tenant)
    {
        _settings = settings;
        _tenant = tenant;
    }

    [HttpGet]
    [ProducesResponseType(typeof(BrandingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        if (!_tenant.HasTenant)
            return BadRequest(new { error = "No school/tenant resolved. Send the tenant domain header." });

        var s = await _settings.GetAsync(ct);
        var branding = new BrandingDto
        {
            SchoolName = s?.SchoolName ?? _tenant.Current!.SchoolName,
            LogoUrl = s?.LogoUrl,
            PrimaryColor = s?.PrimaryColor,
            SecondaryColor = s?.SecondaryColor,
            LoginBackgroundUrl = s?.LoginBackgroundUrl,
            LoginTitle = s?.LoginTitle,
            LoginSubtitle = s?.LoginSubtitle
        };
        return Ok(branding);
    }
}
