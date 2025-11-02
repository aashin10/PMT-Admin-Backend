using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PmtAdmin.Application.MappingProfiles;
using PmtAdmin.Application.Services;
using PmtAdmin.Domain.Persistance;
using PmtAdmin.Domain.Persistance.Dashboard;
using PmtAdmin.Domain.Persistance.Settings;
using PmtAdmin.Infrastructure.Context;
using PmtAdmin.Infrastructure.Repositories;
using PmtAdmin.Infrastructure.Repositories.Dashboard;
using PmtAdmin.Infrastructure.Repositories.Settings;
using PmtAdmin.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            services.AddScoped<IPasswordHashingService, PasswordHashingService>();
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<IProjectReadRepository, ProjectReadRepository>();
            services.AddScoped<ISuperAdminRepository, SuperAdminRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            //services.AddAutoMapper(typeof(RoleProfile).Assembly);
            services.AddScoped<IPermissionRepository, PermissionRepository>();


            return services;
        }
    }
}
