using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Core.Dtos;

public sealed class PerformanceTestDto
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
    public string Title { get; set; } = string.Empty;
    public DateTime TestDate { get; set; }
    public decimal MaxMarks { get; set; }
}

public sealed class CreatePerformanceTestRequest
{
    [Required]
    public int ClassId { get; set; }
    public int? SectionId { get; set; }
    [Required]
    public int SubjectId { get; set; }
    [Required]
    public int TeacherEmployeeId { get; set; }
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    [Required]
    public DateTime TestDate { get; set; }
    [Range(0, 9999)]
    public decimal MaxMarks { get; set; }
}

/// <summary>One row per student in a test's class (result may be unentered).</summary>
public sealed class TestResultRowDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string? RollNumber { get; set; }
    public int? ResultId { get; set; }
    public decimal? MarksObtained { get; set; }
    public string? Grade { get; set; }
    public string? Remarks { get; set; }
}

public sealed class SaveTestResultRequest
{
    [Required]
    public int PerformanceTestId { get; set; }
    [Required]
    public int StudentId { get; set; }
    public decimal? MarksObtained { get; set; }
    public string? Grade { get; set; }
    public string? Remarks { get; set; }
}

/// <summary>A student's own result line (student portal).</summary>
public sealed class StudentResultDto
{
    public int ResultId { get; set; }
    public int PerformanceTestId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime TestDate { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public decimal MaxMarks { get; set; }
    public decimal? MarksObtained { get; set; }
    public string? Grade { get; set; }
    public string? Remarks { get; set; }
}
