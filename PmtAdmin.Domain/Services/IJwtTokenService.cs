using PmtAdmin.Domain.Entities;

namespace BACKEND_CQRS.Domain.Services
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        int? ValidateToken(string token);
    }
}
