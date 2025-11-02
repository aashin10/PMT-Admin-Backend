using AutoMapper;
using MediatR;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Query.Projects;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.Projects
{
    public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ApiResponse<ProjectDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IProjectRepository _projectRepository;

        public GetProjectByIdQueryHandler(IMapper mapper, IProjectRepository projectRepository)
        {
            _mapper = mapper;
            _projectRepository = projectRepository;
        }

        public async Task<ApiResponse<ProjectDTO>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetProjectByIdWithDetailsAsync(request.Id);

            if (project == null)
            {
                return ApiResponse<ProjectDTO>.NotFound("Project not found");
            }

            var projectDto = new ProjectDTO
            {
                Id = project.Id,
                Name = project.Name,
                Key = project.Key,
                Description = project.Description,
                CustomerOrgName = project.CustomerOrgName,
                CustomerDomainUrl = project.CustomerDomainUrl,
                CustomerDescription = project.CustomerDescription,
                PocEmail = project.PocEmail,
                PocPhone = project.PocPhone,
                ProjectManagerId = project.ProjectManagerId,
                ProjectManagerName = project.ProjectManager?.Name,
                ProjectManagerRoleId = project.ProjectManagerRoleId,
                StatusId = project.StatusId,
                StatusName = project.Status?.Name,
                DeliveryUnitId = project.DeliveryUnitId,
                DeliveryUnitName = project.DeliveryUnit?.Name,
                DeliveryUnitCode = project.DeliveryUnit?.Code,
                TeamSize = project.ProjectMembers?.Count ?? 0,
                SprintCount = project.Sprints?.Count ?? 0,
                AdditionalInformation = project.CustomFields?.Select(cf => new CustomFieldDTO
                {
                    Id = cf.Id,
                    Name = cf.Name,
                    Value = cf.Value
                }).ToList() ?? new List<CustomFieldDTO>(),
                Teams = project.Teams?.Select(t => new TeamDTO
                {
                    Id = t.Id,
                    Name = t.Name
                }).ToList() ?? new List<TeamDTO>(),
                TeamMembers = project.ProjectMembers?.Select(pm => new TeamMemberDTO
                {
                    Id = pm.Id,
                    Name = pm.User?.Name,
                    //Role = pm.ProjectRole,
                    //Email = pm.User?.Email,
                    //Team = pm.TeamId?.ToString()
                }).ToList() ?? new List<TeamMemberDTO>(),
                IsImportedFromJira = project.IsImportedFromJira,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt
            };

            return ApiResponse<ProjectDTO>.Success(projectDto);
        }
    }
}