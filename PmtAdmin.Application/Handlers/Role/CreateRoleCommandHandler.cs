using AutoMapper;
using MediatR;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using RoleManagement.Application.Commands;
using RoleManagement.Application.Queries;
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
                // Step 1: Create the role entity
                var role = new PmtAdmin.Domain.Entities.Role
                {
                    Name = request.Name,
                    Description = request.Description,
                    CreatedAt = DateTime.UtcNow
                };

                // Step 2: Save role first (so it has an Id)
                var createdRole = await _rolesRepository.CreateAsync(role);

                // Step 3: If permissions were sent, fetch and assign them
                if (request.PermissionIds != null && request.PermissionIds.Any())
                {
                    var permissions = await _permissionRepository.GetByIdAsync(request.PermissionIds);

                    createdRole.RolePermissions = permissions.Select(p => new RolePermission
                    {
                        RoleId = createdRole.Id,
                        PermissionId = p.Id
                    }).ToList();

                    await _rolesRepository.UpdateAsync(createdRole);
                }

                // Step 4: Map to DTO (after permissions are attached)
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
