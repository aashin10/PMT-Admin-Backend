using AutoMapper;
using MediatR;
using PmtAdmin.Application.Command.Projects;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using System;
using System.Collections.Generic;
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
                // Basic validation
                if (string.IsNullOrWhiteSpace(request.Name))
                    return ApiResponse<ProjectDTO>.Fail("Project name is required.");
                if (string.IsNullOrWhiteSpace(request.Key))
                    return ApiResponse<ProjectDTO>.Fail("Project key is required.");
                if (!request.StatusId.HasValue || request.StatusId <= 0)
                    return ApiResponse<ProjectDTO>.Fail("Valid status is required.");
                if (!request.DeliveryUnitId.HasValue || request.DeliveryUnitId <= 0)
                    return ApiResponse<ProjectDTO>.Fail("Valid delivery unit is required.");

                // Load project with details (includes CustomFields) using repository abstraction
                var project = await _projectRepository.GetProjectByIdWithDetailsAsync(request.Id);
                if (project == null)
                    return ApiResponse<ProjectDTO>.NotFound("Project not found");

                // Update scalar fields
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

                // Metadata handling
                if (!string.IsNullOrWhiteSpace(request.Metadata))
                {
                    try
                    {
                        JsonDocument.Parse(request.Metadata); // validate json
                        project.Metadata = request.Metadata;
                    }
                    catch (JsonException)
                    {
                        return ApiResponse<ProjectDTO>.Fail("Metadata must be a valid JSON string.");
                    }
                }
                else if (request.Metadata == string.Empty)
                {
                    // Explicit empty string means clear metadata
                    project.Metadata = null;
                }

                // Custom fields processing
                if (request.CustomFields != null)
                {
                    project.CustomFields ??= new List<CustomField>();

                    if (request.CustomFields.Count == 0)
                    {
                        // Clear all
                        project.CustomFields.Clear();
                    }
                    else
                    {
                        // Update existing and add new
                        foreach (var cfDto in request.CustomFields)
                        {
                            if (string.IsNullOrWhiteSpace(cfDto.Name))
                                return ApiResponse<ProjectDTO>.Fail("Custom field name cannot be empty.");
                            if (string.IsNullOrWhiteSpace(cfDto.Value))
                                return ApiResponse<ProjectDTO>.Fail("Custom field value cannot be empty.");

                            if (cfDto.Id != Guid.Empty)
                            {
                                var existing = project.CustomFields.FirstOrDefault(f => f.Id == cfDto.Id);
                                if (existing != null)
                                {
                                    existing.Name = cfDto.Name;
                                    existing.Value = cfDto.Value;
                                }
                                else
                                {
                                    // Treat as new if not found
                                    project.CustomFields.Add(new CustomField
                                    {
                                        Id = cfDto.Id,
                                        Name = cfDto.Name,
                                        Value = cfDto.Value,
                                        ProjectId = project.Id
                                    });
                                }
                            }
                            else
                            {
                                // New field
                                project.CustomFields.Add(new CustomField
                                {
                                    Id = Guid.NewGuid(),
                                    Name = cfDto.Name,
                                    Value = cfDto.Value,
                                    ProjectId = project.Id
                                });
                            }
                        }
                    }
                }

                // Persist changes
                await _projectRepository.UpdateAsync(project);

                // Reload for response (with details)
                var updated = await _projectRepository.GetProjectByIdWithDetailsAsync(project.Id);
                var dto = _mapper.Map<ProjectDTO>(updated);
                return ApiResponse<ProjectDTO>.Success(dto, "Project updated successfully");
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                return ApiResponse<ProjectDTO>.Fail($"Error updating project: {message}");
            }
        }
    }
}
