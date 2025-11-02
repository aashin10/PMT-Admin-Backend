using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Query;
using PmtAdmin.Domain.Persistance;
using System.Threading;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.Role
{
    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleDto>
    {
        private readonly IRoleRepository _rolesRepository;
        private readonly IMapper _mapper;

        public GetRoleByIdQueryHandler(IRoleRepository rolesRepository, IMapper mapper)
        {
            _rolesRepository = rolesRepository;
            _mapper = mapper;
        }

        public async Task<RoleDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await _rolesRepository.GetById(request.Id);

            if (role == null)
                return null;

            // Use AutoMapper to map Role to RoleDto
            return _mapper.Map<RoleDto>(role);
        }
    }
}