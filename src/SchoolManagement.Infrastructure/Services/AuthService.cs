using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;
using SchoolManagement.Core.Tenancy;

namespace SchoolManagement.Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwt;
    private readonly ITenantContext _tenant;

    public AuthService(
        IUserRepository users,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwt,
        ITenantContext tenant)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _jwt = jwt;
        _tenant = tenant;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _users.GetByUsernameAsync(request.Username, ct);
        if (user is null || !user.IsActive)
            return null;

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            return null;

        var domain = _tenant.Current?.Domain ?? string.Empty;
        var (token, expires) = _jwt.CreateToken(user, domain);

        await _users.UpdateLastLoginAsync(user.Id, ct);

        return new LoginResponse
        {
            Token = token,
            ExpiresAtUtc = expires,
            UserId = user.Id,
            Username = user.Username,
            Role = user.RoleName,
            EmployeeId = user.EmployeeId,
            StudentId = user.StudentId,
            SchoolName = _tenant.Current?.SchoolName ?? string.Empty
        };
    }
}
