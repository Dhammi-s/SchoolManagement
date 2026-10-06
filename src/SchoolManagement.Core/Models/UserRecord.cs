namespace SchoolManagement.Core.Models;

/// <summary>Row returned by usp_User_GetByUsername.</summary>
public sealed class UserRecord
{
    public int Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string PasswordHash { get; init; } = string.Empty;
    public int RoleId { get; init; }
    public string RoleName { get; init; } = string.Empty;
    public int? EmployeeId { get; init; }
    public int? StudentId { get; init; }
    public bool IsActive { get; init; }
}
