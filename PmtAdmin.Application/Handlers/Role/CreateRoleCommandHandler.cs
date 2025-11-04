using AutoMapper;
using MediatR;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.Role
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, ApiResponse<RoleDto>>
    {
        private readonly IRoleRepository _rolesRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;

        public CreateRoleCommandHandler(
            IRoleRepository rolesRepository,
            IPermissionRepository permissionRepository,
            IMapper mapper)
        {
            _rolesRepository = rolesRepository;
            _permissionRepository = permissionRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Check for unique role name
                var existingRoles = await _rolesRepository.GetAllAsync();
                if (existingRoles.Any(r => r.Name != null && r.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    return ApiResponse<RoleDto>.Fail("Role name already exists.");
                }

                // 2. Check that permissions are assigned
                if (request.PermissionIds == null || !request.PermissionIds.Any())
                {
                    return ApiResponse<RoleDto>.Fail("At least one permission must be assigned to the role.");
                }

                // 3. Create the role
                var role = new PmtAdmin.Domain.Entities.Role
                {
                    Name = request.Name,
                    Description = request.Description,
                    Metadata = request.Metadata,
                    CreatedAt = DateTime.UtcNow
                };

                var createdRole = await _rolesRepository.CreateAsync(role);

                // 4. Assign permissions
                var permissions = await _permissionRepository.GetByIdAsync(request.PermissionIds);
                if (permissions == null || permissions.Count != request.PermissionIds.Count)
                {
                    return ApiResponse<RoleDto>.Fail("One or more permissions are invalid.");
                }

                await _rolesRepository.UpdateRolePermissionsAsync(createdRole, request.PermissionIds);

                var roleDto = _mapper.Map<RoleDto>(createdRole);
                return ApiResponse<RoleDto>.Created(roleDto, "Role created successfully");
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                return ApiResponse<RoleDto>.Fail($"Error creating role: {message}");
            }
        }
    }
}
