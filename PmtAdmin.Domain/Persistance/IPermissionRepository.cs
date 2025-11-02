using PmtAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Domain.Persistance
{
    public interface IPermissionRepository
    {
        Task<List<Permission>> GetAllAsync();
        Task<Permission?> GetByIdAsync(int id);
        Task<List<Permission>> GetByIdAsync(IEnumerable<int> ids); // ✅ Add this line
        Task<Permission> CreateAsync(Permission permission);
    }
}
