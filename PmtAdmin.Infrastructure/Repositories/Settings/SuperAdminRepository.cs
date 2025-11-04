using Microsoft.EntityFrameworkCore;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance.Settings;
using PmtAdmin.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Infrastructure.Repositories.Settings
{
    public class SuperAdminRepository : GenericRepository<User>, ISuperAdminRepository
    {
        private readonly AppDbContext _context;
        public SuperAdminRepository(AppDbContext context) : base(context) 
        {
            _context = context;
        }

        public async Task<List<User>> GetSuperAdminsAsync()
        {
            return await _context.User
                .Where(u => u.IsSuperAdmin && !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.User
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task<int> CountActiveSuperAdminsAsync()
        {
            return await _context.User
                .CountAsync(u => u.IsSuperAdmin && !u.IsDeleted);
        }

        public async Task<int> CountActiveEnabledSuperAdminsAsync()
        {
            return await _context.User
                .CountAsync(u => u.IsSuperAdmin && !u.IsDeleted && u.IsActive);
        }

        public async Task<User> SoftDeleteAsync(User user)
        {
            // Ensure the entity is being tracked
            _context.User.Attach(user);
            
            // Update soft delete properties
            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            
            // Mark properties as modified
            _context.Entry(user).Property(x => x.IsDeleted).IsModified = true;
            _context.Entry(user).Property(x => x.DeletedAt).IsModified = true;
            _context.Entry(user).Property(x => x.DeletedBy).IsModified = true;

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            // Ensure the entity is being tracked
            _context.User.Attach(user);

            // Mark necessary properties as modified
            _context.Entry(user).Property(x => x.Name).IsModified = true;
            _context.Entry(user).Property(x => x.Email).IsModified = true;
            _context.Entry(user).Property(x => x.IsActive).IsModified = true;
            _context.Entry(user).Property(x => x.UpdatedAt).IsModified = true;
            _context.Entry(user).Property(x => x.UpdatedBy).IsModified = true;

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> CreateAsync(User user)
        {
            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

    }
}
