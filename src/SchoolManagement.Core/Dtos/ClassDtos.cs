using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Core.Dtos;

public sealed class ClassDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public int? ClassInchargeEmployeeId { get; set; }
    public string? ClassInchargeName { get; set; }
    public int StudentCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class CreateClassRequest
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(12)]
    public string AcademicYear { get; set; } = string.Empty;

    public int? ClassInchargeEmployeeId { get; set; }
}
