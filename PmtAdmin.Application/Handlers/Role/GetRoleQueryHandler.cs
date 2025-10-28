using MediatR;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Query;
using PmtAdmin.Domain.Persistance;
using RoleManagement.Application.Queries;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace PmtAdmin.Application.Handlers.Role
{
    public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, List<RoleDto>>
    {
        private readonly IRoleRepository _rolesRepository;

        public GetRolesQueryHandler(IRoleRepository rolesRepository)
        {
            _rolesRepository = rolesRepository;
        }

        public async Task<List<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _rolesRepository.GetAllAsync();

            var pagedRoles = roles
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(role => new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    Metadata = role.Metadata,
                    CreatedAt = role.CreatedAt.ToString("o"),
                    
                    // Map other properties as needed
                })
                .ToList();

            return pagedRoles;
        }
    }
}