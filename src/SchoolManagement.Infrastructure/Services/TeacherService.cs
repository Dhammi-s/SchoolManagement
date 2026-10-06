using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;

namespace SchoolManagement.Infrastructure.Services;

public sealed class TeacherService : ITeacherService
{
    private readonly IEmployeeRepository _employees;
    private readonly IClassRepository _classes;
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;

    public TeacherService(
        IEmployeeRepository employees,
        IClassRepository classes,
        IUserRepository users,
        IPasswordHasher passwordHasher)
    {
        _employees = employees;
        _classes = classes;
        _users = users;
        _passwordHasher = passwordHasher;
    }

    public async Task<CreateTeacherResponse> HireAsync(CreateTeacherRequest request, CancellationToken ct = default)
    {
        // 1. Create the employee record.
        var employeeId = await _employees.InsertAsync(request, ct);

        // 2. Assign class incharge, if requested.
        if (request.ClassInchargeId is int classId)
            await _classes.AssignInchargeAsync(classId, employeeId, ct);

        // 3. Create a login account, if requested.
        int? userId = null;
        if (request.CreateLogin && !string.IsNullOrWhiteSpace(request.Username) && !string.IsNullOrWhiteSpace(request.Password))
        {
            var hash = _passwordHasher.Hash(request.Password);
            var roleName = string.IsNullOrWhiteSpace(request.RoleName) ? "Teacher" : request.RoleName;
            userId = await _users.InsertAsync(request.Username, hash, roleName, employeeId, null, ct);
        }

        return new CreateTeacherResponse { EmployeeId = employeeId, UserId = userId };
    }
}
