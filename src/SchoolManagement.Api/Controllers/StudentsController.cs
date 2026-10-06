using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class StudentsController : ControllerBase
{
    private readonly IStudentRepository _students;

    public StudentsController(IStudentRepository students) => _students = students;

    /// <summary>Students in a class (i.e. the students under that class's incharge teacher).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<StudentSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByClass([FromQuery] int classId, CancellationToken ct)
        => Ok(await _students.GetByClassAsync(classId, ct));
}
