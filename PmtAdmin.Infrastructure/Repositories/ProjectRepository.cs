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

        public async Task<(IReadOnlyList<Project> Projects, int TotalCount)> GetProjectsForTableAsync(
            int page,
            int pageSize,
            string? searchTerm,
            List<int>? statusIds,
            List<int>? deliveryUnitIds,
            List<int>? projectManagerIds)
        {
            // Start with base query - only include what's needed for table view
            var query = _context.Projects
                .Include(p => p.Status)
                .Include(p => p.DeliveryUnit)
                .Include(p => p.ProjectManager)
                .Include(p => p.ProjectMembers) // For team size calculation
                .Where(p => p.DeletedAt == null)
                .AsQueryable();

            // Apply search filter (project name, key, or manager name)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.ToLower();
                query = query.Where(p =>
                    (p.Name != null && p.Name.ToLower().Contains(search)) ||
                    (p.Key != null && p.Key.ToLower().Contains(search)) ||
                    (p.ProjectManager != null && p.ProjectManager.Name != null &&
                     p.ProjectManager.Name.ToLower().Contains(search))
                );
            }

            // Apply multi-select status filter
            if (statusIds != null && statusIds.Any())
            {
                query = query.Where(p => p.StatusId.HasValue && statusIds.Contains(p.StatusId.Value));
            }

            // Apply multi-select delivery unit filter
            if (deliveryUnitIds != null && deliveryUnitIds.Any())
            {
                query = query.Where(p => p.DeliveryUnitId.HasValue && deliveryUnitIds.Contains(p.DeliveryUnitId.Value));
            }

            // Apply multi-select project manager filter
            if (projectManagerIds != null && projectManagerIds.Any())
            {
                query = query.Where(p => p.ProjectManagerId.HasValue && projectManagerIds.Contains(p.ProjectManagerId.Value));
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply pagination with default sorting by CreatedAt descending
            var projects = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (projects, totalCount);
        }

        public async Task<Project?> GetProjectByIdWithDetailsAsync(Guid id)
        {
            return await _context.Projects
                .Include(p => p.Status)
                .Include(p => p.DeliveryUnit)
                .Include(p => p.ProjectManager)
                .Include(p => p.ProjectMembers)
                    .ThenInclude(pm => pm.User)
                .Include(p => p.ProjectMembers)
                    //.ThenInclude(pm => pm.Team)
                .Include(p => p.Sprints)
                .Include(p => p.CustomFields)
                .Include(p => p.Teams)
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

        public async Task<int> GetTotalProjectCountAsync()
        {
            return await _context.Projects
                .Where(p => p.DeletedAt == null)
                .CountAsync();
        }

        public async Task<int> GetProjectTeamSizeAsync(Guid projectId)
        {
            return await _context.ProjectMembers
                .Where(pm => pm.ProjectId == projectId)
                .CountAsync();
        }
    }
}