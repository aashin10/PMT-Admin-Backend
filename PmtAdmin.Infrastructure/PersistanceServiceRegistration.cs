using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PmtAdmin.Domain.Persistance;
using PmtAdmin.Infrastructure.Context;
using PmtAdmin.Infrastructure.Repositories;
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
                    configuration.GetConnectionString("PmtAdminDbConnection")
                )
            );

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IJiraDatabaseService, JiraDatabaseService>();
            services.AddScoped<IJiraService, JiraService>();

            return services;
        }
    }
}
