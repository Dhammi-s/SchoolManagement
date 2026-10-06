using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Dtos;
using SchoolManagement.Core.Models;
using SchoolManagement.Core.Tenancy;
using SchoolManagement.Infrastructure.Security;

namespace SchoolManagement.Infrastructure.Services;

public sealed class StudentAccountService : IStudentAccountService
{
    private readonly IStudentRepository _students;
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailSender _email;
    private readonly ITenantContext _tenant;

    public StudentAccountService(
        IStudentRepository students,
        IUserRepository users,
        IPasswordHasher passwordHasher,
        IEmailSender email,
        ITenantContext tenant)
    {
        _students = students;
        _users = users;
        _passwordHasher = passwordHasher;
        _email = email;
        _tenant = tenant;
    }

    public async Task<CreateLoginResult> CreateLoginAsync(int studentId, string? username, string? password, CancellationToken ct = default)
    {
        var student = await _students.GetByIdAsync(studentId, ct)
            ?? throw new InvalidOperationException("Student not found.");

        var finalUsername = !string.IsNullOrWhiteSpace(username) ? username!.Trim() : student.AdmissionNumber;
        var finalPassword = !string.IsNullOrWhiteSpace(password) ? password! : PasswordGenerator.Generate();

        var hash = _passwordHasher.Hash(finalPassword);
        var userId = await _users.InsertAsync(finalUsername, hash, Roles.Student, null, studentId, ct);

        var result = new CreateLoginResult
        {
            UserId = userId,
            Username = finalUsername,
            TempPassword = finalPassword,
            Email = student.Email,
        };

        if (!string.IsNullOrWhiteSpace(student.Email))
        {
            var fullName = $"{student.FirstName} {student.LastName}".Trim();
            var school = _tenant.Current?.SchoolName ?? "School";
            result.Emailed = await _email.SendCredentialsAsync(student.Email!, fullName, finalUsername, finalPassword, school, ct);
        }

        return result;
    }
}
