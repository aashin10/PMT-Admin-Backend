using Microsoft.EntityFrameworkCore;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using PmtAdmin.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Infrastructure.Repositories
{
    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
        private readonly AppDbContext _context;

        public ProjectRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Project>> GetAllProjectsWithDetailsAsync()
        {
            return await _context.Projects
                .Include(p => p.Status)
                .Include(p => p.DeliveryUnit)
                .Include(p => p.ProjectManager)
                .Where(p => p.DeletedAt == null)
                .ToListAsync();
        }

        public async Task<Project?> GetProjectByIdWithDetailsAsync(Guid id)
        {
            return await _context.Projects
                .Include(p => p.Status)
                .Include(p => p.DeliveryUnit)
                .Include(p => p.ProjectManager)
                .Where(p => p.Id == id && p.DeletedAt == null)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> SoftDeleteProjectAsync(Guid id)
        {
            var project = await _context.Projects
                .Where(p => p.Id == id && p.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (project == null)
            {
                return false;
            }

            project.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
