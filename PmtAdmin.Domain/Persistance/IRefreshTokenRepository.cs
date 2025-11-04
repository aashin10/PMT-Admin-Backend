using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;

namespace BACKEND_CQRS.Domain.Persistance
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<RefreshToken?> GetActiveByUserIdAsync(int userId);
        Task RevokeAllUserTokensAsync(int userId);
    }
}
