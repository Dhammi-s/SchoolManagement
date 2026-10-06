using SchoolManagement.Core.Dtos;
using SchoolManagement.Core.Models;

namespace SchoolManagement.Core.Abstractions;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

public interface IJwtTokenService
{
    (string token, DateTime expiresAtUtc) CreateToken(UserRecord user, string schoolDomain);
}

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default);
}

/// <summary>Orchestrates hiring a teacher/staff member: employee + optional incharge + optional login.</summary>
public interface ITeacherService
{
    Task<CreateTeacherResponse> HireAsync(CreateTeacherRequest request, CancellationToken ct = default);
}

/// <summary>Uploads documents / images to the configured media store (Cloudinary).</summary>
public interface IMediaStorage
{
    Task<MediaUploadResponse> UploadAsync(Stream content, string fileName, string folder, CancellationToken ct = default);
}
