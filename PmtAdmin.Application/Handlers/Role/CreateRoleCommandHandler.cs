using AutoMapper;
using MediatR;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Application.Constants;
using PmtAdmin.Domain.Persistance;
using PmtAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoleManagement.Application.Commands;
using RoleManagement.Application.Queries;

namespace PmtAdmin.Application.Handlers.Role
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, ApiResponse<RoleDto>>
    {
        private readonly IRoleRepository _rolesRepository;
        private readonly IMapper _mapper;

        public CreateRoleCommandHandler(IRoleRepository rolesRepository, IMapper mapper)
        {
            _rolesRepository = rolesRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var role = new PmtAdmin.Domain.Entities.Role
                {
                    Name = request.Name,  // Changed from request.Name to request.RoleName
                    Description = request.Description,
                    CreatedAt = DateTime.UtcNow
                };

                var createdRole = await _rolesRepository.CreateAsync(role);
                var roleDto = _mapper.Map<RoleDto>(createdRole);

                return ApiResponse<RoleDto>.Created(roleDto, "Role created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<RoleDto>.Fail($"Error creating role: {ex.Message}", StatusCode.InternalServerCode);
            }
        }
    }
}