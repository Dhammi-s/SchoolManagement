using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;
using SchoolManagement.Core.Models;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class TeachersController : ControllerBase
{
    private readonly IEmployeeRepository _employees;
    private readonly ITeacherService _teacherService;
    private readonly ITimetableRepository _timetable;

    public TeachersController(IEmployeeRepository employees, ITeacherService teacherService, ITimetableRepository timetable)
    {
        _employees = employees;
        _teacherService = teacherService;
        _timetable = timetable;
    }

    /// <summary>List teachers (or pass ?role= for other staff, e.g. Accountant).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] string role = Roles.Teacher, CancellationToken ct = default)
        => Ok(await _employees.GetByRoleAsync(string.IsNullOrWhiteSpace(role) ? null : role, ct));

    /// <summary>Hire a teacher: details + optional class incharge + optional login.</summary>
    [HttpPost]
    [Authorize(Roles = $"{Roles.Principal},{Roles.HeadMaster}")]
    [ProducesResponseType(typeof(CreateTeacherResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Hire([FromBody] CreateTeacherRequest request, CancellationToken ct)
    {
        var result = await _teacherService.HireAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { role = request.RoleName }, result);
    }

    /// <summary>A teacher's weekly schedule.</summary>
    [HttpGet("{id:int}/schedule")]
    [ProducesResponseType(typeof(IReadOnlyList<TimetablePeriodDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Schedule(int id, CancellationToken ct)
        => Ok(await _timetable.GetByTeacherAsync(id, ct));

    /// <summary>Set a staff member's profile photo (Cloudinary URL).</summary>
    [HttpPut("{id:int}/photo")]
    [Authorize(Roles = $"{Roles.Principal},{Roles.HeadMaster}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetPhoto(int id, [FromBody] SetPhotoRequest request, CancellationToken ct)
    {
        await _employees.SetPhotoAsync(id, request.PhotoUrl, ct);
        return NoContent();
    }
}
