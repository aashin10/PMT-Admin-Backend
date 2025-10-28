using MediatR;
using AutoMapper;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Application.Constants;
using PmtAdmin.Domain.Persistance;
using PmtAdmin.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using RoleManagement.Application.Commands;
using RoleManagement.Application.Queries;

namespace PmtAdmin.Application.Handlers.Role
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, ApiResponse<RoleDto>>
    {
        private readonly IRoleRepository _rolesRepository;
        private readonly IMapper _mapper;

        public UpdateRoleCommandHandler(IRoleRepository rolesRepository, IMapper mapper)
        {
            _rolesRepository = rolesRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<RoleDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if role exists
                var existingRole = await _rolesRepository.GetById(request.Id);
                if (existingRole == null)
                {
                    return ApiResponse<RoleDto>.Fail("Role not found", StatusCode.NotFound);
                }

                // Update role properties
                existingRole.Name = request.Name;
                existingRole.Description = request.Description;
                existingRole.Metadata = request.Metadata;
                existingRole.UpdatedAt = DateTime.UtcNow;

                // Update the role
                var updatedRole = await _rolesRepository.UpdateAsync(existingRole);
                var roleDto = _mapper.Map<RoleDto>(updatedRole);

                return ApiResponse<RoleDto>.Success(roleDto, "Role updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<RoleDto>.Fail($"Error updating role: {ex.Message}", StatusCode.InternalServerCode);
            }
        }
    }
}