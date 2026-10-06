using SchoolManagement.Core.Dtos;
using SchoolManagement.Core.Models;

namespace SchoolManagement.Core.Abstractions;

public interface IUserRepository
{
    Task<UserRecord?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task UpdateLastLoginAsync(int userId, CancellationToken ct = default);
    Task<int> InsertAsync(string username, string passwordHash, string roleName, int? employeeId, int? studentId, CancellationToken ct = default);
}

public interface IClassRepository
{
    Task<IReadOnlyList<ClassDto>> GetAllAsync(CancellationToken ct = default);
    Task<int> CreateAsync(CreateClassRequest request, CancellationToken ct = default);
    Task AssignInchargeAsync(int classId, int? employeeId, CancellationToken ct = default);
}

public interface ISchoolSettingsRepository
{
    Task<SchoolSettingsDto?> GetAsync(CancellationToken ct = default);
    Task UpdateAsync(SchoolSettingsDto settings, CancellationToken ct = default);
}

public interface IEmployeeRepository
{
    Task<IReadOnlyList<EmployeeDto>> GetByRoleAsync(string? roleName, CancellationToken ct = default);
    Task<int> InsertAsync(CreateTeacherRequest request, CancellationToken ct = default);
}

public interface ISubjectRepository
{
    Task<IReadOnlyList<SubjectDto>> GetAllAsync(CancellationToken ct = default);
    Task<int> InsertAsync(CreateSubjectRequest request, CancellationToken ct = default);
    Task AssignToClassAsync(AssignClassSubjectRequest request, CancellationToken ct = default);
}

public interface ISectionRepository
{
    Task<IReadOnlyList<SectionDto>> GetByClassAsync(int classId, CancellationToken ct = default);
    Task<int> InsertAsync(CreateSectionRequest request, CancellationToken ct = default);
}

public interface ITimetableRepository
{
    Task<int> InsertAsync(CreateTimetablePeriodRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<TimetablePeriodDto>> GetByTeacherAsync(int teacherEmployeeId, CancellationToken ct = default);
    Task<IReadOnlyList<TimetablePeriodDto>> GetByClassAsync(int classId, CancellationToken ct = default);
}

public interface IStudentRepository
{
    Task<IReadOnlyList<StudentSummaryDto>> GetByClassAsync(int classId, CancellationToken ct = default);
}
