using AutoMapper;
using MediatR;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Query.Status;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.ProjectStatus
{
    public class GetAllProjectStatusQueryHandler : IRequestHandler<GetAllProjectStatusQuery, ApiResponse<List<ProjectStatusDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IProjectStatusRepository _projectStatusRepository;

        public GetAllProjectStatusQueryHandler(IMapper mapper, IProjectStatusRepository projectStatusRepository)
        {
            _mapper = mapper;
            _projectStatusRepository = projectStatusRepository;
        }

        public async Task<ApiResponse<List<ProjectStatusDto>>> Handle(GetAllProjectStatusQuery request, CancellationToken cancellationToken)
        {
            var statuses = await _projectStatusRepository.GetAllAsync();

            if (statuses == null || !statuses.Any())
            {
                return ApiResponse<List<ProjectStatusDto>>.Success(new List<ProjectStatusDto>(), "No project statuses found");
            }

            var statusDtos = _mapper.Map<List<ProjectStatusDto>>(statuses);

            return ApiResponse<List<ProjectStatusDto>>.Success(statusDtos);
        }
    }
}
