using PmtAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Domain.Persistance
{
    public interface IUserRepository : IGenericRepository<Users>
    {
        Task<Users?> GetByEmailAsync(string email);
        Task<Users?> GetByJiraIdAsync(string jiraId);
        Task<IReadOnlyList<Users>> GetActiveUsersAsync();
        Task<IReadOnlyList<Users>> GetAllNonDeletedUsersAsync();
        Task DeleteUsersByIdsAsync(IEnumerable<int> ids);
    }
}
