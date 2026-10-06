using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;
using SchoolManagement.Core.Models;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class SubjectsController : ControllerBase
{
    private readonly ISubjectRepository _subjects;

    public SubjectsController(ISubjectRepository subjects) => _subjects = subjects;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SubjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct) => Ok(await _subjects.GetAllAsync(ct));

    [HttpPost]
    [Authorize(Roles = $"{Roles.Principal},{Roles.HeadMaster}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateSubjectRequest request, CancellationToken ct)
    {
        var id = await _subjects.InsertAsync(request, ct);
        return CreatedAtAction(nameof(GetAll), new { id }, new { id });
    }

    /// <summary>Assign which teacher teaches a subject in a class.</summary>
    [HttpPost("assign")]
    [Authorize(Roles = $"{Roles.Principal},{Roles.HeadMaster}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Assign([FromBody] AssignClassSubjectRequest request, CancellationToken ct)
    {
        await _subjects.AssignToClassAsync(request, ct);
        return NoContent();
    }
}
