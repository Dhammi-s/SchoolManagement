using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Core.Dtos;

public sealed class EmployeeDto
{
    public int Id { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? Designation { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Qualification { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfJoining { get; set; }
    public string? PhotoUrl { get; set; }
    public bool IsActive { get; set; }
    /// <summary>Comma-separated names of classes this employee is incharge of.</summary>
    public string? InchargeClasses { get; set; }
}

/// <summary>
/// Hire a teacher (or other staff): capture details, optionally assign the class
/// they are incharge of, and optionally create a login account.
/// </summary>
public sealed class CreateTeacherRequest
{
    [Required, MaxLength(30)]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? LastName { get; set; }

    /// <summary>Role name. Defaults to Teacher.</summary>
    public string RoleName { get; set; } = "Teacher";

    public string? Designation { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Qualification { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfJoining { get; set; }
    public string? PhotoUrl { get; set; }

    /// <summary>Class this teacher will be incharge of (optional).</summary>
    public int? ClassInchargeId { get; set; }

    /// <summary>Create a login account for this teacher.</summary>
    public bool CreateLogin { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}

public sealed class CreateTeacherResponse
{
    public int EmployeeId { get; set; }
    public int? UserId { get; set; }
}

public sealed class AssignInchargeRequest
{
    [Required]
    public int ClassId { get; set; }
    public int? EmployeeId { get; set; }
}
