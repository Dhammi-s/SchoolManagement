using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Core.Dtos;

public sealed class SubjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
}

public sealed class CreateSubjectRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(20)]
    public string? Code { get; set; }
}

public sealed class SectionDto
{
    public int Id { get; set; }
    public int ClassId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class CreateSectionRequest
{
    [Required]
    public int ClassId { get; set; }
    [Required, MaxLength(20)]
    public string Name { get; set; } = string.Empty;
}

public sealed class StudentSummaryDto
{
    public int Id { get; set; }
    public string AdmissionNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string? Gender { get; set; }
    public string? RollNumber { get; set; }
    public int? ClassId { get; set; }
    public int? SectionId { get; set; }
    public string? SectionName { get; set; }
    public string? PhotoUrl { get; set; }
    public string? GuardianName { get; set; }
    public string? GuardianPhone { get; set; }
}

public sealed class TimetablePeriodDto
{
    public int Id { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int? SectionId { get; set; }
    public string? SectionName { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int TeacherEmployeeId { get; set; }
    public string? TeacherName { get; set; }
    public byte DayOfWeek { get; set; }
    public byte PeriodNumber { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}

public sealed class CreateTimetablePeriodRequest
{
    [Required]
    public int ClassId { get; set; }
    public int? SectionId { get; set; }
    [Required]
    public int SubjectId { get; set; }
    [Required]
    public int TeacherEmployeeId { get; set; }
    [Range(1, 7)]
    public byte DayOfWeek { get; set; }
    [Range(1, 20)]
    public byte PeriodNumber { get; set; }
    [Required]
    public TimeSpan StartTime { get; set; }
    [Required]
    public TimeSpan EndTime { get; set; }
}

public sealed class AssignClassSubjectRequest
{
    [Required]
    public int ClassId { get; set; }
    [Required]
    public int SubjectId { get; set; }
    public int? TeacherEmployeeId { get; set; }
}
