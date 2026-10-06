using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;
using SchoolManagement.Core.Tenancy;
using SchoolManagement.Infrastructure.Security;

namespace SchoolManagement.Infrastructure.Services;

public sealed class TeacherService : ITeacherService
{
    private readonly IEmployeeRepository _employees;
    private readonly IClassRepository _classes;
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailSender _email;
    private readonly ITenantContext _tenant;

    public TeacherService(
        IEmployeeRepository employees,
        IClassRepository classes,
        IUserRepository users,
        IPasswordHasher passwordHasher,
        IEmailSender email,
        ITenantContext tenant)
    {
        _employees = employees;
        _classes = classes;
        _users = users;
        _passwordHasher = passwordHasher;
        _email = email;
        _tenant = tenant;
    }

    public async Task<CreateTeacherResponse> HireAsync(CreateTeacherRequest request, CancellationToken ct = default)
    {
        var employeeId = await _employees.InsertAsync(request, ct);

        if (request.ClassInchargeId is int classId)
            await _classes.AssignInchargeAsync(classId, employeeId, ct);

        var response = new CreateTeacherResponse { EmployeeId = employeeId };

        if (request.CreateLogin)
        {
            var roleName = string.IsNullOrWhiteSpace(request.RoleName) ? "Teacher" : request.RoleName;
            var username = !string.IsNullOrWhiteSpace(request.Username) ? request.Username!.Trim() : request.EmployeeCode;
            var password = !string.IsNullOrWhiteSpace(request.Password) ? request.Password! : PasswordGenerator.Generate();

            var hash = _passwordHasher.Hash(password);
            var userId = await _users.InsertAsync(username, hash, roleName, employeeId, null, ct);

            response.UserId = userId;
            response.Username = username;
            response.TempPassword = password;

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var fullName = $"{request.FirstName} {request.LastName}".Trim();
                var school = _tenant.Current?.SchoolName ?? "School";
                response.LoginEmailed = await _email.SendCredentialsAsync(request.Email!, fullName, username, password, school, ct);
            }
        }

        return response;
    }
}
