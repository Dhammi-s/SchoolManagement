using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;
using SchoolManagement.Core.Models;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class StudentsController : ControllerBase
{
    private const string ManageRoles = $"{Roles.Principal},{Roles.HeadMaster}";

    private readonly IStudentRepository _students;
    private readonly IStudentInterestRepository _interests;
    private readonly IStudentDocumentRepository _documents;
    private readonly IStudentAccountService _accounts;

    public StudentsController(
        IStudentRepository students,
        IStudentInterestRepository interests,
        IStudentDocumentRepository documents,
        IStudentAccountService accounts)
    {
        _students = students;
        _interests = interests;
        _documents = documents;
        _accounts = accounts;
    }

    /// <summary>Create a login account for a student and email the credentials (if an email is on file).</summary>
    [HttpPost("{id:int}/login")]
    [Authorize(Roles = ManageRoles)]
    [ProducesResponseType(typeof(CreateLoginResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateLogin(int id, [FromBody] CreateStudentLoginRequest request, CancellationToken ct)
    {
        var result = await _accounts.CreateLoginAsync(id, request.Username, request.Password, ct);
        return Ok(result);
    }

    /// <summary>List students, optionally filtered by class or a search term.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<StudentListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int? classId, [FromQuery] string? search, CancellationToken ct)
        => Ok(await _students.GetAllAsync(classId, search, ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(StudentDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var student = await _students.GetByIdAsync(id, ct);
        return student is null ? NotFound() : Ok(student);
    }

    /// <summary>Admit a new student.</summary>
    [HttpPost]
    [Authorize(Roles = ManageRoles)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Admit([FromBody] CreateStudentRequest request, CancellationToken ct)
    {
        var id = await _students.InsertAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}/photo")]
    [Authorize(Roles = ManageRoles)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetPhoto(int id, [FromBody] SetPhotoRequest request, CancellationToken ct)
    {
        await _students.SetPhotoAsync(id, request.PhotoUrl, ct);
        return NoContent();
    }

    // ---- Interests -------------------------------------------------------
    [HttpGet("{id:int}/interests")]
    [ProducesResponseType(typeof(IReadOnlyList<StudentInterestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInterests(int id, CancellationToken ct)
        => Ok(await _interests.GetByStudentAsync(id, ct));

    [HttpPost("{id:int}/interests")]
    [Authorize(Roles = ManageRoles)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> AddInterest(int id, [FromBody] AddInterestRequest request, CancellationToken ct)
    {
        var newId = await _interests.AddAsync(id, request, ct);
        return CreatedAtAction(nameof(GetInterests), new { id }, new { id = newId });
    }

    [HttpDelete("interests/{interestId:int}")]
    [Authorize(Roles = ManageRoles)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteInterest(int interestId, CancellationToken ct)
    {
        await _interests.DeleteAsync(interestId, ct);
        return NoContent();
    }

    // ---- Documents -------------------------------------------------------
    [HttpGet("{id:int}/documents")]
    [ProducesResponseType(typeof(IReadOnlyList<StudentDocumentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDocuments(int id, CancellationToken ct)
        => Ok(await _documents.GetByStudentAsync(id, ct));

    /// <summary>Record a document already uploaded to Cloudinary (via /api/media/upload).</summary>
    [HttpPost("{id:int}/documents")]
    [Authorize(Roles = ManageRoles)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> AddDocument(int id, [FromBody] AddDocumentRequest request, CancellationToken ct)
    {
        var newId = await _documents.AddAsync(id, request, ct);
        return CreatedAtAction(nameof(GetDocuments), new { id }, new { id = newId });
    }
}
