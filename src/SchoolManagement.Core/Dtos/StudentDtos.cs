using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Core.Dtos;

public sealed class StudentListItemDto
{
    public int Id { get; set; }
    public string AdmissionNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string? Gender { get; set; }
    public string? RollNumber { get; set; }
    public int? ClassId { get; set; }
    public string? ClassName { get; set; }
    public int? SectionId { get; set; }
    public string? SectionName { get; set; }
    public string? PhotoUrl { get; set; }
    public string? GuardianName { get; set; }
    public string? GuardianPhone { get; set; }
    public bool UsesBusService { get; set; }
    public decimal PendingFees { get; set; }
}

public sealed class StudentDetailDto
{
    public int Id { get; set; }
    public string AdmissionNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? ClassId { get; set; }
    public string? ClassName { get; set; }
    public int? SectionId { get; set; }
    public string? SectionName { get; set; }
    public string? RollNumber { get; set; }
    public string? Address { get; set; }
    public string? GuardianName { get; set; }
    public string? GuardianPhone { get; set; }
    public string? PreviousSchoolName { get; set; }
    public string? PreviousSchoolDetails { get; set; }
    public bool UsesBusService { get; set; }
    public int? BusRouteId { get; set; }
    public string? BusRouteName { get; set; }
    public DateTime? AdmissionDate { get; set; }
    public string? PhotoUrl { get; set; }
    public bool IsActive { get; set; }
    public decimal PendingFees { get; set; }
}

public sealed class CreateStudentRequest
{
    [Required, MaxLength(30)]
    public string AdmissionNumber { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? LastName { get; set; }

    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? ClassId { get; set; }
    public int? SectionId { get; set; }
    public string? RollNumber { get; set; }
    public string? Address { get; set; }
    public string? GuardianName { get; set; }
    public string? GuardianPhone { get; set; }
    public string? PreviousSchoolName { get; set; }
    public string? PreviousSchoolDetails { get; set; }
    public bool UsesBusService { get; set; }
    public int? BusRouteId { get; set; }
    public DateTime? AdmissionDate { get; set; }
    public string? PhotoUrl { get; set; }
}

public sealed class BusRouteDto
{
    public int Id { get; set; }
    public string RouteName { get; set; } = string.Empty;
    public string? VehicleNumber { get; set; }
    public string? DriverName { get; set; }
    public string? DriverPhone { get; set; }
    public decimal Fee { get; set; }
    public bool IsActive { get; set; }
}

public sealed class CreateBusRouteRequest
{
    [Required, MaxLength(100)]
    public string RouteName { get; set; } = string.Empty;
    public string? VehicleNumber { get; set; }
    public string? DriverName { get; set; }
    public string? DriverPhone { get; set; }
    public decimal Fee { get; set; }
}

public sealed class StudentInterestDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string InterestType { get; set; } = string.Empty;
    public string InterestName { get; set; } = string.Empty;
}

public sealed class AddInterestRequest
{
    [Required, MaxLength(50)]
    public string InterestType { get; set; } = string.Empty;
    [Required, MaxLength(100)]
    public string InterestName { get; set; } = string.Empty;
}

public sealed class StudentDocumentDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string? PublicId { get; set; }
    public DateTime UploadedAt { get; set; }
}

public sealed class AddDocumentRequest
{
    [Required, MaxLength(100)]
    public string DocumentType { get; set; } = string.Empty;
    public string? FileName { get; set; }
    [Required, MaxLength(500)]
    public string FileUrl { get; set; } = string.Empty;
    public string? PublicId { get; set; }
}

public sealed class SetPhotoRequest
{
    [Required, MaxLength(500)]
    public string PhotoUrl { get; set; } = string.Empty;
}
