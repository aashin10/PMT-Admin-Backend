using AutoMapper;
using MediatR;
using PmtAdmin.Application.Command.Projects;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.Projects
{
    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ApiResponse<ProjectDTO>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IBoardRepository _boardRepository;
        private readonly IMapper _mapper;

        public CreateProjectCommandHandler(
            IProjectRepository projectRepository,
            IBoardRepository boardRepository,
            IMapper mapper)
        {
            _projectRepository = projectRepository;
            _boardRepository = boardRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ProjectDTO>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate and process Metadata if provided
                if (!string.IsNullOrWhiteSpace(request.Metadata))
                {
                    try
                    {
                        JsonDocument.Parse(request.Metadata); // Validates JSON format
                    }
                    catch (JsonException)
                    {
                        return ApiResponse<ProjectDTO>.Fail("Metadata must be a valid JSON string.");
                    }
                }

                // Create the project entity
                var project = new Project
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Key = request.Key,
                    Description = request.Description,
                    CustomerOrgName = request.CustomerOrgName,
                    CustomerDomainUrl = request.CustomerDomainUrl,
                    CustomerDescription = request.CustomerDescription,
                    PocEmail = request.PocEmail,
                    PocPhone = request.PocPhone,
                    ProjectManagerId = request.ProjectManagerId,
                    ProjectManagerRoleId = request.ProjectManagerRoleId,
                    StatusId = request.StatusId,
                    DeliveryUnitId = request.DeliveryUnitId,
                    CreatedBy = request.CreatedBy,
                    CreatedAt = DateTime.UtcNow,
                    TemplateId = request.TemplateId,
                    Metadata = request.Metadata,
                    IsImportedFromJira = false
                };

                // Create the project
                var createdProject = await _projectRepository.CreateAsync(project);

                // Create default board with columns using project name
                await CreateDefaultBoardAsync(createdProject.Id, createdProject.Name, request.CreatedBy);

                // Fetch the created project with all related data for response
                var projectWithDetails = await _projectRepository.GetProjectByIdWithDetailsAsync(createdProject.Id);
                var projectDto = _mapper.Map<ProjectDTO>(projectWithDetails);

                return ApiResponse<ProjectDTO>.Created(projectDto, "Project created successfully with default board");
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                return ApiResponse<ProjectDTO>.Fail($"Error creating project: {message}");
            }
        }

        private async Task CreateDefaultBoardAsync(Guid projectId, string projectName, int? createdBy)
        {
            // Create default board with ProjectName-Board format
            var board = new Board
            {
                ProjectId = projectId,
                TeamId = null, // As per requirement
                Name = $"{projectName}-Board", // Changed to use ProjectName-Board format
                Description = "Default project board",
                Type = "default", // Default type
                IsActive = true,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };

            // Create default columns
            var columns = new List<BoardColumn>
            {
                new BoardColumn
                {
                    Id = Guid.NewGuid(),
                    StatusId = 1, // To Do
                    BoardColumnName = "To Do",
                    BoardColor = "#3498db", // Blue
                    Position = 1
                },
                new BoardColumn
                {
                    Id = Guid.NewGuid(),
                    StatusId = 2, // In-Progress
                    BoardColumnName = "In-Progress",
                    BoardColor = "#f39c12", // Yellow
                    Position = 2
                },
                new BoardColumn
                {
                    Id = Guid.NewGuid(),
                    StatusId = 3, // Done
                    BoardColumnName = "Done",
                    BoardColor = "#2ecc71", // Green
                    Position = 3
                }
            };

            // Create board with columns and mappings
            await _boardRepository.CreateBoardWithColumnsAsync(board, columns);
        }
    }
}