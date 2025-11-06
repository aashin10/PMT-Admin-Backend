using System.Text.Json;
using MediatR;
using AutoMapper;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Application.Constants;
using PmtAdmin.Domain.Persistance;
using PmtAdmin.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.Role
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, ApiResponse<RoleDto>>
    {
        private readonly IRoleRepository _rolesRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;

        public UpdateRoleCommandHandler(IRoleRepository rolesRepository, IPermissionRepository permissionRepository, IMapper mapper)
        {
            _rolesRepository = rolesRepository;
            _permissionRepository = permissionRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<RoleDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Get the existing role with its permissions (tracked by EF)
                var role = await _rolesRepository.GetById(request.Id);
                if (role == null)
                    return ApiResponse<RoleDto>.NotFound("Role not found.");

                // Step 2: Update basic fields
                role.Name = request.Name;
                role.Description = request.Description;
                role.UpdatedAt = DateTime.UtcNow;

                // Validate and sanitize Metadata
                if (!string.IsNullOrWhiteSpace(request.Metadata))
                {
                    try
                    {
                        JsonDocument.Parse(request.Metadata); // Throws if invalid
                        role.Metadata = request.Metadata;
                    }
                    catch (JsonException)
                    {
                        return ApiResponse<RoleDto>.Fail("Metadata must be a valid JSON string.");
                    }
                }
                else
                {
                    role.Metadata = null;
                }

                // Step 3: Update permissions (if provided)
                if (request.PermissionIds != null)
                {
                    // Validate permission IDs exist
                    var permissions = await _permissionRepository.GetByIdAsync(request.PermissionIds);
                    if (permissions.Count != request.PermissionIds.Count)
                        return ApiResponse<RoleDto>.Fail("One or more permission IDs are invalid.");

                    await _rolesRepository.UpdateRolePermissionsAsync(role, request.PermissionIds);
                }

                // Step 4: Save changes
                await _rolesRepository.UpdateAsync(role);

                // Step 5: Fetch updated role with all related data
                var updatedRole = await _rolesRepository.GetById(request.Id);
                var roleDto = _mapper.Map<RoleDto>(updatedRole);
                
                return ApiResponse<RoleDto>.Success(roleDto, "Role updated successfully");
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                return ApiResponse<RoleDto>.Fail($"Error updating role: {message}");
            }
        }
    }
}