using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;
using SchoolManagement.Core.Models;

namespace SchoolManagement.Api.Controllers;

/// <summary>
/// Student portal — the logged-in student's own profile, results and fees.
/// The student id is taken from the JWT, never from the URL.
/// </summary>
[ApiController]
[Route("api/me")]
[Authorize(Roles = Roles.Student)]
public sealed class PortalController : ControllerBase
{
    private readonly IStudentRepository _students;
    private readonly IPerformanceRepository _performance;
    private readonly IFeeRepository _fees;

    public PortalController(IStudentRepository students, IPerformanceRepository performance, IFeeRepository fees)
    {
        _students = students;
        _performance = performance;
        _fees = fees;
    }

    private int? CurrentStudentId =>
        int.TryParse(User.FindFirst("studentId")?.Value, out var id) ? id : null;

    [HttpGet("profile")]
    [ProducesResponseType(typeof(StudentDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Profile(CancellationToken ct)
    {
        if (CurrentStudentId is not int id) return NotFound(new { error = "No student profile linked to this account." });
        var student = await _students.GetByIdAsync(id, ct);
        return student is null ? NotFound() : Ok(student);
    }

    [HttpGet("results")]
    [ProducesResponseType(typeof(IReadOnlyList<StudentResultDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Results(CancellationToken ct)
    {
        if (CurrentStudentId is not int id) return Ok(Array.Empty<StudentResultDto>());
        return Ok(await _performance.GetResultsByStudentAsync(id, ct));
    }

    [HttpGet("fees")]
    [ProducesResponseType(typeof(IReadOnlyList<StudentFeeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Fees(CancellationToken ct)
    {
        if (CurrentStudentId is not int id) return Ok(Array.Empty<StudentFeeDto>());
        return Ok(await _fees.GetByStudentAsync(id, ct));
    }
}
