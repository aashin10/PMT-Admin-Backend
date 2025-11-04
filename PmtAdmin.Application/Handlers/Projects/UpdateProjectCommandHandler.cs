using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PmtAdmin.Application.Command.Projects;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.Projects
{
    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ApiResponse<ProjectDTO>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        public UpdateProjectCommandHandler(IProjectRepository projectRepository, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ProjectDTO>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate required fields FIRST
                if (string.IsNullOrWhiteSpace(request.Name))
                    return ApiResponse<ProjectDTO>.Fail("Project name is required.");
                if (string.IsNullOrWhiteSpace(request.Key))
                    return ApiResponse<ProjectDTO>.Fail("Project key is required.");
                if (!request.StatusId.HasValue || request.StatusId <= 0)
                    return ApiResponse<ProjectDTO>.Fail("Valid status is required.");
                if (!request.DeliveryUnitId.HasValue || request.DeliveryUnitId <= 0)
                    return ApiResponse<ProjectDTO>.Fail("Valid delivery unit is required.");

                // Validate custom fields if provided
                if (request.CustomFields != null)
                {
                    foreach (var cf in request.CustomFields)
                    {
                        if (string.IsNullOrWhiteSpace(cf.Name))
                            return ApiResponse<ProjectDTO>.Fail("Custom field name cannot be empty.");
                        if (string.IsNullOrWhiteSpace(cf.Value))
                            return ApiResponse<ProjectDTO>.Fail("Custom field value cannot be empty.");
                    }
                }

                // Load project WITHOUT including custom fields to avoid tracking conflicts
                var project = await _projectRepository.GetQueryable()
                    .FirstOrDefaultAsync(p => p.Id == request.Id && p.DeletedAt == null);
                
                if (project == null)
                    return ApiResponse<ProjectDTO>.NotFound("Project not found");

                // Update all project fields
                project.Name = request.Name;
                project.Key = request.Key;
                project.Description = request.Description;
                project.CustomerOrgName = request.CustomerOrgName;
                project.CustomerDomainUrl = request.CustomerDomainUrl;
                project.CustomerDescription = request.CustomerDescription;
                project.PocEmail = request.PocEmail;
                project.PocPhone = request.PocPhone;
                project.ProjectManagerId = request.ProjectManagerId;
                project.ProjectManagerRoleId = request.ProjectManagerRoleId;
                project.StatusId = request.StatusId;
                project.DeliveryUnitId = request.DeliveryUnitId;
                project.TemplateId = request.TemplateId;
                project.UpdatedAt = DateTime.UtcNow;

                // Validate and update Metadata
                if (!string.IsNullOrWhiteSpace(request.Metadata))
                {
                    try
                    {
                        JsonDocument.Parse(request.Metadata);
                        project.Metadata = request.Metadata;
                    }
                    catch (JsonException)
                    {
                        return ApiResponse<ProjectDTO>.Fail("Metadata must be a valid JSON string.");
                    }
                }
                else if (request.Metadata == string.Empty)
                {
                    project.Metadata = null;
                }

                // Save project changes
                await _projectRepository.SaveChangesAsync();

                // Fetch fresh data for response  
                var updatedProject = await _projectRepository.GetProjectByIdWithDetailsAsync(request.Id);
                var projectDto = _mapper.Map<ProjectDTO>(updatedProject);

                return ApiResponse<ProjectDTO>.Success(projectDto, "Project updated successfully");
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                return ApiResponse<ProjectDTO>.Fail($"Error updating project: {message}");
            }
        }
    }
}
