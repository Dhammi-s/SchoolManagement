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
    Task SetPhotoAsync(int employeeId, string photoUrl, CancellationToken ct = default);
}

public interface IFeeRepository
{
    Task<IReadOnlyList<FeeStructureDto>> GetStructuresAsync(int? classId, CancellationToken ct = default);
    Task<int> CreateStructureAsync(CreateFeeStructureRequest request, CancellationToken ct = default);
    Task<int> AssignAsync(AssignFeeRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<StudentFeeDto>> GetByStudentAsync(int studentId, CancellationToken ct = default);
    Task RecordPaymentAsync(int studentFeeId, decimal amount, CancellationToken ct = default);
    Task<IReadOnlyList<PendingFeeDto>> GetPendingAsync(CancellationToken ct = default);
}

public interface IPerformanceRepository
{
    Task<int> CreateTestAsync(CreatePerformanceTestRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<PerformanceTestDto>> GetTestsAsync(int? classId, int? teacherEmployeeId, CancellationToken ct = default);
    Task SaveResultAsync(SaveTestResultRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<TestResultRowDto>> GetResultsByTestAsync(int performanceTestId, CancellationToken ct = default);
    Task<IReadOnlyList<StudentResultDto>> GetResultsByStudentAsync(int studentId, CancellationToken ct = default);
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
    Task<IReadOnlyList<StudentListItemDto>> GetAllAsync(int? classId, string? search, CancellationToken ct = default);
    Task<StudentDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> InsertAsync(CreateStudentRequest request, CancellationToken ct = default);
    Task SetPhotoAsync(int studentId, string photoUrl, CancellationToken ct = default);
}

public interface IBusRouteRepository
{
    Task<IReadOnlyList<BusRouteDto>> GetAllAsync(CancellationToken ct = default);
    Task<int> InsertAsync(CreateBusRouteRequest request, CancellationToken ct = default);
}

public interface IStudentInterestRepository
{
    Task<IReadOnlyList<StudentInterestDto>> GetByStudentAsync(int studentId, CancellationToken ct = default);
    Task<int> AddAsync(int studentId, AddInterestRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

public interface IStudentDocumentRepository
{
    Task<IReadOnlyList<StudentDocumentDto>> GetByStudentAsync(int studentId, CancellationToken ct = default);
    Task<int> AddAsync(int studentId, AddDocumentRequest request, CancellationToken ct = default);
}
