using PmtAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Domain.Persistance
{
    public interface IProjectRepository : IGenericRepository<Project>
    {
        Task<Project?> GetProjectByIdWithDetailsAsync(Guid id);
        Task<bool> SoftDeleteProjectAsync(Guid id);
        Task<int> GetTotalProjectCountAsync();
        Task<int> GetProjectTeamSizeAsync(Guid projectId);
        Task<(IReadOnlyList<Project> Projects, int TotalCount)> GetProjectsForTableAsync(
            int page,
            int pageSize,
            string? searchTerm,
            List<int>? statusIds,
            List<int>? deliveryUnitIds,
            List<int>? projectManagerIds);
        Task<IReadOnlyList<ProjectManagerInfo>> GetUniqueProjectManagersAsync();
        Task<Team?> GetTeamWithMembersAsync(int teamId, Guid projectId);
    }

    public class ProjectManagerInfo
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}