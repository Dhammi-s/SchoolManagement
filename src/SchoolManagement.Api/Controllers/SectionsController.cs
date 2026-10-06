using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;
using SchoolManagement.Core.Models;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class SectionsController : ControllerBase
{
    private readonly ISectionRepository _sections;

    public SectionsController(ISectionRepository sections) => _sections = sections;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SectionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByClass([FromQuery] int classId, CancellationToken ct)
        => Ok(await _sections.GetByClassAsync(classId, ct));

    [HttpPost]
    [Authorize(Roles = $"{Roles.Principal},{Roles.HeadMaster}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateSectionRequest request, CancellationToken ct)
    {
        var id = await _sections.InsertAsync(request, ct);
        return CreatedAtAction(nameof(GetByClass), new { classId = request.ClassId }, new { id });
    }
}
