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
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<User>> GetActiveUsersAsync()
        {
            return await _context.User
                .Where(u => u.IsActive && !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<User>> GetAllNonDeletedUsersAsync()
        {
            return await _context.User
                .Where(u => !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.User
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<User?> GetByJiraIdAsync(string jiraId)
        {
            return await _context.User
                .FirstOrDefaultAsync(u => u.JiraId == jiraId && !u.IsDeleted);
        }

        public async Task DeleteUsersByIdsAsync(IEnumerable<int> ids)
        {
            var users = await _context.User
                .Where(u => ids.Contains(u.Id) && !u.IsDeleted)
                .ToListAsync();

            foreach (var user in users)
            {
                user.IsDeleted = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<User>> GetFilteredUsersAsync(string? type, string? status)
        {
            var query = _context.User.Where(u => !u.IsDeleted);

            // Filter by Type if provided
            if (!string.IsNullOrWhiteSpace(type))
            {
                // Normalize to capitalize first letter
                var normalizedType = NormalizeEnum(type);
                query = query.Where(u => u.Type == normalizedType);
            }

            // Filter by Status if provided
            if (!string.IsNullOrWhiteSpace(status))
            {
                var normalizedStatus = NormalizeEnum(status);
                if (normalizedStatus == "Active")
                {
                    query = query.Where(u => u.IsActive);
                }
                else if (normalizedStatus == "Inactive")
                {
                    query = query.Where(u => !u.IsActive);
                }
            }

            return await query.ToListAsync();
        }

        private string NormalizeEnum(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;

            // Capitalize first letter, lowercase rest
            var trimmed = value.Trim();
            return char.ToUpper(trimmed[0]) + trimmed.Substring(1).ToLower();
        }
    }
}
