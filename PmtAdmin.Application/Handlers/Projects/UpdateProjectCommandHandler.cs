using AutoMapper;
using MediatR;
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
                // Get the existing project with all details
                var project = await _projectRepository.GetProjectByIdWithDetailsAsync(request.Id);
                if (project == null)
                    return ApiResponse<ProjectDTO>.NotFound("Project not found");

                // Update basic project fields
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
                project.CreatedBy = request.CreatedBy;
                project.UpdatedAt = DateTime.UtcNow;
                project.TemplateId = request.TemplateId;

                // Validate and update Metadata
                if (!string.IsNullOrWhiteSpace(request.Metadata))
                {
                    try
                    {
                        JsonDocument.Parse(request.Metadata); // Validates JSON format
                        project.Metadata = request.Metadata;
                    }
                    catch (JsonException)
                    {
                        return ApiResponse<ProjectDTO>.Fail("Metadata must be a valid JSON string.");
                    }
                }
                else
                {
                    project.Metadata = null;
                }

                // Update custom fields if provided
                if (request.CustomFields != null)
                {
                    // Clear existing custom fields
                    project.CustomFields.Clear();

                    // Add new custom fields
                    foreach (var customFieldDto in request.CustomFields)
                    {
                        var customField = new CustomField
                        {
                            Id = customFieldDto.Id == Guid.Empty ? Guid.NewGuid() : customFieldDto.Id,
                            ProjectId = project.Id,
                            Name = customFieldDto.Name,
                            Value = customFieldDto.Value
                        };
                        project.CustomFields.Add(customField);
                    }
                }

                // Update the project
                await _projectRepository.UpdateAsync(project);

                // Fetch the updated project with all related data
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
