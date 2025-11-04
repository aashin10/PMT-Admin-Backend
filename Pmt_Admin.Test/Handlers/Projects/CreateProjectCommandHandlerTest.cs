using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Xunit;
using PmtAdmin.Application.Handlers.Projects;
using PmtAdmin.Application.Command.Projects;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;

public class CreateProjectCommandHandlerTest
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<IBoardRepository> _boardRepositoryMock;
    private readonly Mock<IProjectMemberRepository> _projectMemberRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateProjectCommandHandler _handler;

    public CreateProjectCommandHandlerTest()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _boardRepositoryMock = new Mock<IBoardRepository>();
        _projectMemberRepositoryMock = new Mock<IProjectMemberRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateProjectCommandHandler(
            _projectRepositoryMock.Object,
            _boardRepositoryMock.Object,
            _projectMemberRepositoryMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_InvalidMetadata_ReturnsFail()
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Metadata = "{invalidJson"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("Metadata must be a valid JSON string.", result.Message);
    }

    [Fact]
    public async Task Handle_SuccessfulCreation_ReturnsCreated()
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Name = "Test Project",
            Key = "TP",
            Description = "Test Desc",
            ProjectManagerId = 1,
            ProjectManagerRoleId = 2,
            CreatedBy = 1,
            Metadata = "{\"foo\":\"bar\"}"
        };

        var createdProject = new Project
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Key = command.Key,
            Description = command.Description,
            ProjectManagerId = command.ProjectManagerId,
            ProjectManagerRoleId = command.ProjectManagerRoleId,
            CreatedBy = command.CreatedBy,
            Metadata = command.Metadata
        };

        var projectWithDetails = new Project
        {
            Id = createdProject.Id,
            Name = createdProject.Name,
            Key = createdProject.Key,
            Description = createdProject.Description,
            ProjectManagerId = createdProject.ProjectManagerId,
            ProjectManagerRoleId = createdProject.ProjectManagerRoleId,
            CreatedBy = createdProject.CreatedBy,
            Metadata = createdProject.Metadata
        };

        var projectDto = new ProjectDTO
        {
            Id = createdProject.Id,
            Name = createdProject.Name,
            Key = createdProject.Key,
            Description = createdProject.Description
        };

        _projectRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Project>()))
            .ReturnsAsync(createdProject);

        _projectMemberRepositoryMock.Setup(r => r.GetByProjectAndUserAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync((ProjectMember)null);

        _projectMemberRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<ProjectMember>()))
            .ReturnsAsync(new ProjectMember());

        _boardRepositoryMock.Setup(r => r.CreateBoardWithColumnsAsync(It.IsAny<Board>(), It.IsAny<List<BoardColumn>>()))
            .ReturnsAsync(new Board());

        _projectRepositoryMock.Setup(r => r.GetProjectByIdWithDetailsAsync(createdProject.Id))
            .ReturnsAsync(projectWithDetails);

        _mapperMock.Setup(m => m.Map<ProjectDTO>(projectWithDetails))
            .Returns(projectDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(projectDto, result.Data);
        Assert.Equal("Project created successfully with default board", result.Message);

        // Verify that project member was created
        _projectMemberRepositoryMock.Verify(r => r.CreateAsync(It.Is<ProjectMember>(pm => 
            pm.ProjectId == createdProject.Id && 
            pm.UserId == command.ProjectManagerId && 
            pm.RoleId == command.ProjectManagerRoleId &&
            pm.IsOwner == true)), Times.Once);
    }

    [Fact]
    public async Task Handle_ProjectManagerAlreadyExists_DoesNotAddDuplicate()
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Name = "Test Project",
            ProjectManagerId = 1,
            ProjectManagerRoleId = 2,
            CreatedBy = 1
        };

        var createdProject = new Project
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            ProjectManagerId = command.ProjectManagerId,
            ProjectManagerRoleId = command.ProjectManagerRoleId,
            CreatedBy = command.CreatedBy
        };

        var existingProjectMember = new ProjectMember
        {
            Id = 1,
            ProjectId = createdProject.Id,
            UserId = command.ProjectManagerId,
            RoleId = command.ProjectManagerRoleId
        };

        _projectRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Project>()))
            .ReturnsAsync(createdProject);

        _projectMemberRepositoryMock.Setup(r => r.GetByProjectAndUserAsync(createdProject.Id, command.ProjectManagerId.Value))
            .ReturnsAsync(existingProjectMember);

        _boardRepositoryMock.Setup(r => r.CreateBoardWithColumnsAsync(It.IsAny<Board>(), It.IsAny<List<BoardColumn>>()))
            .ReturnsAsync(new Board());

        _projectRepositoryMock.Setup(r => r.GetProjectByIdWithDetailsAsync(createdProject.Id))
            .ReturnsAsync(createdProject);

        _mapperMock.Setup(m => m.Map<ProjectDTO>(It.IsAny<Project>()))
            .Returns(new ProjectDTO());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        // No Succeeded property, just check result.Data or result.Message as needed
        Assert.NotNull(result.Data);

        // Verify that project member was NOT created (since it already exists)
        _projectMemberRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<ProjectMember>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NoProjectManager_DoesNotAddProjectMember()
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Name = "Test Project",
            ProjectManagerId = null, // No project manager
            CreatedBy = 1
        };

        var createdProject = new Project
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            ProjectManagerId = null,
            CreatedBy = command.CreatedBy
        };

        _projectRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Project>()))
            .ReturnsAsync(createdProject);

        _boardRepositoryMock.Setup(r => r.CreateBoardWithColumnsAsync(It.IsAny<Board>(), It.IsAny<List<BoardColumn>>()))
            .ReturnsAsync(new Board());

        _projectRepositoryMock.Setup(r => r.GetProjectByIdWithDetailsAsync(createdProject.Id))
            .ReturnsAsync(createdProject);

        _mapperMock.Setup(m => m.Map<ProjectDTO>(It.IsAny<Project>()))
            .Returns(new ProjectDTO());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result.Data);

        // Verify that project member was NOT created (since no project manager was specified)
        _projectMemberRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<ProjectMember>()), Times.Never);
        _projectMemberRepositoryMock.Verify(r => r.GetByProjectAndUserAsync(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_ReturnsFail()
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Name = "Test Project"
        };

        _projectRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Project>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.StartsWith("Error creating project:", result.Message);
    }
}
