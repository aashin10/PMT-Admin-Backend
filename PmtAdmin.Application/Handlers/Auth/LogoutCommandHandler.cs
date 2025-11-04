using BACKEND_CQRS.Application.Command;
using BACKEND_CQRS.Domain.Persistance;
using MediatR;
using Microsoft.Extensions.Logging;
using PmtAdmin.Application.Wrappers;

namespace BACKEND_CQRS.Application.Handler.Auth
{
    // Fix for CS0311: Ensure LogoutCommand implements IRequest<ApiResponse<bool>>
    // Fix for CS0452: Change ApiResponse<bool> to ApiResponse<object> since T must be a reference type
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ApiResponse<object>>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger<LogoutCommandHandler> _logger;

        public LogoutCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            ILogger<LogoutCommandHandler> logger)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<object>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Logout request for user: {UserId}", request.UserId);

                // Revoke all refresh tokens for the user
                await _refreshTokenRepository.RevokeAllUserTokensAsync(request.UserId);

                _logger.LogInformation("Logout successful for user: {UserId}", request.UserId);

                return ApiResponse<object>.Success(null, "Logout successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for user: {UserId}", request.UserId);
                return ApiResponse<object>.Fail("An error occurred during logout");
            }
        }
    }
}
