using PmtAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Domain.Persistance
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByJiraIdAsync(string jiraId);
        Task<IReadOnlyList<User>> GetActiveUsersAsync();
        Task<IReadOnlyList<User>> GetAllNonDeletedUsersAsync();
        Task<IReadOnlyList<User>> GetFilteredUsersAsync(string? type, string? status);
        Task DeleteUsersByIdsAsync(IEnumerable<int> ids);
    }
}
