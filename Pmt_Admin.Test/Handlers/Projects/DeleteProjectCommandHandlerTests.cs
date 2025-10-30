using AutoMapper;
using FluentAssertions;
using Moq;
using PmtAdmin.Application.Command.Projects;
using PmtAdmin.Application.Handlers.Projects;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Pmt_Admin.Test.Handlers.Projects
{
    public class DeleteProjectCommandHandlerTests
    {
        private readonly Mock<IProjectRepository> _projectRepositoryMock;
        private readonly DeleteProjectCommandHandler _handler;

        public DeleteProjectCommandHandlerTests()
        {
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _handler = new DeleteProjectCommandHandler(_projectRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenProjectExists_ReturnsSuccess()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var command = new DeleteProjectCommand { Id = projectId };

            _projectRepositoryMock
                .Setup(x => x.SoftDeleteProjectAsync(projectId))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Should().Be("Project deleted successfully");
            result.Message.Should().Be("Request processed successfully");

            _projectRepositoryMock.Verify(x => x.SoftDeleteProjectAsync(projectId), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenProjectDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var command = new DeleteProjectCommand { Id = projectId };

            _projectRepositoryMock
                .Setup(x => x.SoftDeleteProjectAsync(projectId))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(404);
            result.Message.Should().Be("Project not found");
            result.Data.Should().BeNull();

            _projectRepositoryMock.Verify(x => x.SoftDeleteProjectAsync(projectId), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var command = new DeleteProjectCommand { Id = projectId };

            _projectRepositoryMock
                .Setup(x => x.SoftDeleteProjectAsync(projectId))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => 
                await _handler.Handle(command, CancellationToken.None));

            _projectRepositoryMock.Verify(x => x.SoftDeleteProjectAsync(projectId), Times.Once);
        }
    }
}
