using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Domain.Persistance
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetById(int id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
