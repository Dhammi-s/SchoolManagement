using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;
using SchoolManagement.Core.Models;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ClassesController : ControllerBase
{
    private readonly IClassRepository _classes;

    public ClassesController(IClassRepository classes) => _classes = classes;

    /// <summary>List all classes with incharge name and student count.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ClassDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _classes.GetAllAsync(ct));

    /// <summary>Create a class (Principal / HeadMaster only).</summary>
    [HttpPost]
    [Authorize(Roles = $"{Roles.Principal},{Roles.HeadMaster}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateClassRequest request, CancellationToken ct)
    {
        var id = await _classes.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetAll), new { id }, new { id });
    }

    /// <summary>Assign (or clear) the class incharge teacher.</summary>
    [HttpPut("incharge")]
    [Authorize(Roles = $"{Roles.Principal},{Roles.HeadMaster}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AssignIncharge([FromBody] AssignInchargeRequest request, CancellationToken ct)
    {
        await _classes.AssignInchargeAsync(request.ClassId, request.EmployeeId, ct);
        return NoContent();
    }
}
