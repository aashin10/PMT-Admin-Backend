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
                // Filter by the Status field directly
                query = query.Where(u => u.Status == normalizedStatus);
            }

            return await query.ToListAsync();
        }

        public async Task<(IReadOnlyList<User> Users, int TotalCount)> GetUsersWithPaginationAsync(
            int pageNumber,
            int pageSize,
            string sortBy,
            string sortOrder,
            string? type,
            string? status)
        {
            var query = _context.User.Where(u => !u.IsDeleted);

            // Apply filtering
            if (!string.IsNullOrWhiteSpace(type))
            {
                var normalizedType = NormalizeEnum(type);
                query = query.Where(u => u.Type == normalizedType);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var normalizedStatus = NormalizeEnum(status);
                // Filter by the Status field directly
                query = query.Where(u => u.Status == normalizedStatus);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = ApplySorting(query, sortBy, sortOrder);

            // Apply pagination
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }

        private IQueryable<User> ApplySorting(IQueryable<User> query, string sortBy, string sortOrder)
        {
            var isDescending = sortOrder?.ToLower() == "desc";

            return sortBy?.ToLower() switch
            {
                "name" => isDescending ? query.OrderByDescending(u => u.Name) : query.OrderBy(u => u.Name),
                "email" => isDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                "type" => isDescending ? query.OrderByDescending(u => u.Type) : query.OrderBy(u => u.Type),
                "createdat" => isDescending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
                _ => query.OrderBy(u => u.Name) // Default to name ascending
            };
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
