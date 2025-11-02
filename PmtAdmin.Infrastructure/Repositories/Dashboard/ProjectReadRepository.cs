using Microsoft.EntityFrameworkCore;
using PmtAdmin.Domain.Persistance.Dashboard;
using PmtAdmin.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Infrastructure.Repositories.Dashboard
{
    public class ProjectReadRepository : IProjectReadRepository
    {
        private readonly AppDbContext _context;
        public ProjectReadRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectProjection>> GetAllProjectProjectionsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Projects
                .AsNoTracking()
                .Select(p => new ProjectProjection
                {
                    Id = p.Id,
                    CreatedAt = p.CreatedAt,
                    DeliveryUnitId = p.DeliveryUnitId,
                    StatusId = p.StatusId
                })
                .ToListAsync(cancellationToken);
        }
    }
}
