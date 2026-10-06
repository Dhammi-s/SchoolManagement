using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Configuration;
using SchoolManagement.Core.Tenancy;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Infrastructure.Media;
using SchoolManagement.Infrastructure.Repositories;
using SchoolManagement.Infrastructure.Security;
using SchoolManagement.Infrastructure.Services;
using SchoolManagement.Infrastructure.Tenancy;

namespace SchoolManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<CloudinaryOptions>(configuration.GetSection(CloudinaryOptions.SectionName));
        services.Configure<TenancyOptions>(configuration.GetSection(TenancyOptions.SectionName));

        // Tenancy & data access (scoped: per-request tenant resolution).
        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<ITenantStore, TenantStore>();

        // Repositories.
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IClassRepository, ClassRepository>();
        services.AddScoped<ISchoolSettingsRepository, SchoolSettingsRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<ISubjectRepository, SubjectRepository>();
        services.AddScoped<ISectionRepository, SectionRepository>();
        services.AddScoped<ITimetableRepository, TimetableRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();

        // Services.
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IMediaStorage, CloudinaryService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITeacherService, TeacherService>();

        return services;
    }
}
