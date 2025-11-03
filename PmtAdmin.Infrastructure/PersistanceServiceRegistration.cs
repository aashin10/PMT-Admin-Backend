using BACKEND_CQRS.Domain.Persistance;
using BACKEND_CQRS.Domain.Services;
using BACKEND_CQRS.Infrastructure.Repository;
using BACKEND_CQRS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PmtAdmin.Application.MappingProfiles;
using BACKEND_CQRS.Infrastructure.Services;
using PmtAdmin.Application.Services;
using PmtAdmin.Domain.Persistance;
using PmtAdmin.Domain.Persistance.Dashboard;
using PmtAdmin.Domain.Persistance.Settings;
using PmtAdmin.Infrastructure.Context;
using PmtAdmin.Infrastructure.Repositories;
using PmtAdmin.Infrastructure.Repositories.Dashboard;
using PmtAdmin.Infrastructure.Repositories.Settings;
using PmtAdmin.Infrastructure.Services;
using PmtAdmin.Infrastructure.Services.Jira;

namespace PmtAdmin.Infrastructure
{
    public static class PersistanceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add PostgreSQL Connection
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection")
                )
            );

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IProjectStatusRepository, ProjectStatusRepository>();
            services.AddScoped<IDuRepository, DeliveryUnitRepository>();

            //services.AddScoped<IPasswordHashingService, PasswordHashingService>();
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<IProjectReadRepository, ProjectReadRepository>();
            services.AddScoped<ISuperAdminRepository, SuperAdminRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            //services.AddAutoMapper(typeof(RoleProfile).Assembly);
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IBoardBoardColumnMapRepository, BoardBoardColumnMapRepository>();
            services.AddScoped<IBoardColumnRepository, BoardColumnRepository>();
            services.AddScoped<IBoardRepository, BoardRepository>();

            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IJiraDatabaseService, JiraDatabaseService>();
            services.AddScoped<IJiraService, JiraService>();

            // Authentication & Security
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            //.AddScoped<IPasswordHashService, PasswordHashService>();
            //services.AddScoped<IPasswordHashingService, PasswordHashService>();

            //using BACKEND_CQRS.Domain.Services;
            //using BACKEND_CQRS.Infrastructure.Services;

            //services.AddScoped<IPasswordHashingService, PasswordHashService>();
            services.AddScoped<IPasswordHashService, PasswordHashService>();
          services.AddScoped<IPasswordHashingService, PasswordHashService>();

            // Register JWT token service
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            // Register refresh token repository
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();


            return services;
        }
    }
}
