namespace SchoolManagement.Core.Models;

/// <summary>Canonical role names (must match the Roles seed in the tenant DB).</summary>
public static class Roles
{
    public const string Principal = "Principal";
    public const string HeadMaster = "HeadMaster";
    public const string Teacher = "Teacher";
    public const string Accountant = "Accountant";
    public const string Student = "Student";

    public static readonly string[] All = [Principal, HeadMaster, Teacher, Accountant, Student];
}
