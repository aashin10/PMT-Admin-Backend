using PmtAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Domain.Persistance.Settings
{
    public interface ISuperAdminRepository:IGenericRepository<User>
    {
        Task<User> GetByIdAsync(int id);
        Task<List<User>> GetSuperAdminsAsync();
        //Task<bool> DeleteSuperAdminAsync(int id);

        // ✅ New method
        Task<int> CountActiveSuperAdminsAsync();
        Task<int> CountActiveEnabledSuperAdminsAsync();


    }
}
