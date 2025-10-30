using MediatR;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Constants;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Persistance;
using RoleManagement.Application.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.Role
{
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, ApiResponse<string>>
    {
        private readonly IRoleRepository _rolesRepository;

        public DeleteRoleCommandHandler(IRoleRepository rolesRepository)
        {
            _rolesRepository = rolesRepository;
        }

        public async Task<ApiResponse<string>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if role exists
                var existingRole = await _rolesRepository.GetById(request.Id);
                if (existingRole == null)
                {
                    return ApiResponse<string>.Fail("Role not found");
                }

                // Delete the role
                await _rolesRepository.DeleteAsync(existingRole);

                return ApiResponse<string>.Success("Role deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Fail($"Error deleting role: {ex.Message}");
            }
        }
    }
}