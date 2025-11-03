using PmtAdmin.Domain.Entities;

namespace PmtAdmin.Domain.Persistance
{
    public interface ICustomFieldRepository : IGenericRepository<CustomField>
    {
        Task<CustomField?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<CustomField>> GetByProjectIdAsync(Guid projectId);
    }
}
