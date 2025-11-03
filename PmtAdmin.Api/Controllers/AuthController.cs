using BACKEND_CQRS.Application.Command;
using BACKEND_CQRS.Application.Dto;
//using BACKEND_CQRS.Application.Wrapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PmtAdmin.Application.Wrappers;
using System.Security.Claims;

namespace BACKEND_CQRS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IMediator mediator, ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>Access token and refresh token</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ApiResponse<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            _logger.LogInformation("Login endpoint called for email: {Email}", request.Email);
            // request.Email = request.Email.Trim().ToLower();

            var command = new LoginCommand
            {
                Email = request.Email,
                Password = request.Password
            };

            var result = await _mediator.Send(command);
            return result;
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        /// <param name="request">Refresh token</param>
        /// <returns>New access token and refresh token</returns>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ApiResponse<LoginResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            _logger.LogInformation("Refresh token endpoint called");

            var command = new RefreshTokenCommand
            {
                RefreshToken = request.RefreshToken
            };

            var result = await _mediator.Send(command);
            return result;
        }

        /// <summary>
        /// Logout and revoke all refresh tokens
        /// </summary>
        /// <returns>Success status</returns>
        [HttpPost("logout")]
        [Authorize]
        public async Task<ApiResponse<object>> Logout()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogWarning("Logout failed: Invalid user ID in token");
                return ApiResponse<object>.Fail("Invalid user session");
            }

            _logger.LogInformation("Logout endpoint called for user: {UserId}", userId);

            var command = new LogoutCommand
            {
                UserId = userId
            };

            var result = await _mediator.Send(command);
            return result;
        }

        /// <summary>
        /// Get current authenticated user information
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            try
            {
                // Try multiple claim types for userId
                var userId = User.FindFirst("sub")?.Value
                          ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;

                var email = User.FindFirst("email")?.Value
                         ?? User.FindFirst(ClaimTypes.Email)?.Value;

                var name = User.FindFirst("name")?.Value
                        ?? User.FindFirst(ClaimTypes.Name)?.Value;

                var isSuperAdminClaim = User.FindFirst("is_super_admin")?.Value;
                var isActiveClaim = User.FindFirst("is_active")?.Value;

                _logger.LogInformation("GetCurrentUser called. UserId: {UserId}, Email: {Email}", userId, email);

                // Debug: Log ALL claims
                _logger.LogInformation("All claims in token: {Claims}",
                    string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}")));

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("GetCurrentUser failed: UserId claim not found in token");
                    _logger.LogWarning("Available claims: {ClaimTypes}",
                        string.Join(", ", User.Claims.Select(c => c.Type)));
                    return Unauthorized(ApiResponse<object>.Fail("Invalid user session"));
                }

                var userInfo = new UserInfoDto
                {
                    UserId = userId,
                    Email = email ?? "",
                    Name = name ?? "Unknown User",
                    IsSuperAdmin = !string.IsNullOrEmpty(isSuperAdminClaim) && bool.Parse(isSuperAdminClaim),
                    IsActive = !string.IsNullOrEmpty(isActiveClaim) && bool.Parse(isActiveClaim),
                    Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
                };

                _logger.LogInformation("User info retrieved successfully for UserId: {UserId}", userId);
                return Ok(ApiResponse<UserInfoDto>.Success(userInfo, "User information retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current user information");
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred while retrieving user information"));
            }
        }
    }
}