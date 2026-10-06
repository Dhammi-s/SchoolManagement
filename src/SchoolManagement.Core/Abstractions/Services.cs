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

/// <summary>Creates a login account for an admitted student and (optionally) emails the credentials.</summary>
public interface IStudentAccountService
{
    Task<CreateLoginResult> CreateLoginAsync(int studentId, string? username, string? password, CancellationToken ct = default);
}

/// <summary>Sends transactional email (Brevo). Returns false when not configured or on failure — never throws.</summary>
public interface IEmailSender
{
    Task<bool> SendCredentialsAsync(string toEmail, string toName, string username, string tempPassword, string schoolName, CancellationToken ct = default);
}

/// <summary>Uploads documents / images to the configured media store (Cloudinary).</summary>
public interface IMediaStorage
{
    Task<MediaUploadResponse> UploadAsync(Stream content, string fileName, string folder, CancellationToken ct = default);
}
