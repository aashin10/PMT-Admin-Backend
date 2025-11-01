using AutoMapper;
using MediatR;
using PmtAdmin.Application.Dto.SettingsDTO;
using PmtAdmin.Application.Query.Settings;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Persistance.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.Settings
{
    public class GetSuperAdminQueryHandler
        : IRequestHandler<GetSuperAdminQuery, ApiResponse<List<SuperAdminDto>>>
    {
        private readonly ISuperAdminRepository _superAdminRepository;
        private readonly IMapper _mapper;

        public GetSuperAdminQueryHandler(ISuperAdminRepository superAdminRepository, IMapper mapper)
        {
            _superAdminRepository = superAdminRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<SuperAdminDto>>> Handle(GetSuperAdminQuery request, CancellationToken cancellationToken)
        {
            // Get all users and filter only super admins
            var allUsers = await _superAdminRepository.GetAllAsync();

            // Filter users where IsSuperAdmin is true and user is not deleted
            var superAdmins = allUsers
                .Where(u => u.IsSuperAdmin && !u.IsDeleted)
                .ToList();

            // Pagination
            var paged = superAdmins
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var dtos = _mapper.Map<List<SuperAdminDto>>(paged);

            return ApiResponse<List<SuperAdminDto>>.Success(dtos);
        }
    }
}
