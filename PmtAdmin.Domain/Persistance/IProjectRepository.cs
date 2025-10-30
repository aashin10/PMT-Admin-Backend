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
        Task<IReadOnlyList<Project>> GetAllProjectsWithDetailsAsync();
        Task<Project?> GetProjectByIdWithDetailsAsync(Guid id);
        Task<bool> SoftDeleteProjectAsync(Guid id);
    }
}
