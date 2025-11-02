using Microsoft.EntityFrameworkCore;
using PmtAdmin.Application.Dto;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using PmtAdmin.Infrastructure.Context;

namespace PmtAdmin.Infrastructure.Repositories
{
    public class DeliveryUnitRepository : GenericRepository<DeliveryUnit>, IDuRepository
    {
        private readonly AppDbContext _context;

        public DeliveryUnitRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<DeliveryUnit>> GetAllWithProjectCountAsync(CancellationToken cancellationToken)
        {

            return await _context.DeliveryUnits
                    .Include(du => du.Projects) // Load related projects
                    .ThenInclude(p => p.Status) // Load Status for filtering
                    .ToListAsync(cancellationToken);

        }
    

    //public async Task<DeliveryUnit> CreateNewDu(DeliveryUnit du, CancellationToken cancellationToken)
    //    {
    //        await _context.DeliveryUnits.AddAsync(du, cancellationToken);
    //        await _context.SaveChangesAsync(cancellationToken);
    //        return du;
    //    }

        public async Task<DeliveryUnit> CreateNewDu(DeliveryUnit du,CancellationToken cancellationToken)
        {
            await _context.DeliveryUnits.AddAsync(du, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return du;
        }
        public async Task<DeliveryUnit?> GetDuById(int id, CancellationToken cancellationToken)
        {
            return await _context.DeliveryUnits
                .Include(d => d.Projects)
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        }
        public async Task<DeliveryUnit?> UpdateDuAsync(DeliveryUnit du, CancellationToken cancellationToken)
        {
            var existingDu = await _context.DeliveryUnits.FirstOrDefaultAsync(x => x.Id == du.Id, cancellationToken);
            if (existingDu == null)
                return null;

            // Prevent DU code changes
            du.Code = existingDu.Code;

            _context.Entry(existingDu).CurrentValues.SetValues(du);
            await _context.SaveChangesAsync(cancellationToken);

            return existingDu;
        }
        public async Task<bool> DeleteDuAsync(DeliveryUnit du, CancellationToken cancellationToken)
        {
            //// Soft delete (recommended)
            //du.IsActive = false;
            //du.UpdatedAt = DateTime.UtcNow;

            //_context.DeliveryUnits.Update(du);
            //await _context.SaveChangesAsync(cancellationToken);
            //return true;

            //For hard delete:
            _context.DeliveryUnits.Remove(du);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}