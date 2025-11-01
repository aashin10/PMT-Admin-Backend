using Microsoft.EntityFrameworkCore;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using PmtAdmin.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PmtAdmin.Infrastructure.Repositories.PermissionRepository;

namespace PmtAdmin.Infrastructure.Repositories
{
    
        public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
    {
            private readonly AppDbContext _context;

            public PermissionRepository(AppDbContext context) : base(context)
            {
                _context = context;
            }

            public async Task<List<Permission>> GetAllAsync()
            {
                return await _context.Permissions.ToListAsync();
            }

            public async Task<Permission?> GetByIdAsync(int id)
            {
                return await _context.Permissions.FindAsync(id);
            }

            public async Task<List<Permission>> GetByIdAsync(IEnumerable<int> ids)
            {
                return await _context.Permissions
                    .Where(p => ids.Contains(p.Id))
                    .ToListAsync();
            }

            public async Task<Permission> CreateAsync(Permission permission)
            {
                _context.Permissions.Add(permission);
                await _context.SaveChangesAsync();
                return permission;
            }
        }

    }

