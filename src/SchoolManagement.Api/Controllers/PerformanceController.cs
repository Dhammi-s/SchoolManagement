using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;
using SchoolManagement.Core.Models;

namespace SchoolManagement.Api.Controllers;

/// <summary>
/// Teacher-entered performance / test results (subject-wise, per class).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class PerformanceController : ControllerBase
{
    private const string TeachRoles = $"{Roles.Principal},{Roles.HeadMaster},{Roles.Teacher}";

    private readonly IPerformanceRepository _performance;

    public PerformanceController(IPerformanceRepository performance) => _performance = performance;

    [HttpPost("tests")]
    [Authorize(Roles = TeachRoles)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateTest([FromBody] CreatePerformanceTestRequest request, CancellationToken ct)
    {
        var id = await _performance.CreateTestAsync(request, ct);
        return CreatedAtAction(nameof(GetTests), new { classId = request.ClassId }, new { id });
    }

    [HttpGet("tests")]
    [ProducesResponseType(typeof(IReadOnlyList<PerformanceTestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTests([FromQuery] int? classId, [FromQuery] int? teacherId, CancellationToken ct)
        => Ok(await _performance.GetTestsAsync(classId, teacherId, ct));

    [HttpGet("tests/{id:int}/results")]
    [ProducesResponseType(typeof(IReadOnlyList<TestResultRowDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetResults(int id, CancellationToken ct)
        => Ok(await _performance.GetResultsByTestAsync(id, ct));

    [HttpPost("results")]
    [Authorize(Roles = TeachRoles)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SaveResult([FromBody] SaveTestResultRequest request, CancellationToken ct)
    {
        await _performance.SaveResultAsync(request, ct);
        return NoContent();
    }

    [HttpGet("student/{studentId:int}/results")]
    [ProducesResponseType(typeof(IReadOnlyList<StudentResultDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentResults(int studentId, CancellationToken ct)
        => Ok(await _performance.GetResultsByStudentAsync(studentId, ct));
}
