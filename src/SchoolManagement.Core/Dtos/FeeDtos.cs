using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Core.Dtos;

public sealed class FeeStructureDto
{
    public int Id { get; set; }
    public int? ClassId { get; set; }
    public string? ClassName { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsActive { get; set; }
}

public sealed class CreateFeeStructureRequest
{
    public int? ClassId { get; set; }
    [Required, MaxLength(12)]
    public string AcademicYear { get; set; } = string.Empty;
    [Required, MaxLength(150)]
    public string Title { get; set; } = string.Empty;
    [Range(0, 9999999)]
    public decimal Amount { get; set; }
    public DateTime? DueDate { get; set; }
}

public sealed class StudentFeeDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int FeeStructureId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public decimal AmountDue { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal Balance { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
}

public sealed class AssignFeeRequest
{
    [Required]
    public int StudentId { get; set; }
    [Required]
    public int FeeStructureId { get; set; }
}

public sealed class RecordPaymentRequest
{
    [Range(0.01, 9999999)]
    public decimal Amount { get; set; }
}

public sealed class PendingFeeDto
{
    public int StudentId { get; set; }
    public string AdmissionNumber { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string? ClassName { get; set; }
    public decimal PendingAmount { get; set; }
}
