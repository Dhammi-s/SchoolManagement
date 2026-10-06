using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;
using SchoolManagement.Core.Tenancy;

namespace SchoolManagement.Api.Controllers;

/// <summary>
/// Uploads profile pictures and student documents (images / PDFs) to Cloudinary.
/// Files are organised per tenant: {tenant-domain}/{folder}.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class MediaController : ControllerBase
{
    private const long MaxBytes = 15 * 1024 * 1024; // 15 MB

    private readonly IMediaStorage _storage;
    private readonly ITenantContext _tenant;

    public MediaController(IMediaStorage storage, ITenantContext tenant)
    {
        _storage = storage;
        _tenant = tenant;
    }

    /// <param name="folder">Logical folder, e.g. "students", "staff", "documents".</param>
    [HttpPost("upload")]
    [RequestSizeLimit(MaxBytes)]
    [ProducesResponseType(typeof(MediaUploadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string folder, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { error = "No file provided." });
        if (file.Length > MaxBytes)
            return BadRequest(new { error = "File exceeds the 15 MB limit." });

        var tenantFolder = _tenant.HasTenant ? _tenant.Current!.Domain : "shared";
        var safeFolder = string.IsNullOrWhiteSpace(folder) ? "misc" : folder.Trim();
        var fullFolder = $"school-management/{tenantFolder}/{safeFolder}";

        await using var stream = file.OpenReadStream();
        var result = await _storage.UploadAsync(stream, file.FileName, fullFolder, ct);
        return Ok(result);
    }
}
