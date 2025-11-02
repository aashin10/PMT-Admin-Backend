//using PmtAdmin.Application.Dto;
using PmtAdmin.Domain.Entities;

namespace PmtAdmin.Domain.Persistance
{
    public interface IDuRepository : IGenericRepository<DeliveryUnit>
    {
        Task<List<DeliveryUnit>> GetAllWithProjectCountAsync(CancellationToken cancellationToken);
        Task<DeliveryUnit> CreateNewDu(DeliveryUnit du,CancellationToken cancellationToken);

        Task<DeliveryUnit?> GetDuById(int id, CancellationToken cancellationToken);

        Task<DeliveryUnit?> UpdateDuAsync(DeliveryUnit du, CancellationToken cancellationToken);

        Task<bool> DeleteDuAsync(DeliveryUnit du, CancellationToken cancellationToken);

    }
}