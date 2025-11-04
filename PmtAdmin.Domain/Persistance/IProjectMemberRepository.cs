using PmtAdmin.Domain.Entities;

namespace PmtAdmin.Domain.Persistance
{
    public interface IProjectMemberRepository : IGenericRepository<ProjectMember>
    {
        Task<ProjectMember?> GetByProjectAndUserAsync(Guid projectId, int userId);
        Task<IEnumerable<ProjectMember>> GetProjectMembersByProjectIdAsync(Guid projectId);
        Task<IEnumerable<ProjectMember>> GetProjectMembersByUserIdAsync(int userId);
    }
}