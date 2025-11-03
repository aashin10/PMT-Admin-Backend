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
                else if (request.Metadata == string.Empty)
                {
                    project.Metadata = null;
                }

                // Handle custom fields - only update/add, never delete existing ones
                if (request.CustomFields != null)
                {
                    // Special case: if empty array is provided, clear all fields
                    if (!request.CustomFields.Any())
                    {
                        project.CustomFields.Clear();
                    }
                    else
                    {
                        // Update existing fields and add new ones, but preserve unlisted fields
                        foreach (var requestField in request.CustomFields)
                        {
                            if (requestField.Id != Guid.Empty)
                            {
                                // Update existing field
                                var existingField = project.CustomFields.FirstOrDefault(f => f.Id == requestField.Id);
                                if (existingField != null)
                                {
                                    existingField.Name = requestField.Name;
                                    existingField.Value = requestField.Value;
                                }
                                else
                                {
                                    // Field with this ID doesn't exist, create it
                                    var newField = new CustomField
                                    {
                                        Id = requestField.Id,
                                        ProjectId = project.Id,
                                        Name = requestField.Name,
                                        Value = requestField.Value
                                    };
                                    project.CustomFields.Add(newField);
                                }
                            }
                            else
                            {
                                // Create new field
                                var newField = new CustomField
                                {
                                    Id = Guid.NewGuid(),
                                    ProjectId = project.Id,
                                    Name = requestField.Name,
                                    Value = requestField.Value
                                };
                                project.CustomFields.Add(newField);
                            }
                        }
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
