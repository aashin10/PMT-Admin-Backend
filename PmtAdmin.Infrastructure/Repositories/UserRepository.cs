using Microsoft.EntityFrameworkCore;
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
    public class UserRepository : GenericRepository<Users>, IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Users>> GetActiveUsersAsync()
        {
            return await _context.Users
                .Where(u => u.Is_Active && !u.Is_Deleted)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Users>> GetAllNonDeletedUsersAsync()
        {
            return await _context.Users
                .Where(u => !u.Is_Deleted)
                .ToListAsync();
        }

        public async Task<Users?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && !u.Is_Deleted);
        }

        public async Task<Users?> GetByJiraIdAsync(string jiraId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Jira_Id == jiraId && !u.Is_Deleted);
        }

        public async Task DeleteUsersByIdsAsync(IEnumerable<int> ids)
        {
            var users = await _context.Users
                .Where(u => ids.Contains(u.Id) && !u.Is_Deleted)
                .ToListAsync();

            foreach (var user in users)
            {
                user.Is_Deleted = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}
