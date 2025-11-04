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
    
    public class UpdateProjectCommandHandlerTests
    {
        private readonly Mock<IProjectRepository> _projectRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UpdateProjectCommandHandler _handler;

        public UpdateProjectCommandHandlerTests()
        {
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new UpdateProjectCommandHandler(_projectRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ProjectNotFound_ReturnsNotFound()
        {
            // Arrange
            var command = new UpdateProjectCommand { Id = Guid.NewGuid() };
            _projectRepositoryMock.Setup(r => r.GetProjectByIdWithDetailsAsync(command.Id))
                .ReturnsAsync((Project)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("Project not found", result.Message);
        }

        [Fact]
        public async Task Handle_InvalidMetadata_ReturnsFail()
        {
            // Arrange
            var command = new UpdateProjectCommand
            {
                Id = Guid.NewGuid(),
                Metadata = "{invalidJson"
            };
            var project = new Project { Id = command.Id, CustomFields = new List<CustomField>() };
            _projectRepositoryMock.Setup(r => r.GetProjectByIdWithDetailsAsync(command.Id))
                .ReturnsAsync(project);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("Metadata must be a valid JSON string.", result.Message);
        }

        [Fact]
        public async Task Handle_MetadataEmptyString_SetsMetadataNull()
        {
            // Arrange
            var command = new UpdateProjectCommand
            {
                Id = Guid.NewGuid(),
                Metadata = string.Empty
            };
            var project = new Project { Id = command.Id, CustomFields = new List<CustomField>() };
            _projectRepositoryMock.Setup(r => r.GetProjectByIdWithDetailsAsync(command.Id))
                .ReturnsAsync(project);
            _projectRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Project>()))
                .Returns(Task.FromResult<Project?>(null));
            _projectRepositoryMock.Setup(r => r.GetProjectByIdWithDetailsAsync(command.Id))
                .ReturnsAsync(project);
            _mapperMock.Setup(m => m.Map<ProjectDTO>(It.IsAny<Project>()))
                .Returns(new ProjectDTO());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Null(project.Metadata);
            Assert.Equal("Project updated successfully", result.Message);
        }

        [Fact]
        public async Task Handle_CustomFields_ClearAllFields()
        {
            // Arrange
            var command = new UpdateProjectCommand
            {
                Id = Guid.NewGuid(),
                CustomFields = new List<CustomFieldDTO>() // empty list
            };
            var project = new Project
            {
                Id = command.Id,
                CustomFields = new List<CustomField>
            {
                new CustomField { Id = Guid.NewGuid(), Name = "Field1", Value = "Value1" }
            }
            };
            _projectRepositoryMock.Setup(r => r.GetProjectByIdWithDetailsAsync(command.Id))
                .ReturnsAsync(project);
            _projectRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Project>()))
                .Returns(Task.FromResult<Project?>(null));
            _projectRepositoryMock.Setup(r => r.GetProjectByIdWithDetailsAsync(command.Id))
                .ReturnsAsync(project);
            _mapperMock.Setup(m => m.Map<ProjectDTO>(It.IsAny<Project>()))
                .Returns(new ProjectDTO());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Empty(project.CustomFields);
            Assert.Equal("Project updated successfully", result.Message);
        }

        [Fact]
        public async Task Handle_CustomFields_UpdateAndAdd()
        {
            // Arrange
            var existingFieldId = Guid.NewGuid();
            var command = new UpdateProjectCommand
            {
                Id = Guid.NewGuid(),
                CustomFields = new List<CustomFieldDTO>
            {
                new CustomFieldDTO { Id = existingFieldId, Name = "UpdatedName", Value = "UpdatedValue" },
                new CustomFieldDTO { Id = Guid.Empty, Name = "NewField", Value = "NewValue" }
            }
            };
            var project = new Project
            {
                Id = command.Id,
                CustomFields = new List<CustomField>
            {
                new CustomField { Id = existingFieldId, Name = "OldName", Value = "OldValue" }
            }
            };
            _projectRepositoryMock.Setup(r => r.GetProjectByIdWithDetailsAsync(command.Id))
                .ReturnsAsync(project);
            _projectRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Project>()))
                .Returns(Task.FromResult<Project?>(null));
            _projectRepositoryMock.Setup(r => r.GetProjectByIdWithDetailsAsync(command.Id))
                .ReturnsAsync(project);
            _mapperMock.Setup(m => m.Map<ProjectDTO>(It.IsAny<Project>()))
                .Returns(new ProjectDTO());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Contains(project.CustomFields, f => f.Name == "UpdatedName" && f.Value == "UpdatedValue");
            Assert.Contains(project.CustomFields, f => f.Name == "NewField" && f.Value == "NewValue");
            Assert.Equal("Project updated successfully", result.Message);
        }

        [Fact]
        public async Task Handle_SuccessfulUpdate_ReturnsSuccess()
        {
            // Arrange
            var command = new UpdateProjectCommand
            {
                Id = Guid.NewGuid(),
                Name = "Test Project",
                CustomFields = null
            };
            var project = new Project { Id = command.Id, CustomFields = new List<CustomField>() };
            var projectDto = new ProjectDTO { Id = command.Id, Name = "Test Project" };

            _projectRepositoryMock.SetupSequence(r => r.GetProjectByIdWithDetailsAsync(command.Id))
                .ReturnsAsync(project) // first call
                .ReturnsAsync(project); // after update
            _projectRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Project>()))
                .Returns(Task.FromResult<Project?>(null));
            _mapperMock.Setup(m => m.Map<ProjectDTO>(It.IsAny<Project>()))
                .Returns(projectDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("Project updated successfully", result.Message);
            Assert.Equal(projectDto, result.Data);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ReturnsFail()
        {
            // Arrange
            var command = new UpdateProjectCommand { Id = Guid.NewGuid() };
            _projectRepositoryMock.Setup(r => r.GetProjectByIdWithDetailsAsync(command.Id))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.StartsWith("Error updating project:", result.Message);
        }
    }
}
