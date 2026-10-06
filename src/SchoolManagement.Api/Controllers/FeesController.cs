using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;
using SchoolManagement.Core.Models;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class FeesController : ControllerBase
{
    private const string ManageRoles = $"{Roles.Principal},{Roles.Accountant}";

    private readonly IFeeRepository _fees;

    public FeesController(IFeeRepository fees) => _fees = fees;

    [HttpGet("structures")]
    [ProducesResponseType(typeof(IReadOnlyList<FeeStructureDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStructures([FromQuery] int? classId, CancellationToken ct)
        => Ok(await _fees.GetStructuresAsync(classId, ct));

    [HttpPost("structures")]
    [Authorize(Roles = ManageRoles)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateStructure([FromBody] CreateFeeStructureRequest request, CancellationToken ct)
    {
        var id = await _fees.CreateStructureAsync(request, ct);
        return CreatedAtAction(nameof(GetStructures), new { id }, new { id });
    }

    [HttpPost("assign")]
    [Authorize(Roles = ManageRoles)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Assign([FromBody] AssignFeeRequest request, CancellationToken ct)
    {
        var id = await _fees.AssignAsync(request, ct);
        return CreatedAtAction(nameof(GetByStudent), new { studentId = request.StudentId }, new { id });
    }

    [HttpGet("student/{studentId:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<StudentFeeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByStudent(int studentId, CancellationToken ct)
        => Ok(await _fees.GetByStudentAsync(studentId, ct));

    [HttpPost("{studentFeeId:int}/payment")]
    [Authorize(Roles = ManageRoles)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RecordPayment(int studentFeeId, [FromBody] RecordPaymentRequest request, CancellationToken ct)
    {
        await _fees.RecordPaymentAsync(studentFeeId, request.Amount, ct);
        return NoContent();
    }

    [HttpGet("pending")]
    [Authorize(Roles = ManageRoles + $",{Roles.HeadMaster}")]
    [ProducesResponseType(typeof(IReadOnlyList<PendingFeeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPending(CancellationToken ct)
        => Ok(await _fees.GetPendingAsync(ct));
}
