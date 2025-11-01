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


        //public async Task<bool> DeleteSuperAdminAsync(int id)
        //{
        //    var superAdmin = await _context.User
        //        .FirstOrDefaultAsync(u => u.Id == id && u.IsSuperAdmin && !u.IsDeleted);

        //    if (superAdmin == null)
        //        return false;

        //    // Soft delete
        //    superAdmin.IsDeleted = true;
        //    _context.User.Update(superAdmin);
        //    await _context.SaveChangesAsync();

        //    return true;
        //}
    }
}
