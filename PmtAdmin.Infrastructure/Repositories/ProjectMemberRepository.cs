using Microsoft.EntityFrameworkCore;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using PmtAdmin.Infrastructure.Context;

namespace PmtAdmin.Infrastructure.Repositories
{
    public class ProjectMemberRepository : GenericRepository<ProjectMember>, IProjectMemberRepository
    {
        private readonly AppDbContext _appDbContext;

        public ProjectMemberRepository(AppDbContext context) : base(context)
        {
            _appDbContext = context;
        }

        public async Task<ProjectMember?> GetByProjectAndUserAsync(Guid projectId, int userId)
        {
            return await _appDbContext.ProjectMembers
                .Include(pm => pm.User)
                .Include(pm => pm.Role)
                .Include(pm => pm.Project)
                .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);
        }

        public async Task<IEnumerable<ProjectMember>> GetProjectMembersByProjectIdAsync(Guid projectId)
        {
            return await _appDbContext.ProjectMembers
                .Include(pm => pm.User)
                .Include(pm => pm.Role)
                .Where(pm => pm.ProjectId == projectId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProjectMember>> GetProjectMembersByUserIdAsync(int userId)
        {
            return await _appDbContext.ProjectMembers
                .Include(pm => pm.Project)
                .Include(pm => pm.Role)
                .Where(pm => pm.UserId == userId)
                .ToListAsync();
        }
    }
}