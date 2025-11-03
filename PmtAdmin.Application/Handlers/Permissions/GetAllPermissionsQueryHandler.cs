using AutoMapper;
using MediatR;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Query.Permissions;
using PmtAdmin.Domain.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.Permissions
{
    public class GetAllPermissionsQueryHandler : IRequestHandler<GetAllPermissionsQuery, List<PermissionDto>>
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;

        public GetAllPermissionsQueryHandler(IPermissionRepository permissionRepository, IMapper mapper)
        {
            _permissionRepository = permissionRepository;
            _mapper = mapper;
        }

        public async Task<List<PermissionDto>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissions = await _permissionRepository.GetAllAsync();
            return _mapper.Map<List<PermissionDto>>(permissions);
        }
    }
}

