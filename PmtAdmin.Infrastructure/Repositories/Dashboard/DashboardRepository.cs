using Microsoft.EntityFrameworkCore;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance.Dashboard;
using PmtAdmin.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Infrastructure.Repositories.Dashboard
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly AppDbContext _context;

        public DashboardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(IReadOnlyList<Project> Projects, IReadOnlyList<DeliveryUnit> DeliveryUnits)> GetDashboardDataAsync()
        {
            var projects = await _context.Projects
                .Include(p => p.Status)
                .Include(p => p.DeliveryUnit)
                .Where(p => p.DeletedAt == null)  // Only include non-deleted projects
                .ToListAsync();

            var deliveryUnits = await _context.DeliveryUnits
                .Where(du => du.IsActive)  // Only include active delivery units
                .ToListAsync();

            return (projects, deliveryUnits);
        }
    }
}
