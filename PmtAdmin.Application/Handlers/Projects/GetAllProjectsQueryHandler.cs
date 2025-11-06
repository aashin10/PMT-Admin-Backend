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
    public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, ApiResponse<PaginatedResponse<ProjectTableDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IProjectRepository _projectRepository;

        public GetAllProjectsQueryHandler(IMapper mapper, IProjectRepository projectRepository)
        {
            _mapper = mapper;
            _projectRepository = projectRepository;
        }

        public async Task<ApiResponse<PaginatedResponse<ProjectTableDTO>>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate pagination parameters
                if (request.Page < 1)
                    request.Page = 1;

                if (request.PageSize < 1)
                    request.PageSize = 10;

                // Limit page size to prevent performance issues (10, 25, 50, max 100)
                if (request.PageSize > 100)
                    request.PageSize = 100;

                // Get paginated and filtered projects
                var (projects, totalCount) = await _projectRepository.GetProjectsForTableAsync(
                    request.Page,
                    request.PageSize,
                    request.SearchTerm,
                    request.StatusIds,
                    request.DeliveryUnitIds,
                    request.ProjectManagerIds
                );

                if (projects == null || !projects.Any())
                {
                    var emptyResponse = new PaginatedResponse<ProjectTableDTO>
                    {
                        Items = new List<ProjectTableDTO>(),
                        TotalCount = 0,
                        Page = request.Page,
                        PageSize = request.PageSize,
                        TotalPages = 0
                    };
                    return ApiResponse<PaginatedResponse<ProjectTableDTO>>.Success(emptyResponse, "No projects found");
                }

                // Map to simplified table DTOs with safe null checking
                var projectTableDtos = projects.Select(p => new ProjectTableDTO
                {
                    Id = p.Id,
                    Name = p.Name ?? string.Empty,
                    Key = p.Key ?? string.Empty,
                    Status = p.Status != null ? new ProjectStatusDto
                    {
                        Id = p.Status.Id,
                        Name = p.Status.Name ?? string.Empty,
                        Description = p.Status.Description
                    } : null,
                    DeliveryUnit = p.DeliveryUnit != null ? new DeliveryUnitDto
                    {
                        Id = p.DeliveryUnit.Id,
                        Name = p.DeliveryUnit.Name ?? string.Empty,
                        Code = p.DeliveryUnit.Code ?? string.Empty
                    } : null,
                    TeamSize = p.ProjectMembers?.Count ?? 0,
                    ProjectManager = p.ProjectManager != null ? new ProjectManagerDto
                    {
                        Id = p.ProjectManager.Id,
                        Name = p.ProjectManager.Name ?? string.Empty
                    } : null,
                    IsImportedFromJira = p.IsImportedFromJira
                }).ToList();

                // Calculate total pages
                var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

                var paginatedResponse = new PaginatedResponse<ProjectTableDTO>
                {
                    Items = projectTableDtos,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalPages = totalPages
                };

                return ApiResponse<PaginatedResponse<ProjectTableDTO>>.Success(
                    paginatedResponse,
                    $"Retrieved {projectTableDtos.Count} projects from {totalCount} total"
                );
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving projects: {ex.Message}", ex);
            }
        }
    }
}