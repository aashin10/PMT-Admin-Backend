using AutoMapper;
using Moq;
using PmtAdmin.Application.Command.Projects;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Handlers.Projects;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pmt_Admin.Test.Handlers.Projects
{


    public class CreateProjectCommandHandlerTests
    {
        private readonly Mock<IProjectRepository> _projectRepositoryMock;
        private readonly Mock<IBoardRepository> _boardRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CreateProjectCommandHandler _handler;

        public CreateProjectCommandHandlerTests()
        {
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _boardRepositoryMock = new Mock<IBoardRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new CreateProjectCommandHandler(
                _projectRepositoryMock.Object,
                _boardRepositoryMock.Object,
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
                CreatedBy = 1,
                Metadata = "{\"foo\":\"bar\"}"
            };

            var createdProject = new Project
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                Key = command.Key,
                Description = command.Description,
                CreatedBy = command.CreatedBy,
                Metadata = command.Metadata
            };

            var projectWithDetails = new Project
            {
                Id = createdProject.Id,
                Name = createdProject.Name,
                Key = createdProject.Key,
                Description = createdProject.Description,
                CreatedBy = createdProject.CreatedBy,
                Metadata = createdProject.Metadata
            };

            var projectDto = new ProjectDTO
            {
                CustomerOrgName = "Org",
                CustomerDomainUrl = "http://org.com",
                CreatedAt = DateTime.UtcNow
            };

            _projectRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Project>()))
                .ReturnsAsync(createdProject);

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
}
