using MediatR;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Query;
using PmtAdmin.Domain.Persistance;
using Microsoft.EntityFrameworkCore;
using RoleManagement.Application.Queries;

using System.Threading;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.Role
{
    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleDto>
    {
        private readonly IRoleRepository _rolesRepository;

        public GetRoleByIdQueryHandler(IRoleRepository rolesRepository)
        {
            _rolesRepository = rolesRepository;
        }

        public async Task<RoleDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            // You may need to implement this method in your repository
            var role = await _rolesRepository.GetById(request.Id);

            if (role == null)
                return null;

            // Map Role entity to RoleDto
            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Metadata = role.Metadata,
                CreatedAt = role.CreatedAt.ToString("o"), // ISO 8601 format
               
                // Map other properties as needed
            };
        }
    }
}