using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PmtAdmin.Application.Command.Settings;
using PmtAdmin.Application.Dto.SettingsDTO;
using PmtAdmin.Application.Query.Settings;
using PmtAdmin.Application.Wrappers;

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
            // Set the creator ID from the authenticated user
            command.CreatedBy = GetCurrentUserId(); // We'll implement this helper method
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
            command.Id = id; // ensure ID from route is used
            command.UpdatedBy = GetCurrentUserId(); // Track who updated
            var updated = await _mediator.Send(command);
            return updated;
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse<SuperAdminDto>> DeleteSuperAdmin(int id)
        {
            var result = await _mediator.Send(new DeleteSuperAdminCommand 
            { 
                Id = id,
                DeletedBy = GetCurrentUserId() // Track who deleted
            });
            return result;
        }

        // Helper method to get current user ID - in a real application, this would get it from claims
        private int GetCurrentUserId()
        {
            // TODO: Implement proper user authentication and get the real user ID
            // For now, returning a dummy ID 1 (typically would come from JWT token or claims)
            return 1;
        }
    }
}
