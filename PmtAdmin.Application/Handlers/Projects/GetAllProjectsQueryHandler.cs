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
    public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, ApiResponse<List<ProjectDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IProjectRepository _projectRepository;

        public GetAllProjectsQueryHandler(IMapper mapper, IProjectRepository projectRepository)
        {
            _mapper = mapper;
            _projectRepository = projectRepository;
        }

        public async Task<ApiResponse<List<ProjectDTO>>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
        {
            var projects = await _projectRepository.GetAllProjectsWithDetailsAsync();

            if (projects == null || !projects.Any())
            {
                return ApiResponse<List<ProjectDTO>>.Success(new List<ProjectDTO>(), "No projects found");
            }

            var projectDtos = projects.Select(p => new ProjectDTO
            {
                Id = p.Id,
                Name = p.Name,
                Key = p.Key,
                Description = p.Description,
                CustomerOrgName = p.CustomerOrgName,
                CustomerDomainUrl = p.CustomerDomainUrl,
                CustomerDescription = p.CustomerDescription,
                PocEmail = p.PocEmail,
                PocPhone = p.PocPhone,
                ProjectManagerId = p.ProjectManagerId,
                ProjectManagerName = p.ProjectManager?.Name,
                ProjectManagerRoleId = p.ProjectManagerRoleId,
                StatusId = p.StatusId,
                StatusName = p.Status?.Name,
                DeliveryUnitId = p.DeliveryUnitId,
                DeliveryUnitName = p.DeliveryUnit?.Description,
                IsImportedFromJira = p.IsImportedFromJira,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList();

            return ApiResponse<List<ProjectDTO>>.Success(projectDtos);
        }
    }
}
