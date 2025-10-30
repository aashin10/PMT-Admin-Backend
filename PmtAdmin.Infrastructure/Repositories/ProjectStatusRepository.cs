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
    public class ProjectStatusRepository : GenericRepository<ProjectStatus>, IProjectStatusRepository
    {
        private readonly AppDbContext _context;

        public ProjectStatusRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
