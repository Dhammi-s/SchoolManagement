using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;
using SchoolManagement.Core.Models;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class TimetableController : ControllerBase
{
    private readonly ITimetableRepository _timetable;

    public TimetableController(ITimetableRepository timetable) => _timetable = timetable;

    /// <summary>Add a lecture period (Principal / HeadMaster set the schedule).</summary>
    [HttpPost]
    [Authorize(Roles = $"{Roles.Principal},{Roles.HeadMaster}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateTimetablePeriodRequest request, CancellationToken ct)
    {
        var id = await _timetable.InsertAsync(request, ct);
        return CreatedAtAction(nameof(GetByClass), new { classId = request.ClassId }, new { id });
    }

    [HttpGet("class/{classId:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<TimetablePeriodDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByClass(int classId, CancellationToken ct)
        => Ok(await _timetable.GetByClassAsync(classId, ct));

    [HttpGet("teacher/{teacherId:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<TimetablePeriodDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByTeacher(int teacherId, CancellationToken ct)
        => Ok(await _timetable.GetByTeacherAsync(teacherId, ct));
}
