using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PmtAdmin.Application.Command.Settings;
using PmtAdmin.Application.CustomException;
using PmtAdmin.Application.Dto.SettingsDTO;
using PmtAdmin.Application.Query.Settings;
using PmtAdmin.Application.Wrappers;
using System.Security.Claims;

namespace PmtAdmin.Api.Controllers.Settings
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SuperAdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SuperAdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ApiResponse<SuperAdminDto>> CreateSuperAdmin([FromBody] CreateSuperAdminCommand command)
        {
            command.CreatedBy = GetCurrentUserId();
            var newSuperAdmin = await _mediator.Send(command);
            return newSuperAdmin;
        }

        [HttpGet]
        public async Task<ApiResponse<List<SuperAdminDto>>> GetSuperAdmins()
        {
            var superAdmins = await _mediator.Send(new GetSuperAdminQuery());
            return superAdmins;
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<SuperAdminDto>> UpdateSuperAdmin(int id, [FromBody] UpdateSuperAdminCommand command)
        {
            command.Id = id;
            command.UpdatedBy = GetCurrentUserId();
            var updated = await _mediator.Send(command);
            return updated;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<SuperAdminDto>>> DeleteSuperAdmin(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                // Check if user is trying to delete themselves
                if (id == currentUserId)
                {
                    return BadRequest(ApiResponse<SuperAdminDto>.Fail("Self-deletion is not allowed. You cannot delete your own account."));
                }

                var result = await _mediator.Send(new DeleteSuperAdminCommand
                {
                    Id = id,
                    DeletedBy = currentUserId
                });
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<SuperAdminDto>.NotFound(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<SuperAdminDto>.Fail($"An error occurred: {ex.Message}"));
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID claim not found in token");
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID format in token");
            }

            return userId;
        }
    }
}










//using MediatR;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using PmtAdmin.Application.Command.Settings;
//using PmtAdmin.Application.Dto.SettingsDTO;
//using PmtAdmin.Application.Query.Settings;
//using PmtAdmin.Application.Wrappers;
//using System.Security.Claims;

//namespace PmtAdmin.Api.Controllers.Settings
//{
//    [Authorize]
//    [ApiController]
//    [Route("api/[controller]")]
//    public class SuperAdminController : ControllerBase
//    {
//        private readonly IMediator _mediator;

//        public SuperAdminController(IMediator mediator)
//        {
//            _mediator = mediator;
//        }

//        [HttpPost]
//        public async Task<ApiResponse<SuperAdminDto>> CreateSuperAdmin([FromBody] CreateSuperAdminCommand command)
//        {
//            // Set the creator ID from the authenticated user
//            command.CreatedBy = GetCurrentUserId(); // We'll implement this helper method
//            var newSuperAdmin = await _mediator.Send(command);
//            return newSuperAdmin;
//        }

//        [HttpGet]
//        public async Task<ApiResponse<List<SuperAdminDto>>> GetSuperAdmins()
//        {
//            var superAdmins = await _mediator.Send(new GetSuperAdminQuery());
//            return superAdmins;
//        }

//        [HttpPut("{id}")]
//        public async Task<ApiResponse<SuperAdminDto>> UpdateSuperAdmin(int id, [FromBody] UpdateSuperAdminCommand command)
//        {
//            command.Id = id; // ensure ID from route is used
//            command.UpdatedBy = GetCurrentUserId(); // Track who updated
//            var updated = await _mediator.Send(command);
//            return updated;
//        }

//        [HttpDelete("{id}")]
//        public async Task<ApiResponse<SuperAdminDto>> DeleteSuperAdmin(int id)
//        {
//            var result = await _mediator.Send(new DeleteSuperAdminCommand 
//            { 
//                Id = id,
//                DeletedBy = GetCurrentUserId() // Track who deleted
//            });
//            return result;
//        }

//        // Helper method to get current user ID - in a real application, this would get it from claims
//        //private int GetCurrentUserId()
//        //{
//        //    // TODO: Implement proper user authentication and get the real user ID
//        //    // For now, returning a dummy ID 1 (typically would come from JWT token or claims)
//        //    return 1;
//        //}
//        private int GetCurrentUserId()
//        {
//            // Get the user ID from the "nameid" claim (ClaimTypes.NameIdentifier)
//            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

//            if (string.IsNullOrEmpty(userIdClaim))
//            {
//                throw new UnauthorizedAccessException("User ID claim not found in token");
//            }

//            if (!int.TryParse(userIdClaim, out int userId))
//            {
//                throw new UnauthorizedAccessException("Invalid user ID format in token");
//            }

//            return userId;
//        }
//    }
//}
