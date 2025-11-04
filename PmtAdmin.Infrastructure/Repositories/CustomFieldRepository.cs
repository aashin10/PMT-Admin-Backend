using Microsoft.EntityFrameworkCore;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using PmtAdmin.Infrastructure.Context;

namespace PmtAdmin.Infrastructure.Repositories
{
    public class CustomFieldRepository : GenericRepository<CustomField>, ICustomFieldRepository
    {
        private readonly AppDbContext _context;

        public CustomFieldRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<CustomField?> GetByIdAsync(Guid id)
        {
            return await _context.Set<CustomField>()
                .FirstOrDefaultAsync(cf => cf.Id == id);
        }

        public async Task<IReadOnlyList<CustomField>> GetByProjectIdAsync(Guid projectId)
        {
            return await _context.Set<CustomField>()
                .Where(cf => cf.ProjectId == projectId)
                .OrderBy(cf => cf.Name)
                .ToListAsync();
        }
    }
}
